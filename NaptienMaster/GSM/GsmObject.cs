using Krypton.Toolkit;
using NaptienMaster.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GsmComm.PduConverter;
using System.Web;
using System.Web.Security;
using System.Web.WebSockets;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace NaptienMaster.GSM
{
    public class GsmObject
    {
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private SemaphoreSlim lockWriterPort = new SemaphoreSlim(1, 1);
        private SemaphoreSlim orderHandlingSlim = new SemaphoreSlim(1, 1);
        private SemaphoreSlim lockWritePort = new SemaphoreSlim(1);
        private string port;
        private string telco;
        private string phone;
        private string status;
        private string tkc;
        private string tkkm;
        private string charge;
        private string charged;
        private string expire;
        private string msg;
        private string simtype;
        private bool _isSim;
        public bool _isInfor;
        private bool isTKKM;
        public bool isRecheckInfo;
        private bool is_wintel = false;
        private bool recheckInfo;
        private bool checkSimHasResult = false;
        private bool stopRead=false;
        private bool isFailSendSMS;
        private bool isreadport;
        private object lockReadPort = new object();
        public SerialPort sp;
        private DateTime lastReportPhone = DateTime.MinValue;
        private DateTime lastReportNetwork = DateTime.MinValue;
        private DateTime lastReportInfo = DateTime.MinValue;
        private DateTime lastReportSerialData = DateTime.MinValue;
        private DateTime lastTKKMReport = DateTime.MinValue;
        private DateTime lastRecheckInfo = DateTime.MinValue;
        public DateTime lastRunTopUp = DateTime.MinValue;
        public DateTime lastRunCharge = DateTime.MinValue;
        public DateTime lastSendSmS=DateTime.MinValue;
        public bool CarrierHasResult;
        public string phone_secondary_temp = "";
        private string text = "";
        private bool _isPhone;
        private int oldindexsms;
        private bool check_11_dig = false;
        private bool delallsms = true;
        private bool isTopUp = false;
        public bool is_Topup=false;
        public bool lock_pin = false;
        private bool is_updateTKC = false;
        private bool isCheckCharge = false;
        private DateTime timeOutExit = DateTime.MinValue;
        private string data;
        private bool is_send_sms = true;
        private string current_sms;
        private bool smsSuccess;
        public string updateTK = "";
        private CancellationTokenSource cts;
        public string ussd_balance;
        public string ussd_charge;
        public string topupResult = "";
        public string checkChargeValue = "";
        public string message_sim_type = "";
        public string after_card_real_amount = "";
        public bool test_ussd = false;
        public string first_request_sim_type = "";
        public DataGridViewRow rowGSMSelected = new DataGridViewRow();
        public Form1 instance = Form1.ReturnInstance();
        public Login login_instance = Login.ReturnLoginInstance();
        private TimeZoneInfo vietnam_standard_time = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public Task task { get; set; }

        public GsmObject(string port,DataGridViewRow row)
        {
            this.rowGSMSelected = row;
            this.Port = port;
            this.Phone = "";
            this.Expire = "";
            this.Telco = "";
            this.Status = "Waiting";
            this.TKC = "";
            this.Message = loadMsg("Đang chờ nhận Sim");
            this.SimType = "";
            this.TKKM = "";
            this.Charge = "";
            this.Charged = "";
            
            cts = new CancellationTokenSource();
            this.sp = new SerialPort()
            {
                PortName = port,
                BaudRate = int.Parse(this.instance.baudrate),
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.RequestToSend,
                NewLine = "\r\n",
                WriteTimeout = 1000,
                ReadTimeout = 5000
            };
            this.sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            this.task = Task.Run(() => this.Work(), cts.Token);
        }
        public GsmObject(string port)
        {
            this.rowGSMSelected = instance.newRow();
            this.Port = port;
            this.Telco = "";
            this.Phone = "";
            this.Status = "Waiting";
            this.TKC = "";
            this.Expire = "";
            this.Message = this.loadMsg("Đang lấy thông tin");
            this.SimType = "";
            this.TKKM = "";
            this.Charge = "";
            this.Charged = "";
        }
        public string Port
        {
            get => this.port;
            set
            {
                this.port = value;
                UpdateGUI.ChangeRow(rowGSMSelected, "port", value);
            }
        }
        public string Phone
        {
            get => this.phone;
            set
            {
                this.phone = value;
                UpdateGUI.ChangeRow(rowGSMSelected, "sdt", value);
            }
        }
        public string TKC
        {
            get => this.tkc;
            set
            {
                this.tkc = value;
                UpdateGUI.ChangeRow(rowGSMSelected, "tkc", value);
            }
        }
        public string Telco
        {
            get => this.telco;
            set
            {
                this.telco = value;
                UpdateGUI.ChangeRow(rowGSMSelected, "telco", value);
            }
        }
        public string Status
        {
            get => this.status;
            set
            {
                this.status = value;
                UpdateGUI.ChangeRow(rowGSMSelected, "status", value);
            }
        }
        public string TKKM
        {
            get => this.tkkm;
            set
            {
                this.tkkm = value;
                UpdateGUI.ChangeRow(rowGSMSelected, "tkkm", value);
            }
        }
        public string Expire
        {
            get => this.expire;
            set
            {
                this.expire = value;
                UpdateGUI.ChangeRow(rowGSMSelected, "expire", value);
            }
        }
        public string Message
        {
            get => this.msg;
            set
            {
                this.msg = value;
                UpdateGUI.ChangeRow(rowGSMSelected, "message", value);
            }
        }
        public string SimType
        {
            get => this.simtype;
            set
            {
                this.simtype = value;
                UpdateGUI.ChangeRow(rowGSMSelected, "simtype", value);
            }
        }
        public string Charge
        {
            get => this.charge;
            set
            {
                this.charge = value;
                UpdateGUI.ChangeRow(rowGSMSelected, "charge", value);
            }
        }
        public string Charged
        {
            get => this.charged;
            set
            {
                this.charged = value;
                UpdateGUI.ChangeRow(rowGSMSelected, "charged", value);
            }
        }
        
        public string loadMsg(string message)
        {
            this.Message = "";
            string res = "[" + DateTime.Now + "]" + message;
            return res;
        }
        public async Task sendAT(string command)
        {
            await this.lockWriterPort.WaitAsync();
            try
            {
                this.sp.WriteLine(command);
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
            }
            finally
            {
                this.lockWriterPort.Release();
            }
        }
        public async Task runUSSD(string command)
        {
            try
            {
                await this.sendAT("AT+CMGF=0");
                await Task.Delay(100);
                await this.sendAT("AT+CUSD=2");
                await Task.Delay(100);
                await this.sendAT($"AT+CUSD=1,\"{command}\",15\r");
        
            }
            catch (Exception er)
            {
                this.instance.loadData(er.Message);
            }
        }
        public bool TryOpenPort()
        {
            try
            {
                if (!this.sp.IsOpen)
                {
                    this.sp.Open();
                }
                this.sp.DiscardInBuffer();
                this.sp.DiscardOutBuffer();
                return this.sp.IsOpen;
            }
            catch (Exception er)
            {
                this.instance.loadData(er.Message);
                return false;
            }
        }
       
        public async Task CheckSimReady()
        {
            try
            {
                this.checkSimHasResult = false;
                this._isSim = true;
                await this.sendAT("AT+CPIN?");
                await this.sendAT("AT+CGMI?");
            }
            catch(Exception er)
            {
                this.instance.loadData(er.Message);
            }
        }
        public void DataReceivedHandler(object sender,SerialDataReceivedEventArgs e)
        {
            if (!Monitor.TryEnter(this.lockReadPort, 2000))
            {
                return;
            }
            try
            {  if (stopRead == false)
                {
                    this.data += this.sp.ReadExisting();
                    if (this.data.EndsWith("\n") || this.data.EndsWith("\r"))
                    {
                        this.lastReportSerialData = DateTime.Now;
                        if (this.data.Contains("+CUSD:"))
                        {
                            if (this.data.EndsWith("OK\r\n"))
                            {
                                this.HandleSerialData(data);
                                data = "";
                            }

                        }
                        else
                        {
                            this.HandleSerialData(data);
                            data = "";
                        }
                    }
                }

            }
            catch (Exception er)
            {
                this.instance.loadData(er.Message);
            }
            finally
            {
                Monitor.Exit(this.lockReadPort);
            }
        }
        public string ConvertToDecimal(string input)
        {
            int value = Convert.ToInt32(input);
            return value.ToString("C2").Replace("$", "");

        }
        //ham reset row
        public void reset(string port, DataGridViewRow row)
        {
            this.rowGSMSelected = row;
            this.Port = port;
            this.Telco = "";
            this.Phone = "";
            this.Status = "Waiting";
            this.TKC = "";
            this.Expire = "";
            this.SimType = "";
            this.Message = this.loadMsg("Đang lấy thông tin");
            this.TKKM = "";
            this.Charge = "";
            this.Charged = "";
            this.isTopUp = false;
            this.topupResult = "";
            this.message_sim_type = "";
            this.after_card_real_amount = "";
            this.checkChargeValue = "";
            this.CarrierHasResult = false;
            this.first_request_sim_type = "";
            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
            {
                this.rowGSMSelected.Cells["status"].Style.BackColor = Color.Transparent;
            }));
            cts.Cancel();
            this.sp.BaudRate = int.Parse(this.instance.baudrate);
            this.sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            this.task = Task.Run(() => this.Work(), cts.Token);
        }
        //ham reset tkc
        public void resetUSSD(string port,DataGridViewRow row)
        {
            this.rowGSMSelected = row;
            this.Port = port;
            this.TKC = "";
            this.test_ussd = true;
            this._isInfor = true;
            this.lastReportInfo = DateTime.Now;
            this.is_updateTKC = true;
            this.ussd_balance = "";
        }

        public void resetCheckCharge(string port,DataGridViewRow row)
        {
            this.rowGSMSelected = row;
            this.Port = port;
            this.ussd_charge = "";
            this.checkChargeValue = "";
            this.isCheckCharge = true;
            this.lastRunCharge = DateTime.Now;
        }
        //ham xu ly don
        public async Task<int> OrderHandling(string phone, string code, RechargeOrder info)
        {
            int res = -1;
            await orderHandlingSlim.WaitAsync();
            try
            {
                DateTime topUpResponse = DateTime.Now;
                string reply = "";
                string reply_push_recharge = "";
                string network = this.Telco;
                string report = "";
                string before_balance = "";
                string after_balance = "";
                string card_real_amount = "";
                string balance = "";
                string sim_type = this.SimType;
                bool retry_topup = true;
                bool retry_ussd = true;
                Regex regex_ussd = new Regex(@"da nap [\d,]+\s+VND");
                DateTime ussd_balance_timeout;
                Guid guid = Guid.NewGuid();
                if (this.Status == "Đang xử lý")
                    {
                    return res;
                    }
                this.after_card_real_amount = "";
                string info_log = info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                    LoggerManager.LogInfo(info_log);
                    DateTime datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                    reply_push_recharge = $"{{\"command\":\"PUSH_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Bắt đầu xử lý\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                    var json_object_push_recharge = JsonConvert.DeserializeObject(reply_push_recharge);
                    string res_json_push_recharge = JsonConvert.SerializeObject(json_object_push_recharge, Formatting.Indented);
                    login_instance.wsHelper.sendDataToServer(res_json_push_recharge);
                if (this.SimType == "TT")
                {
                    before_balance = this.TKC;
                }
                else if(this.SimType == "TS")
                {
                    before_balance = this.checkChargeValue; 
                }
                //neu before_balance la null và thuộc sim trả trước thì sẽ dừng nạp và hủy kết nối sim
                  if (string.IsNullOrEmpty(before_balance) && this.SimType=="TT")
                    {
                    res = -1;
                    reply_push_recharge = $"{{\"command\":\"PUSH_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":false,\"message\":\"Sim bị lỗi không hiện tài khoản chính.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                    json_object_push_recharge = JsonConvert.DeserializeObject(reply_push_recharge);
                    res_json_push_recharge = JsonConvert.SerializeObject(json_object_push_recharge, Formatting.Indented);
                    login_instance.wsHelper.sendDataToServer(res_json_push_recharge);
                    string error_res = this.instance.pushSingleSimInfo(phone, 0);
                    if (!string.IsNullOrEmpty(error_res))
                    {
                        this.login_instance.wsHelper.sendDataToServer(error_res);
                        LoggerManager.LogTrace("Lỗi không nhận được balance:" + error_res);
                    }
                    report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Sim bị lỗi không hiện tài khoản chính.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{network}\",\"sim_type\":\"{sim_type}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tổng cước nợ của bạn là:{before_balance}\",\"after_balance_ussd\":\"Tổng cước nợ của bạn là:{after_balance}\",\"card_real_amount\":\"0\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                    var report_sim_disconnect = JsonConvert.DeserializeObject(report);
                    string res_report_sim_disconnect = JsonConvert.SerializeObject(report_sim_disconnect);
                    if (Properties.Settings.Default.reportRechargeOrderList == null)
                    {
                        Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                    }
                    Properties.Settings.Default.reportRechargeOrderList.Add(res_report_sim_disconnect);
                    Properties.Settings.Default.Save();
                    return res;
                    }
                    //neu before_balance la null va thuoc sim tra sau thi dung nap thi huy ket noi sim
                    else if((string.IsNullOrEmpty(before_balance)) && this.SimType=="TS")
                {
                    res = -1;
                    reply_push_recharge = $"{{\"command\":\"PUSH_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":false,\"message\":\"Sim bị lỗi không truy xuất được nợ cước.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                    json_object_push_recharge = JsonConvert.DeserializeObject(reply_push_recharge);
                    res_json_push_recharge = JsonConvert.SerializeObject(json_object_push_recharge, Formatting.Indented);
                    login_instance.wsHelper.sendDataToServer(res_json_push_recharge);
                    string error_res = this.instance.pushSingleSimInfo(phone, 0);
                    if (!string.IsNullOrEmpty(error_res))
                    {
                        this.login_instance.wsHelper.sendDataToServer(error_res);
                        LoggerManager.LogTrace("Lỗi không tra cứu được nợ cước của sim:" + error_res);
                    }
                    report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Sim bị lỗi không truy xuất được nợ cước.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{network}\",\"sim_type\":\"{sim_type}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tổng cước nợ của bạn là:{before_balance}\",\"after_balance_ussd\":\"Tổng cước nợ của bạn là:{after_balance}\",\"card_real_amount\":\"0\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                    var report_sim_disconnect = JsonConvert.DeserializeObject(report);
                    string res_report_sim_disconnect = JsonConvert.SerializeObject(report_sim_disconnect);
                    if (Properties.Settings.Default.reportRechargeOrderList == null)
                    {
                        Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                    }
                    Properties.Settings.Default.reportRechargeOrderList.Add(res_report_sim_disconnect);
                    Properties.Settings.Default.Save();
                    this.instance.transactionUpdate();
                    return res;
                }
                    this.Message = this.loadMsg("Bắt đầu xử lý");
                    this.Status = "Đang xử lý";
                    this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                    {
                           this.rowGSMSelected.Cells["status"].Style.BackColor = Color.Aquamarine;    
                    }));
                //Tien hanh nap the
                    this.runTopUp(code);
                    while (DateTime.Now.Subtract(topUpResponse).TotalSeconds<60 && string.IsNullOrEmpty(this.topupResult))
                    {  if (DateTime.Now.Subtract(topUpResponse).TotalSeconds > 30 && retry_topup && isTopUp)
                    {
                        if (this.lock_pin)
                        {
                            this.lock_pin = false;
                        }
                        retry_topup = false;
                        await Task.Delay(500);
                        this.runTopUp(code);
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                    }
                if (!string.IsNullOrEmpty(this.topupResult))
                {
                    //case nap the that bai khi co ussd message tra ve
                    if (this.SimType == "TS")
                    {
                        info_log = "Thông tin nạp:" + info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                        LoggerManager.LogInfo(info_log);
                        res = 0;
                        ussd_balance_timeout = DateTime.Now;
                        this.resetCheckCharge(this.Port, this.rowGSMSelected);
                        while(string.IsNullOrEmpty(this.ussd_charge))
                        {
                            if (string.IsNullOrEmpty(this.Telco))
                            {
                                report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Sim bị mất kết nối trong quá trình nạp.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{network}\",\"sim_type\":\"{sim_type}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tổng cước nợ của bạn là:{before_balance}\",\"after_balance_ussd\":\"Tổng cước nợ của bạn là:{after_balance}\",\"card_real_amount\":\"0\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                var report_sim_disconnect = JsonConvert.DeserializeObject(report);
                                string res_report_sim_disconnect = JsonConvert.SerializeObject(report_sim_disconnect);
                                if (Properties.Settings.Default.reportRechargeOrderList == null)
                                {
                                    Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                                }
                                Properties.Settings.Default.reportRechargeOrderList.Add(res_report_sim_disconnect);
                                Properties.Settings.Default.Save();
                                this.instance.transactionUpdate();
                                return res;
                            }
                            else
                            {
                                await Task.Delay(100);
                            }
                        }
                        after_balance = ussd_charge;
                        if(this.is_Topup == false)
                        {

                            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                            {
                                this.rowGSMSelected.Cells["status"].Style.BackColor = Color.IndianRed;
                            }));
                            this.Status = "Thất bại";
                            this.Message = this.loadMsg(this.topupResult);
                            double before_balance_value = 0;
                            double after_balance_value = 0;
                            try
                            {
                                before_balance_value = double.Parse(this.instance.balance_standard(before_balance));
                            }
                            catch (Exception err)
                            {
                                LoggerManager.LogError("before_balance_parse:" + err.Message);
                            }
                            try
                            {
                                after_balance_value = double.Parse(this.instance.balance_standard(after_balance));
                            }
                            catch(Exception err)
                            {
                                LoggerManager.LogError("after_balance_parse:" + err.Message);
                            }
                            balance = after_balance_value.ToString();
                            card_real_amount = "0";
                            try
                            {
                                datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);

                                reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Cước nợ trước của bạn là:{before_balance}\",\"after_balance_ussd\":\"Cước nợ trước của bạn là:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                var json_object = JsonConvert.DeserializeObject(reply);
                                string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                LoggerManager.LogTrace(res_json);
                                login_instance.wsHelper.sendDataToServer(res_json);
                            }
                            catch (Exception er)
                            {
                                LoggerManager.LogError("update_recharge_order:" + er.Message);
                            }
                            report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Cước nợ trước của bạn là:{before_balance}\",\"after_balance_ussd\":\"Cước nợ trước của bạn là:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";

                            var report_json_object = JsonConvert.DeserializeObject(report);

                            string res_report = JsonConvert.SerializeObject(report_json_object);
                            if (Properties.Settings.Default.reportRechargeOrderList == null)
                            {
                                Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                            }
                            Properties.Settings.Default.reportRechargeOrderList.Add(res_report);
                            Properties.Settings.Default.Save();
                            this.instance.transactionUpdate();
                            string error_message = this.topupResult;
                            if (error_message.Contains("Dich vu nay khong duoc"))
                            {
                                string error_res = this.instance.pushSingleSimInfo(phone, 0);
                                if (!string.IsNullOrEmpty(error_res))
                                {
                                    this.login_instance.wsHelper.sendDataToServer(error_res);
                                    LoggerManager.LogTrace("Lỗi dịch vụ không cho phép:" + error_res);
                                }
                            }

                        }
                        else {
                            res = 1;
                            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                            {
                                this.rowGSMSelected.Cells["status"].Style.BackColor = Color.GreenYellow;
                            }));
                            this.Status = "Thành công";
                            this.Message = this.loadMsg(this.topupResult);
                            string card_amount = info.Payload.Card_Amount;
                            if (!string.IsNullOrEmpty(this.Charge) && !string.IsNullOrEmpty(this.Charged))
                            {
                                this.Charged = (int.Parse(this.Charged.Replace(",", "").Replace(".", "")) + int.Parse(this.instance.balance_standard(card_amount))).ToString();
                            }
                            double before_balance_value = 0;
                            double after_balance_value = 0;
                            try
                            {
                                before_balance_value = double.Parse(this.instance.balance_standard(before_balance));
                            }
                            catch (Exception err)
                            {
                                LoggerManager.LogError("before_balance_parse:" + err.Message);
                            }
                            after_balance_value = double.Parse(this.instance.balance_standard(after_balance));
                            balance = after_balance_value.ToString();
                            while(string.IsNullOrEmpty(after_card_real_amount))
                            {
                                await Task.Delay(100);
                            }
                            card_real_amount = after_card_real_amount;
                            try
                            {
                                datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                                reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Cước nợ trước của bạn là:{before_balance}\",\"after_balance_ussd\":\"Cước nợ trước của bạn là:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                var json_object = JsonConvert.DeserializeObject(reply);
                                string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                LoggerManager.LogTrace(res_json);
                                login_instance.wsHelper.sendDataToServer(res_json);
                            }
                            catch (Exception er)
                            {
                                LoggerManager.LogError("update_recharge_order:" + er.Message);
                            }
                            report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Cước nợ trước của bạn là:{before_balance}\",\"after_balance_ussd\":\"Cước nợ trước của bạn là:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";

                            var report_json_object = JsonConvert.DeserializeObject(report);

                            string res_report = JsonConvert.SerializeObject(report_json_object);
                            if (Properties.Settings.Default.reportRechargeOrderList == null)
                            {
                                Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                            }
                            Properties.Settings.Default.reportRechargeOrderList.Add(res_report);
                            Properties.Settings.Default.Save();
                            this.instance.transactionUpdate();                           
                        }
                    }
                    else
                    {
                        info_log = "Thất bại:" + info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                        LoggerManager.LogInfo(info_log);
                        res = 0;
                        ussd_balance_timeout = DateTime.Now;
                        this.resetUSSD(this.Port, this.rowGSMSelected);
                        while (string.IsNullOrEmpty(this.ussd_balance))
                        {
                            if (string.IsNullOrEmpty(this.Telco))
                            {  
                                report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Sim bị mất kết nối trong quá trình nạp.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{network}\",\"sim_type\":\"{sim_type}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tài khoản của bạn là:{before_balance}\",\"after_balance_ussd\":\"Tài khoản của bạn là:{after_balance}\",\"card_real_amount\":\"0\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                var report_sim_disconnect = JsonConvert.DeserializeObject(report);
                                string res_report_sim_disconnect = JsonConvert.SerializeObject(report_sim_disconnect);
                                if (Properties.Settings.Default.reportRechargeOrderList == null)
                                {
                                    Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                                }
                                Properties.Settings.Default.reportRechargeOrderList.Add(res_report_sim_disconnect);
                                Properties.Settings.Default.Save();
                                this.instance.transactionUpdate();
                                return res;
                            }
                            if (DateTime.Now.Subtract(ussd_balance_timeout).TotalSeconds > 20 && retry_ussd)
                            {
                                try
                                {
                                    retry_ussd = false;
                                    this.resetUSSD(this.Port, this.rowGSMSelected);
                                    ussd_balance_timeout = DateTime.Now;
                                }
                                catch (Exception er)
                                {
                                    LoggerManager.LogError(er.Message);
                                }
                            }
                            else
                            {
                                await Task.Delay(100);
                            }
                        }
                        after_balance = this.ussd_balance;
                        if (this.is_Topup == false)
                        {  
                            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                            {
                                this.rowGSMSelected.Cells["status"].Style.BackColor = Color.IndianRed;
                            }));
                            this.Status = "Thất bại";
                            this.Message = this.loadMsg(this.topupResult);
                            double before_balance_value = 0;
                            double after_balance_value = 0;
                            try
                            {
                                before_balance_value = double.Parse(this.instance.balance_standard(before_balance));
                            }
                            catch (Exception err)
                            {
                                LoggerManager.LogError("before_balance_parse:" + err.Message);
                            }
                            if (!string.IsNullOrEmpty(after_balance))
                            {
                                try
                                {
                                    after_balance_value = double.Parse(this.instance.balance_standard(after_balance));
                                }
                                catch (Exception err)
                                {
                                    LoggerManager.LogError("after_balance_parse:" + err.Message);
                                    this.Message = this.loadMsg("Sim gặp lỗi trong quá trình nạp.Rút sim và gắm lại để tiếp tục nạp.");
                                    after_balance = before_balance;
                                    after_balance_value = before_balance_value;
                                    balance = after_balance_value.ToString();
                                    card_real_amount = "0";
                                    reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                    var json_object = JsonConvert.DeserializeObject(reply);
                                    string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                    LoggerManager.LogTrace(res_json);
                                    report = $"{{\"command\":\" REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"sim_type\":\"TT\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                    var report_ob = JsonConvert.DeserializeObject(report);
                                    string res_report_ob = JsonConvert.SerializeObject(report_ob);
                                    if (Properties.Settings.Default.reportRechargeOrderList == null)
                                    {
                                        Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                                    }
                                    Properties.Settings.Default.reportRechargeOrderList.Add(res_report_ob);
                                    Properties.Settings.Default.Save();
                                    this.instance.transactionUpdate();
                                    login_instance.wsHelper.sendDataToServer(res_json);
                                    string error_res = this.instance.pushSingleSimInfo(phone, 0);
                                    this.login_instance.wsHelper.sendDataToServer(error_res);
                                    return res;
                                }
                            }
                            balance = after_balance_value.ToString();
                            card_real_amount = (after_balance_value - before_balance_value).ToString();
                            try
                            {
                                datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                                reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                var json_object = JsonConvert.DeserializeObject(reply);
                                string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                LoggerManager.LogTrace(res_json);
                                login_instance.wsHelper.sendDataToServer(res_json);
                            }
                            catch (Exception er)
                            {
                                LoggerManager.LogError("update_recharge_order:" + er.Message);
                            }
                            report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";

                            var report_json_object = JsonConvert.DeserializeObject(report);

                            string res_report = JsonConvert.SerializeObject(report_json_object);
                            if (Properties.Settings.Default.reportRechargeOrderList == null)
                            {
                                Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                            }
                            Properties.Settings.Default.reportRechargeOrderList.Add(res_report);
                            Properties.Settings.Default.Save();
                            this.instance.transactionUpdate();
                            string error_message = this.topupResult;
                            if (error_message.Contains("Dich vu nay khong duoc"))
                            {
                                string error_res = this.instance.pushSingleSimInfo(phone, 0);
                                if (!string.IsNullOrEmpty(error_res))
                                {
                                    this.login_instance.wsHelper.sendDataToServer(error_res);
                                    LoggerManager.LogTrace("Lỗi dịch vụ không cho phép:" + error_res);
                                }
                            }
                        }
                        else
                        {  //case nap the thanh cong khi co ussd message tra ve
                            res = 1;
                            info_log = "Thành công:" + info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                            LoggerManager.LogInfo(info_log);
                            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                            {

                                this.rowGSMSelected.Cells["status"].Style.BackColor = Color.GreenYellow;

                            }));
                            this.Status = "Thành công";
                            this.Message = this.loadMsg(this.topupResult);
                            string card_amount = info.Payload.Card_Amount;
                            if (!string.IsNullOrEmpty(this.Charge) && !string.IsNullOrEmpty(this.Charged))
                            {
                                this.Charged = (int.Parse(this.Charged.Replace(",", "").Replace(".", "")) + int.Parse(card_amount.Replace(",", "").Replace(".", ""))).ToString();
                            }
                            double before_balance_value=0;
                            double after_balance_value=0;
                            try
                            {
                                before_balance_value = double.Parse(this.instance.balance_standard(before_balance));
                            }
                            catch(Exception er)
                            {
                                LoggerManager.LogError(er.Message);
                            }
                            if (!string.IsNullOrEmpty(after_balance))
                            {
                                try
                                {
                                    after_balance_value = double.Parse(this.instance.balance_standard(after_balance));
                                }
                                catch (Exception er)
                                {
                                    LoggerManager.LogError("after_balance_parse" + er.Message);
                                    this.Message = this.loadMsg("Sim đã nạp thành công nhưng gặp lỗi.Rút sim và gắm lại để nạp tiếp.");
                                    after_balance_value = before_balance_value + double.Parse(card_amount);
                                    after_balance = after_balance_value.ToString();
                                    balance = after_balance_value.ToString();
                                    Match match_card_amount = regex_ussd.Match(topupResult);
                                    if (match_card_amount.Success)
                                    {
                                        string card_real_amount_temp = match_card_amount.Value;
                                        Regex reg_card_amount = new Regex(@"[^\d,]");
                                        card_real_amount = reg_card_amount.Replace(card_real_amount_temp, "");
                                    }
                                    reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                    var json_object = JsonConvert.DeserializeObject(reply);
                                    string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                    LoggerManager.LogTrace(res_json);
                                    report = $"{{\"command\":\" REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"sim_type\":\"TT\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\"}}";
                                    var report_ob = JsonConvert.DeserializeObject(report);
                                    string res_report_ob = JsonConvert.SerializeObject(report_ob);
                                    if (Properties.Settings.Default.reportRechargeOrderList == null)
                                    {
                                        Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                                    }
                                    Properties.Settings.Default.reportRechargeOrderList.Add(res_report_ob);
                                    Properties.Settings.Default.Save();
                                    this.instance.transactionUpdate();
                                    login_instance.wsHelper.sendDataToServer(res_json);
                                    string error_res = this.instance.pushSingleSimInfo(phone, 0);
                                    this.login_instance.wsHelper.sendDataToServer(error_res);
                                    return res;
                                }
                            }
                            balance = after_balance_value.ToString();
                            Match match_card_real_amount=regex_ussd.Match(topupResult);
                            if(match_card_real_amount.Success)
                            {
                                string card_real_amount_temp = match_card_real_amount.Value;
                                Regex reg_card_amount = new Regex(@"[^\d,]");
                                card_real_amount = reg_card_amount.Replace(card_real_amount_temp, "");
                            }
                            datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                            try
                            {
                                reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                var json_object = JsonConvert.DeserializeObject(reply);
                                string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                LoggerManager.LogTrace(res_json);
                                login_instance.wsHelper.sendDataToServer(res_json);
                            }
                            catch (Exception er)
                            {
                                LoggerManager.LogError("update_recharge_order:" + er.Message);
                            }
                            report = $"{{\"command\":\" REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                            var report_json_object = JsonConvert.DeserializeObject(report);
                            string res_report = JsonConvert.SerializeObject(report_json_object);
                            if (Properties.Settings.Default.reportRechargeOrderList == null)
                            {
                                Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                            }
                            Properties.Settings.Default.reportRechargeOrderList.Add(res_report);
                            Properties.Settings.Default.Save();
                            this.instance.transactionUpdate();
                        }
                    }
                    this.after_card_real_amount = "";
                    this.topupResult = "";
                }
                else
                {  //case khong co ussd message tra ve
                    if (this.lock_pin)
                    {
                        this.lock_pin = false;
                    }
                    this.Message = "Tiến hành tính toán thẻ.";
                    ussd_balance_timeout = DateTime.Now;
                    if (this.SimType == "TS")
                    {
                        this.resetCheckCharge(this.Port, this.rowGSMSelected);
                        while (string.IsNullOrEmpty(this.ussd_charge))
                        {
                            if (string.IsNullOrEmpty(this.Telco))
                            {
                                res = 0;
                                string err_msg = "Sim bị mất kết nối trong quá trình nạp.";
                                report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Sim bị mất kết nối và không rõ kết quả nạp.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{err_msg}\",\"network\":\"{network}\",\"sim_type\":\"{sim_type}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tong no cuoc cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tong no cuoc cua ban la:{after_balance}\",\"card_real_amount\":\"0\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                var report_sim_disconnect = JsonConvert.DeserializeObject(report);

                                string res_report_sim_disconnect = JsonConvert.SerializeObject(report_sim_disconnect);
                                if (Properties.Settings.Default.reportRechargeOrderList == null)
                                {
                                    Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                                }
                                Properties.Settings.Default.reportRechargeOrderList.Add(res_report_sim_disconnect);
                                Properties.Settings.Default.Save();
                                this.instance.transactionUpdate();
                                return res;
                            }
                            else
                            {
                                await Task.Delay(100);
                            }
                        }
                        after_balance = this.ussd_charge;
                    }
                    else
                    {
                        this.resetUSSD(this.Port, this.rowGSMSelected);
                        while (string.IsNullOrEmpty(this.ussd_balance))
                        {
                            if (string.IsNullOrEmpty(this.Telco))
                            {
                                res = 0;
                                string err_msg = "Sim bị mất kết nối trong quá trình nạp.";
                                report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Sim bị mất kết nối và không rõ kết quả nạp.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{err_msg}\",\"network\":\"{network}\",\"sim_type\":\"{sim_type}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"0\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                var report_sim_disconnect = JsonConvert.DeserializeObject(report);

                                string res_report_sim_disconnect = JsonConvert.SerializeObject(report_sim_disconnect);
                                if (Properties.Settings.Default.reportRechargeOrderList == null)
                                {
                                    Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                                }
                                Properties.Settings.Default.reportRechargeOrderList.Add(res_report_sim_disconnect);
                                Properties.Settings.Default.Save();
                                this.instance.transactionUpdate();
                                return res;
                            }
                            if (DateTime.Now.Subtract(ussd_balance_timeout).TotalSeconds > 20 && retry_ussd)
                            {

                                try
                                {
                                    retry_ussd = false;
                                    this.resetUSSD(this.Port, this.rowGSMSelected);
                                    ussd_balance_timeout = DateTime.Now;
                                }
                                catch (Exception er)
                                {
                                    LoggerManager.LogError(er.Message);
                                }
                            }
                            else
                            {
                                await Task.Delay(100);
                            }
                        }
                        after_balance = this.ussd_balance;
                    }
                    double before_balance_value=0;
                    double after_balance_value=0;
                    try
                    {
                        before_balance_value = double.Parse(this.instance.balance_standard(before_balance));
                    }
                    catch(Exception er)
                    {
                        LoggerManager.LogError("before_value_parse" + er.Message);
                    }
                    if (!string.IsNullOrEmpty(after_balance))
                    {
                        try
                        {
                            after_balance_value = double.Parse(this.instance.balance_standard(after_balance));

                        }
                        catch (Exception er)
                        {
                            LoggerManager.LogError("after_value_parse" + er.Message);
                            report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"Không xác định được trạng thái nạp.\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"\",\"balance\":\"\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:\",\"card_real_amount\":\"\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                            var report_json_object = JsonConvert.DeserializeObject(report);
                            string res_report = JsonConvert.SerializeObject(report_json_object);
                            if (Properties.Settings.Default.reportRechargeOrderList == null)
                            {
                                Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                            }
                            Properties.Settings.Default.reportRechargeOrderList.Add(res_report);
                            Properties.Settings.Default.Save();
                            this.instance.transactionUpdate();
                            string error_res = this.instance.pushSingleSimInfo(phone, 0);
                            this.login_instance.wsHelper.sendDataToServer(error_res);
                            return res;
                        }
                    }
                    double diff_value = 0;
                    if(this.SimType=="TS")
                    {
                        if (before_balance_value < 0)
                        {
                            diff_value = Math.Abs(after_balance_value - before_balance_value);
                        }
                        else
                        {
                            diff_value = Math.Abs(before_balance_value - after_balance_value);
                        }
                    }
                    else
                    {
                        diff_value = after_balance_value - before_balance_value;
                    }
                    balance = after_balance_value.ToString();
                    if (diff_value > 0)
                    {   //case nap thanh cong khi khong co ussd message tra ve
                        res = 1;
                        info_log = "Thành công:" + info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                        LoggerManager.LogInfo(info_log);
                        this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                        {

                            this.rowGSMSelected.Cells["status"].Style.BackColor = Color.GreenYellow;

                        }));
                        this.Status = "Thành công";
                        this.Message = "Nạp thẻ thành công.";
                        string card_amount = info.Payload.Card_Amount;
                        if (!string.IsNullOrEmpty(this.Charge) && !string.IsNullOrEmpty(this.Charged))
                        {
                            this.Charged = (int.Parse(this.Charged.Replace(",", "").Replace(".", "")) + int.Parse(card_amount.Replace(",", "").Replace(".", ""))).ToString();
                        }
                        card_real_amount = diff_value.ToString();
                        datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                        try
                        {
                            reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"Bạn đã nạp {card_real_amount} VND vào TKC.\",\"network\":\"{this.Telco}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                            var json_object = JsonConvert.DeserializeObject(reply);
                            string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                            LoggerManager.LogTrace(res_json);
                            login_instance.wsHelper.sendDataToServer(res_json);
                        }
                        catch (Exception er)
                        {
                            LoggerManager.LogError("update_recharge_order:" + er.Message);
                        }
                        report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"Nạp thẻ thành công.\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                        var report_json_object = JsonConvert.DeserializeObject(report);
                        string res_report = JsonConvert.SerializeObject(report_json_object);
                        if (Properties.Settings.Default.reportRechargeOrderList == null)
                        {
                            Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                        }
                        Properties.Settings.Default.reportRechargeOrderList.Add(res_report);
                        Properties.Settings.Default.Save();
                        this.instance.transactionUpdate();
                    }
                    else
                    {   //case nap that bai khi khong co ussd message tra ve
                        this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                        {
                            this.rowGSMSelected.Cells["status"].Style.BackColor = Color.IndianRed;
                        }));
                        res = 0;
                        info_log = "Thất bại:" + info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                        LoggerManager.LogInfo(info_log);
                        this.Status = "Thất bại";
                        string error_msg = "Có lỗi xảy ra khi nạp thẻ";
                        this.Message = this.loadMsg(error_msg);
                        after_balance = this.ussd_balance;
                        card_real_amount = "0";
                        datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                        try
                        {
                            reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{error_msg}\",\"network\":\"{this.Telco}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                            var json_object = JsonConvert.DeserializeObject(reply);
                            string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                            LoggerManager.LogTrace(res_json);
                            login_instance.wsHelper.sendDataToServer(res_json);
                        }
                        catch (Exception er)
                        {
                            LoggerManager.LogError("update_recharge_order:" + er.Message);
                        }
                        report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{error_msg}\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                        var report_json_object = JsonConvert.DeserializeObject(report);
                        string res_report = JsonConvert.SerializeObject(report_json_object);
                        if (Properties.Settings.Default.reportRechargeOrderList == null)
                        {
                            Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                        }
                        Properties.Settings.Default.reportRechargeOrderList.Add(res_report);
                        Properties.Settings.Default.Save();
                        this.instance.transactionUpdate();
                    }
                }
            }
            catch (Exception er)
            {
                LoggerManager.LogError("order_handling:" + er.Message);
            }
            finally
            {
                orderHandlingSlim.Release();
            }
            return res;
        }

        //ham nap the cao
        public async void runTopUp(string code)
        {
            try
            {   
                string ussd_code = $"*100*{code}#";
                await this.runUSSD(ussd_code);
                isTopUp = true;
                lock_pin = true;
            }
            catch(Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
        }
        public async Task<bool> SendSms(string phone, string msg)
        {
            bool res = false;
            await this.lockWritePort.WaitAsync();
            try
            {
                try
                {
                    this.isFailSendSMS = true;
                    this.isreadport = true;
                    this.sp.Write("AT+CMGF=1" + Environment.NewLine);
                    this.sp.Write("AT+CMGS=\"" + phone + "\"\n");
                    this.sp.Write(msg);
                    this.sp.Write(new byte[1] { 26 }, 0, 1);
                    for (int timeWait = 0; timeWait < 25000 && this.isreadport; timeWait += 100)
                    {
                        await Task.Delay(100);
                    }
                    if (!this.isreadport && !isFailSendSMS)
                    {
                        res = true;
                    }
                }
                catch (Exception er)
                {
                    this.instance.loadData(er.Message);
                    LoggerManager.LogError(er.Message);
                }
                this.isreadport = false;
                phone = null;
                msg = null;
            }
            finally
            {
                this.lockWritePort.Release();
            }
            return res;
        }
        public void ReadSmsInbox()
        {
            Task.Run(async () =>
            {
                try
                {
                    await this.sendAT("AT+CMGF=0");
                    await this.sendAT("AT+CMGR=1");
                }
                catch (Exception ex)
                {
                    this.instance.loadData(ex.Message);
                    LoggerManager.LogError(ex.Message);

                }
            });
        }
        public void deleteMessageByIndex()
        {
            Task.Run(async () =>
            {
                try
                {
                    await this.sendAT(string.Format("AT+CMGD={0}\r", this.oldindexsms));
                }
                catch (Exception e)
                {
                    this.instance.loadData(e.Message);
                    LoggerManager.LogError(e.Message);
                }
            });
        }
        //ham xu ly event
        public void HandleSerialData(string input)
        {
            try
            {
                this.lastReportSerialData = DateTime.Now;
                if (input.Contains("+CMGS: ") && this.isreadport && this.isFailSendSMS)
                {
                    this.isreadport = false;
                    this.isFailSendSMS = false;
                }
                else if (input.Contains("CMS ERROR") && this.isreadport && this.isFailSendSMS)
                {
                    this.isreadport = false;
                    this.isFailSendSMS = true;
                }
                if (input.Contains("+CPIN: READY") && !this._isSim)
                {
                    this.Status = "Sẵn sàng";
                    this._isSim = true;
                    this.checkSimHasResult = true;
                    this.Message = loadMsg("Sim đã sẵn sàng");
                }
                else if (input.Contains("+CPIN: READY") && this._isSim)
                {
                    this.timeOutExit = DateTime.Now;
                }
                if (input.Contains("+CPIN: NOT READY") || (input.Contains("+CME ERROR: 10") && !input.Contains("+CME ERROR: 100")))
                {
                    this._isSim = true;
                    this.checkSimHasResult = false;
                    this.CarrierHasResult = false;
                    this.lastReportInfo = DateTime.MinValue;
                    this.lastReportNetwork = DateTime.MinValue;
                    this.lastReportPhone = DateTime.MinValue;
                    this.Phone = "";
                    this.Telco = "";
                    this.Expire = "";
                    this.Status = "No Sim";
                    this.Message = loadMsg("Không nhận dạng được SIM");
                    this.TKC = "";
                    this.TKKM = "";
                    this.sp.Close();
                    this.sp.Open();
                }
                if (input.Contains("+COPS:"))
                {
                    string str = input.Replace(" ", "").Replace("\n\r", "");
                    this.Telco = !str.ToUpper().Contains("VIETTEL") ? (!str.ToUpper().Contains("MOBIFONE") ? (!str.ToUpper().Contains("VINAPHONE") ? (!str.ToUpper().Contains("VIETNAMOBILE") ? "Sim tạm thời bị khóa hoặc không có sóng" : "VIETNAMOBILE") : "VINAPHONE") : "MOBIFONE") : "VIETTEL";
                    if (this.Telco == "Sim tạm thời bị khóa hoặc không có sóng")
                    {
                        this.Message = loadMsg("Không đọc được nhà mạng của Sim");
                        this.Status = "No Carrier";
                    }
                }
                if (input.Contains("+CUSD:"))
                {
                    if (this._isPhone)
                    {
                        this._isPhone = false;
                        if (this.Telco == "VIETTEL")
                        {
                            string phone_reg = input.Replace(" ", "").Replace("\n", "").Replace("\r", "");
                            string[] phone_split = phone_reg.Split(',');
                            phone_split = phone_split.Where(x => !string.IsNullOrWhiteSpace(x) && !string.IsNullOrEmpty(x)).ToArray();
                            string text_clone = "\"YeucaucuaQuykhachkhongduocdapungtaithoidiemnay\"";
                            if (phone_split[1] == text_clone)
                            {   
                                this.text = "\"YeucaucuaQuykhachkhongduocdapungtaithoidiemnay\"";
                                this.SimType = "TS";
                            }
                            if (check_11_dig)
                            {   
                                Regex reg = new Regex("[^0-9]");
                                string[] newip = phone_split[1].Split(':');
                                string phone_eleven = reg.Replace(newip[0], "");
                                this.Phone = phone_eleven;
                            }
                            else
                            {
                                Match match = new Regex(".*?" + char.ConvertFromUtf32(34) + "(\\d+).*?:([0-9\\.]+)d[a-zA-Z0?,:]+(\\d+\\/\\d+\\/\\d+).*").Match(phone_reg);
                                this.Phone = "0" + match.Groups[1].ToString().Substring(2);
                                if (phone_split[1].Contains("."))
                                {
                                    string[] tkc = phone_split[1].Split('.');
                                    Regex reg_digit = new Regex(@"(\d+|\d+\.\d+)d");
                                    Match match_digit = reg_digit.Match(tkc[1]);
                                    if (match_digit.Success)
                                    {
                                        this.TKC = match_digit.Value.Replace("d", "");
                                    }
                                }
                                else
                                {
                                    Regex reg_digit = new Regex(@"(\d+|\d+\. \d+)d");
                                    Match match_digit = reg_digit.Match(phone_split[3]);
                                    if (match_digit.Success)
                                    {
                                        this.TKC = match_digit.Value.Replace("d", "");
                                    }
                                }
                                this.Expire = match.Groups[3].ToString();
                                this.SimType = "TT";
                                this.ussd_balance = this.TKC;

                            }
                            if(!this.instance.phone_list.Contains(this.Phone))
                            {
                                this.instance.phone_list.Add(this.Phone);
                            }
                        }
                        else if (this.Telco == "VINAPHONE")
                        {   if(input.Contains("+CME"))
                            {
                                is_wintel = true;
                                return;
                            }
                            this.Phone = "0" + new Regex(".*(\\d{9}).*").Match(input.Replace(" ", "").Replace("\r\n", "").Replace("\n", "")).Groups[1].ToString();
                            phone_secondary_temp = this.Phone;
                            this.TKC = "";
                            this.Expire = "";
                            if (!this.instance.phone_list.Contains(this.Phone))
                            {
                                this.instance.phone_list.Add(this.Phone);
                            }
                        }
                        else if (this.Telco == "MOBIFONE")
                        {
                            string[] phones = input.Split(',');
                            Regex reg_phone = new Regex(@"\b84\d+\b");
                            string phone_value = phones[1].Replace("\"", "");
                            Match phone_match = reg_phone.Match(phone_value);
                            string phone = "";
                            if (phone_match.Success)
                            {
                                string phone_temp_value = phone_match.Value;
                                phone = "0" + phone_temp_value.Substring(2);
                            }
                            if (!string.IsNullOrEmpty(phone))
                            {
                                this.Phone = phone;
                            }
                           
                            phone_secondary_temp = this.Phone;
                            this.TKC = "";
                            this.Expire = "";
                            if (!this.instance.phone_list.Contains(this.Phone))
                            {
                                this.instance.phone_list.Add(this.Phone);
                            }
                        }
                        else if (this.Telco == "VIETNAMOBILE")
                        {
                            this.Phone = "0" + new Regex(".*(\\d{11}).*").Match(input.Replace(" ", "").Replace("\r\n", "").Replace("\n", "")).Groups[1].ToString().Substring(2);
                            phone_secondary_temp = this.Phone;
                            this.TKC = "";
                            this.Expire = "";
                            if (!this.instance.phone_list.Contains(this.Phone))
                            {
                                this.instance.phone_list.Add(this.Phone);
                            }
                        }
                        if (this.Phone != "" && this.Phone != "Loading" && (this.Phone.Length == 10 || this.Phone.Length == 11))
                        {
                            this.CarrierHasResult = true;
                            if (this.is_updateTKC == false)
                            {
                                this.Message = loadMsg(this.Port + " đã nhận dạng thành công: SĐT:" + this.Phone + " Network:" + this.Telco);
                            }
                            else if (this.is_updateTKC == true && this.Telco.Equals("VIETTEL"))
                            {
                                this.is_updateTKC = false;
                            }
                        }
                        else
                        {
                            this.Phone = "Loading";
                            this.Message = loadMsg(this.Port + " không nhận dạng được số điện thoại.");
                        }
                    }
                  
                    if (this._isInfor)
                    {
                        this._isInfor = false;
                        if (this.Telco == "MOBIFONE")
                        {   
                            if (!input.Contains("TKC") && this.first_request_sim_type!="TT")
                            {
                                this.SimType = "TS";
                                this.TKC = "null";
                                this.Expire = "null";
                            }
                            else
                            {
                                Match match = new Regex("TKC (\\d+) d.* ([\\d+\\/\\d+\\/\\d+]+)").Match(input.Replace("\r\n", "").Replace("\n", "").Replace("-", "/"));
                                this.TKC = match.Groups[1].ToString().Replace("₫", "");
                                this.Expire = match.Groups[2].ToString();
                                this.ussd_balance = this.TKC;
                                if (!string.IsNullOrEmpty(this.TKC))
                                {
                                    this.SimType = "TT";
                                    this.first_request_sim_type = "TT";
                                }
                            }
                        }
                        else if (this.Telco == "VINAPHONE")
                        {
                            if (input.Contains("Request not completed") || input.Contains("+CME ERROR: 100"))
                            {
                                this.SimType = "TS";
                            }
                            else
                            {
                              
                                Match match = new Regex("=(\\d+) VND.* ([\\d+\\/\\d+\\/\\d+]+)").Match(input.Replace("\r\n", "").Replace("\n", ""));
                                this.TKC = match.Groups[1].ToString();
                                string date = match.Groups[0].ToString();
                                string[] dates = date.Split('.');
                                string[] expiration_date = dates[0].Split(',');
                                string expire = expiration_date[1].Replace("HSD", "");
                                this.Expire = expire;
                                this.SimType = "TT";
                                ussd_balance = this.TKC;
                            }
                        }
                        else if (this.Telco == "VIETTEL")
                        {
                            string input_refact = input.Replace(" ", "").Replace("\r\n", "").Replace("\n", "");
                            Match match = new Regex(".*?" + char.ConvertFromUtf32(34) + "(\\d+).*?:([0-9\\.]+)d[a-zA-Z0?,:]+(\\d+\\/\\d+\\/\\d+).*").Match(input_refact);
                            this.Phone = "0" + match.Groups[1].ToString().Substring(2);
                            this.TKC = match.Groups[2].ToString();
                            this.Expire = match.Groups[3].ToString();
                            this.ussd_balance = this.TKC;
                            if (is_updateTKC == true)
                            {
                                is_updateTKC = false;
                                updateTK = this.TKC;
                            }
                        }
                        else if(this.Telco == "VIETNAMOBILE")
                        {
                          
                            string modified_str = input.Replace("\n", " ");
                            string[] split_values = modified_str.Split(' ');
                            string tkc = "";
                            foreach(string value in split_values)
                            {   
                                if(value.Contains("d"))
                                {
                                    tkc = value;
                                    break;
                                }
                            }
                            Regex reg = new Regex("[^0-9.]");
                            tkc = reg.Replace(tkc, "");
                            this.TKC = tkc;
                        }
                    }
                    if (this.isTKKM)
                    {
                        this.isTKKM = false;
                        Regex reg = new Regex(@"(\d+|\d+\.\d+)d");
                        Match match = reg.Match(input);
                        if (match.Success)
                        {
                            string tkkm = match.Value.Replace("d", "");
                            this.TKKM = tkkm;
                        }
                    }
                
                    if (this.isCheckCharge)
                    {
                        if (input.Contains("CME") || input.Contains("ERROR"))
                        {
                            return;
                        }
                        this.isCheckCharge = false;
                        Regex reg = new Regex(@"no truoc [-\d,]+VND");
                        Match match_charge = reg.Match(input);
                        if(match_charge.Success)
                        {
                            string temp_charge_value = match_charge.Value;
                            Regex reg_digit = new Regex(@"[^-\d,]");
                            checkChargeValue = reg_digit.Replace(temp_charge_value, "");
                            this.ussd_charge = checkChargeValue;
                        }
                    }
                    
                    if (isTopUp)
                    {
                        isTopUp = false;
                        string[] value = input.Split(',');
                        string mess = "";
                        if (value[1] == "\"Xin loi")
                        {
                            mess = value[1] + ","+ value[2];
                        }
                        else
                        {
                            mess = value[1];
                        }
                        if (mess.Contains("khong hop le") || mess.Contains("bi khoa") || (mess.Contains("KM") && mess.Contains("CT")) || mess.Contains("khong dung") || mess.Contains("He thong se gui tin nhan") || mess.Contains("The nap da duoc su dung") || mess.Contains("da nhap sai") || mess.Contains("Xin loi") || mess.Contains("Dich vu nay"))
                        {
                            is_Topup = false;
                            if(mess.Contains("KM")&&mess.Contains("CT"))
                            {
                                topupResult = "Mã thẻ không hợp lệ hoặc tài khoản quý khách đã được sử dụng.";
                            }
                            else
                            {
                                topupResult = mess.Replace("\"","");
                            }
                        }
                        else if (mess.Contains("Tai khoan cua Quy khach") || mess.Contains("Nap the thanh cong") || mess.Contains("Ban da nap") ||(mess.Contains("Yeu cau nap tien") && mess.Contains("thanh cong")))
                        {
                            is_Topup = true;
                            topupResult = mess.Replace("\"","");
                        }
                        lock_pin = false;
                    }
                }
                if (input.Contains("+CMGR: "))
                {
                    this.current_sms = input;
                    this.smsSuccess = false;
                    if (this.current_sms.Replace("\n", "").Replace("\r", "").EndsWith("OK"))
                    {
                        input = this.current_sms;
                        this.current_sms = string.Empty;
                        this.smsSuccess = true;
                    }
                }
                else if (!string.IsNullOrEmpty(this.current_sms) && !this.smsSuccess)
                {
                    this.current_sms += input;
                    if (input.Replace("\n", "").Replace("\r", "").EndsWith("OK"))
                    {
                        input = this.current_sms;
                        this.current_sms = string.Empty;
                        this.smsSuccess = true;
                    }
                }
                if (input.Contains("+CMGR: ") && this.smsSuccess)
                {
                    this.deleteMessageByIndex();
                    int start_index_1 = input.IndexOf("+CMGR: ") + 7;
                    int start_index_2 = input.IndexOf("\r\n", start_index_1) + 2;
                    int num = input.IndexOf("\r\n", start_index_2 + 1);
                    if (start_index_2 == -1 || num == -1 || num <= start_index_2)
                    {
                        return;
                    }
                    string pdu = input.Substring(start_index_2, num - start_index_2);
                    string txt1 = string.Empty;
                    string txt2 = string.Empty;
                    string txt3 = string.Empty;
                    SmsDeliverPdu smsDeliverPdu = (SmsDeliverPdu)IncomingSmsPdu.Decode(pdu, true);
                    string otp = smsDeliverPdu.UserDataText;
                    if (otp.Contains("OTP"))
                    {
                        txt1 = smsDeliverPdu.UserDataText;
                        txt2 = smsDeliverPdu.OriginatingAddress;
                        txt3 = string.Format("{0:dd/MM/yyyy}", smsDeliverPdu.SCTimestamp.ToDateTime()) + " " + string.Format("{0:HH:mm:ss}", smsDeliverPdu.SCTimestamp.ToDateTime());
                        Regex regex = new Regex("^[^a-zA-Z0-9]*");
                        string txt1_1 = regex.Replace(txt1, "");
                        int first_place = this.Phone.IndexOf("0");
                        string phone_push = this.Phone;
                        if (first_place == 0)
                        {
                            phone_push = this.Phone.Remove(first_place, 1).Insert(first_place, "84");
                        }
                        string re = txt2 + "~" + txt1_1 + "~" + txt3 + "~" + phone_push;
                        string re_ver1 = txt2 + "@" + txt1_1 + "@" + txt3 + "@" + this.Phone;

                        return;
                    }
                    else
                    {
                        txt1 = smsDeliverPdu.UserDataText;
                        txt2 = smsDeliverPdu.OriginatingAddress;
                        txt3 = string.Format("{0:dd/MM/yyyy}", smsDeliverPdu.SCTimestamp.ToDateTime()) + " " + string.Format("{0:HH:mm:ss}", smsDeliverPdu.SCTimestamp.ToDateTime());
                        Regex regex = new Regex("^[^a-zA-Z0-9]*");
                        string txt1_1 = regex.Replace(txt1, "");
                        Regex phone_reg = new Regex("[^0-9]");
                        string test_phone = regex.Replace(this.Phone, "");
                        if (this.Phone.Length != test_phone.Length)
                        {
                            return;
                        }
                        if (txt2.Equals("123") || txt2.Equals("VIETTEL") || txt2.Equals("VIETTEL_DV") || txt2.Equals("MyViettel") || txt2.Equals("MTTQ") || txt2.Equals("VIETTELCSKH") || txt2.Equals("BO CONG AN") || txt2.Equals("211"))
                        {
                            return;
                        }
                        int first_place = this.Phone.IndexOf("0");
                        string phone_push = "";
                        if (first_place == 0)
                        {
                            phone_push = this.Phone.Remove(first_place, 1).Insert(first_place, "84");
                        }
                        if (txt2 == "+049233")
                        {
                            message_sim_type = txt1;
                            if(message_sim_type.Contains("chi cung cap cho thue bao tra sau"))
                            {  if (string.IsNullOrEmpty(this.SimType) || this.SimType == "TS")
                                {
                                    this.SimType = "TT";
                                    this.first_request_sim_type = "TT";
                                    this.TKC = "";
                                    this.Expire = "";
                                }
                            }
                            else if(message_sim_type.Contains("Xin vui long kiem tra lai"))
                            {
                                this.SimType = "TS";
                                this.TKC = "null";
                                this.Expire = "null";
                            }
                        }
                        else if(txt2 == "9223")
                        {
                            Regex regex_ussd = new Regex(@"la [\d,]+\s+VND");
                            Match match_value=regex_ussd.Match(txt1);
                            if(match_value.Success) 
                            {
                                Regex reg_card_amount = new Regex(@"[^\d,]");
                                after_card_real_amount = reg_card_amount.Replace(match_value.Value, "");
                            }
                        }
                        string re = txt2 + "@" + txt1_1 + "@" + txt3 + "@" + phone_push;
                        string re_ver1 = txt2 + "@" + txt1_1 + "@" + txt3 + "@" + this.Phone;
                    }
                }
                if (!input.Contains("+CMTI: \"SM\","))
                {
                    return;
                }
                this.oldindexsms = int.Parse((input.Split('\n')).FirstOrDefault((y => y.Contains("+CMTI: \"SM\","))).Replace("+CMTI: \"SM\",", ""));
                this.ReadSmsInbox();
            }
            catch (Exception er)
            {
                this.instance.loadData(er.Message);
            }
        }
      
        public async Task DeleteAllSmsInbox()
        {
            try
            {
                await this.sendAT("AT+CPMS=\"SM\",\"SM\",\"SM\"");
                await Task.Delay(100);
                await this.sendAT("AT+CMGF=1");
                await Task.Delay(100);
                await this.sendAT("AT+CMGD=1,4");
                await Task.Delay(100);
                await this.sendAT("AT+CNMI=1,1");
            }
            catch (Exception er)
            {
                this.loadMsg(er.Message);
            }
        }
        
        public Task Work()
        {

            this.delallsms = true;
            return Task.Run(async () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    if (this.rowGSMSelected == null)
                    {
                        this.Message = loadMsg("Port " + this.Port + " không thể kết nối");
                    }
                    else
                    {
                        this._isSim = false;
                        this.Status = "Try Open Port";
                        this.Message = this.Port + " đang mở cổng";
                        bool try_open_port = this.TryOpenPort();
                        if (!try_open_port)
                        {

                            this.Status = "Port Close";
                            this.Message = this.Port + " đã đóng";
                        }
                        else
                        {
                            while (true)
                            { if (lock_pin == false)
                                {
                                    try
                                    {
                                        if (!this._isSim)
                                        {
                                            await CheckSimReady();
                                            for (int msWait = 0; !this.checkSimHasResult && msWait < 3000; msWait += 100)
                                            {
                                                await Task.Delay(100);
                                            }
                                        }
                                        else
                                        {
                                            if (DateTime.Now.Subtract(this.timeOutExit).TotalSeconds > 5)
                                            {
                                                if(this.instance.phone_list.Contains(this.Phone))
                                                {   
                                                    this.instance.phone_list.Remove(this.Phone);
                                                    this.instance.temp_phone_list.Remove(this.Phone);
                                                    string res = this.instance.pushSingleSimInfo(this.Phone,0);
                                                    if (!string.IsNullOrEmpty(res))
                                                    {
                                                        this.login_instance.wsHelper.sendDataToServer(res);
                                                        LoggerManager.LogTrace(res);
                                                    }
                                                }
                                                this.Port = port;
                                                this.Telco = "";
                                                this.Phone = "";
                                                this.Status = "Try Open Port";
                                                this.TKC = "";
                                                this.Expire = "";
                                                this.Message = port + " đang mở cổng";
                                                this.SimType = "";
                                                this.TKKM = "";
                                                this.rowGSMSelected.Cells["status"].Style.BackColor = Color.White;
                                                this.Charge = "";
                                                this.Charged = "";
                                                this.isTopUp = false;
                                                this.topupResult = "";
                                                this.after_card_real_amount = "";
                                                this.checkChargeValue = "";
                                                this.first_request_sim_type = "";
                                                this.message_sim_type = "";
                                                this.delallsms = true;
                                                this.CarrierHasResult = false;
                                            }
                                            if (this.delallsms)
                                            {
                                                await this.DeleteAllSmsInbox();
                                                await Task.Delay(100);
                                                this.delallsms = false;
                                            }
                                                await this.sendAT("AT+CPIN?");
                                            
                                            await Task.Delay(100);

                                            if ((this.Telco == "" || this.Telco == "Sim tạm thời bị khóa hoặc không có sóng") && DateTime.Now.Subtract(this.lastReportNetwork).TotalSeconds > 5.0)
                                            {
                                                this.lastReportNetwork = DateTime.Now;
                                                await this.sendAT("AT+COPS?");
                                                await Task.Delay(100);
                                            }
                                            if (!this.CarrierHasResult && this.Telco != "" && this.Telco != "Sim tạm thời bị khóa hoặc không có sóng" && DateTime.Now.Subtract(this.lastReportPhone).TotalSeconds > 10)
                                            {  if (is_updateTKC == false)
                                                {
                                                    this.Status = "Sẵn sàng";
                                                    this.lastReportPhone = DateTime.Now;
                                                    this.Phone = "Loading";
                                                    this._isPhone = true;
                                                    this.Message = loadMsg(this.Port + " bắt đầu nhận dạng SĐT");
                                                }
                                                else
                                                {
                                                    
                                                    this.lastReportPhone = DateTime.Now;
                                                    this.Status = "Sẵn sàng";
                                                    this.Phone = "Loading";
                                                    this._isPhone = true;
                                                    this.TKC = "";
                                                }
                                                if (this.Telco == "VIETTEL" && !text.Contains("\"YeucaucuaQuykhachkhongduocdapungtaithoidiemnay\""))
                                                {
                                                    await this.runUSSD("*101#");
                                                }
                                                else if (this.Telco == "VIETTEL" && text.Contains("\"YeucaucuaQuykhachkhongduocdapungtaithoidiemnay\""))
                                                {
                                                    check_11_dig = true;
                                                    await this.runUSSD("*098#");
                                                }
                                                else if (this.Telco == "VINAPHONE")
                                                { 
                                                    await this.runUSSD("*110#");
                                                }
                                                else if (this.Telco == "MOBIFONE")
                                                {
                                                    await this.runUSSD("*0#");
                                                }
                                                else if (this.Telco == "VIETNAMOBILE")
                                                {
                                                    await this.runUSSD("*123#");
                                                }
                                                for (int wait = 0; !this.CarrierHasResult && wait < 1000; wait += 100)
                                                {
                                                    await Task.Delay(100);
                                                }
                                            }
                                            await Task.Delay(500);
                                            if(string.IsNullOrEmpty(this.SimType) && this.Telco=="MOBIFONE" && DateTime.Now.Subtract(this.lastSendSmS).TotalSeconds>60)
                                            {
                                                this.lastSendSmS = DateTime.Now;
                                                bool val = await this.SendSms("9233", "TC");
                                                this.timeOutExit = DateTime.Now;
                                            }
                                            await Task.Delay(500);
                                            if ((this.TKC == "" || this.TKC == "Lấy lại TKC" || this.Expire == "") && this.Telco != "" && this.Telco != "Sim tạm thời bị khóa hoặc không có sóng" && DateTime.Now.Subtract(this.lastReportInfo).TotalSeconds > 10 && this.Phone != "" && this.Phone != "Loading")
                                                {
                                                    this.lastReportInfo = DateTime.Now;
                                                    this._isInfor = true;
                                                    if (this.Telco == "MOBIFONE" || this.Telco == "VINAPHONE" || this.Telco == "VIETNAMOBILE" || this.Telco == "VIETTEL")
                                                    {
                                                        await this.runUSSD("*101#");
                                                    }
                                                    await Task.Delay(100);
                                                }
                                                if (!string.IsNullOrEmpty(this.TKC) && !string.IsNullOrEmpty(this.Expire) && !string.IsNullOrEmpty(this.Telco) && !string.IsNullOrEmpty(this.Phone) && this.Phone != "Loading" && DateTime.Now.Subtract(this.lastTKKMReport).TotalSeconds > 10)
                                                {

                                                    if (this.Telco == "VIETTEL")
                                                    {
                                                        this.lastTKKMReport = DateTime.Now;
                                                        this.isTKKM = true;
                                                        await this.runUSSD("*102#");
                                                    }
                                                    await Task.Delay(100);
                                                }
                                                await Task.Delay(100);
                                                if (!string.IsNullOrEmpty(this.Telco) && !string.IsNullOrEmpty(this.Phone) && string.IsNullOrEmpty(this.checkChargeValue) && this.Phone != "Loading" && this.SimType=="TS" && DateTime.Now.Subtract(this.lastRunCharge).TotalSeconds > 10)
                                                {
                                                    this.lastRunCharge = DateTime.Now;
                                                    this.isCheckCharge = true;
                                                    await this.runUSSD("*112#");
                                                }
                                            await Task.Delay(100);
                                            }
                                        
                                    }
                                    catch (Exception e)
                                    {
                                        this.instance.loadData("gsm"+e.Message);
                                    }
                                }
                                else
                                {
                                    timeOutExit = DateTime.Now;
                                }
                            }
                        }
                    }
                }
            });
        }

    }
}

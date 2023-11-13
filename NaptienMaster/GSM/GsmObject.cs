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
using System.Net.Http;
using NaptienMaster.Services;
using NaptienMaster.ResponseItem;

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
        private string otp;
        private string regis_status;
        private bool _isSim;
        public bool _isInfor;
        private bool isTKKM;
        public bool isRecheckInfo;
        private string modem;
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
        public string serial_card_lock = "";
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
        public int lock_sim_status=1;
        private string ussd_lock_sim_status="";
        private DateTime timeOutExit = DateTime.MinValue;
        private string data;
        private bool is_send_sms = true;
        private string current_sms;
        private bool smsSuccess;
        private bool is_modem = false;
        public string updateTK = "";
        private CancellationTokenSource cts;
        public string ussd_balance;
        public string ussd_charge;
        public string topupResult = "";
        public string checkChargeValue = "";
        public string message_sim_type = "";
        public string after_card_real_amount = "";
        public string after_card_sms = "";
        public DateTime sms_response_time;
        public bool test_ussd = false;
        public bool is_lock_sim = false;
        public string first_request_sim_type = "";
        public string reset_lock_sim_fail = "";
        public string prepaid_topup = "";
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
            if (this.login_instance.language == "English")
            {
                this.Status = loadMsg("Waiting");
                this.Message = loadMsg("Waiting for SIM card");

            }
            else if (this.login_instance.language == "中文")
            {
                this.Status = loadMsg("等待");
                this.Message = loadMsg("等待SIM卡");

            }
            else
            {
                this.Status = "Waiting";
                this.Message = loadMsg("Đang chờ nhận Sim");

            }
            this.TKC = "";
            this.SimType = "";
            this.Otp = "";
            this.Regis_Status = "";
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
            if (this.login_instance.language == "English")
            {
                this.Status = loadMsg("Waiting");
                this.Message = loadMsg("Getting Sim Card Info");
            }
            else if (this.login_instance.language == "中文")
            {
                this.Status = loadMsg("等待");
                this.Message = loadMsg("获取SIM卡消息");
            }
            else
            {
                this.Status = "Waiting";
                this.Message = loadMsg("Đang lấy thông tin");
            }
            this.TKC = "";
            this.Expire = "";
            this.Otp = "";
            this.Regis_Status = "";
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

        public string Otp
        {
            get => this.otp;
            set
            {
                this.otp = value;
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
        
        public string Regis_Status
        {
            get => this.regis_status;
            set
            {
                this.regis_status = value;
                UpdateGUI.ChangeRow(rowGSMSelected,"regis_status", value);
            }
        }

        public string Modem
        {
            get => this.modem;
            set
            {
                this.modem = value;
                UpdateGUI.ChangeRow(rowGSMSelected, "modem", value);
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
                await this.sendAT($"AT+CUSD=1,\"{command}\",15");             
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
            if (this.login_instance.language == "English")
            {
                this.Status = loadMsg("Waiting");
            }
            else if (this.login_instance.language == "中文")
            {
                this.Status = loadMsg("等待");
            }
            else
            {
                this.Status = loadMsg("Waiting");
            }
            this.TKC = "";
            this.Expire = "";
            this.SimType = "";
            if (this.login_instance.language == "English")
            {
                this.Message = loadMsg("Getting Sim Card Info");
            }
            else if (this.login_instance.language == "中文")
            {
                this.Message = loadMsg("获取SIM卡消息");
            }
            else
            {
                this.Message = loadMsg("Đang lấy thông tin");
            }
            this.TKKM = "";
            this.Charge = "";
            this.Charged = "";
            this.Otp = "";
            this.Regis_Status = "";
            this.isTopUp = false;
            this.topupResult = "";
            this.message_sim_type = "";
            this.after_card_real_amount = "";
            this.checkChargeValue = "";
            this.CarrierHasResult = false;
            this.first_request_sim_type = "";
            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
            {
                this.rowGSMSelected.Cells["status"].Style.BackColor = Color.White;
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
        //ham tao myMobifone
        public async Task<bool> registerMyMobifone(string phone, int carrier)
        {

            try
            {
                int receive_time = 0;
                int send_otp = 0;
                int otp_receive_time = 0;
                bool stop_receive = false;
                DateTime time_out = DateTime.Now;
                this.Otp = "";
                string token = Environment.GetEnvironmentVariable("MY_TOKEN");
                string network = this.Telco;
                if (this.login_instance.language == "English")
                {
                    this.Regis_Status = "Initializing";
                }
                else if (this.login_instance.language == "中文")
                {
                    this.Regis_Status = "启动中";
                }
                else
                {
                    this.Regis_Status = "Đang khởi tạo";
                }
                this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                {
                    {
                        this.rowGSMSelected.Cells["regis_status"].Style.BackColor = Color.CadetBlue;
                    }
                }));
                string password ="";
                RegisterMVP regis = new RegisterMVP(phone, password, carrier);
                int first_place = regis.phone.IndexOf("0");
                string phone_push = regis.phone.Remove(first_place, 1).Insert(first_place, "84");
                var input_data = new Dictionary<string, string>()
            {
                { "phone", phone_push},
                { "password", regis.password },
                { "carrier", regis.carrier.ToString()},
            };
                var otpVal = await ApiServices.registerMyMobiFone<ResponseMVTRegister>("create-transaction", new FormUrlEncodedContent(input_data), token);
                await Task.Delay(1000);
                try
                {
                    if (otpVal != null)
                    {
                        if (!string.IsNullOrEmpty(otpVal.Message) && otpVal.Message == "Tạo giao dịch thành công")
                        {
                            DateTime port_now = DateTime.Now;
                        loop:

                            if (DateTime.Now.Subtract(time_out).TotalMinutes > 13 && otp_receive_time > 0)
                            {
                                if (this.login_instance.language == "English")
                                {
                                    this.Regis_Status = "Failed";
                                }
                                else if (this.login_instance.language == "中文")
                                {
                                    this.Regis_Status = "失败";
                                }
                                else
                                {
                                    this.Regis_Status = "Thất bại";
                                }
                                this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                {
                                    this.rowGSMSelected.Cells["regis_status"].Style.BackColor = Color.IndianRed;
                                }));
                                otpVal = null;
                                return false;
                            }
                            var transaction_id = otpVal.Data.TranSactionId;
                            var list_transaction_id = new Dictionary<string, string>
                            { { "list_transaction_id[0]",transaction_id} };
                           var status = await ApiServices.registerMyMobiFone<ResponseStatusRegister>("list-transaction", new FormUrlEncodedContent(list_transaction_id), token);
                            if (status != null)
                            {
                                string status_id = status.Data[0].Status;
                                if (status_id.Equals("3"))
                                {
                                    try
                                    {
                                        if (this.login_instance.language == "English")
                                        {
                                            this.Regis_Status = "Failed";
                                        }
                                        else if (this.login_instance.language == "中文")
                                        {
                                            this.Regis_Status = "失败";
                                        }
                                        else
                                        {
                                            this.Regis_Status = "Thất bại";
                                        }
                                        this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                        {
                                            this.rowGSMSelected.Cells["regis_status"].Style.BackColor = Color.IndianRed;
                                        }));
                                    }
                                    catch (Exception er)
                                    {
                                        Console.WriteLine(er.Message);
                                    }
                                    send_otp = 1;
                                    otpVal = null;
                                    status = null;
                                    return false;
                                }

                                else if (status_id.Equals("2"))
                                {
                                    try
                                    {
                                        if (this.login_instance.language == "English")
                                        {
                                            this.Regis_Status = "Login MyMobi successfully";
                                        }
                                        else if (this.login_instance.language == "中文")
                                        {
                                            this.Regis_Status = "成功";
                                        }
                                        else
                                        {
                                            this.Regis_Status = "Login thành công";
                                        }
                                        this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                        {
                                            this.rowGSMSelected.Cells["regis_status"].Style.BackColor = Color.LightGreen;
                                        }));
                                    }
                                    catch (Exception er)
                                    {
                                        Console.WriteLine(er.Message);
                                    }
                                    send_otp = 1;
                                    status = null;
                                    otpVal = null;
                                    return true;
                                }
                            }
                            if (send_otp == 0)
                            {
                                if (!string.IsNullOrEmpty(this.Otp))
                                {
                                    string sms_received = this.Otp;
                                    string[] val = sms_received.Split('~');
                                    string from = val[0];
                                    string content = val[1];
                                    string telco_received_at = val[2];
                                    if (!content.Contains("OTP"))
                                    {
                                        if (this.login_instance.language == "English")
                                        {
                                            this.Regis_Status = "Failed";
                                        }
                                        else if (this.login_instance.language == "中文")
                                        {
                                            this.Regis_Status = "失败";
                                        }
                                        else
                                        {
                                            this.Regis_Status = "Thất bại";
                                        }
                                        this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                        {
                                            this.rowGSMSelected.Cells["regis_status"].Style.BackColor = Color.IndianRed;
                                        }));
                                        otpVal = null;
                                        status = null;
                                        return false;
                                    }
                                    PushSmS sms = new PushSmS(phone, from, content, telco_received_at);
                                    var push_data = new Dictionary<string, string>()
                                {
                                { "phone", sms.phone },
                                { "from", sms.from },
                                { "content", sms.content },
                                { "telco_received_at", sms.telco_received_at}
                                };
                                    ResponseSmSPush smsrep = await ApiServices.registerMyMobiFone<ResponseSmSPush>("add-sms", new FormUrlEncodedContent(push_data), token);
                                    if (smsrep != null)
                                    {
                                        otp_receive_time++;
                                        if (smsrep.Message.Equals("Nhận thành công"))
                                        {
                                            this.Otp = "";
                                        }
                                        else
                                        {
                                            send_otp = 1;
                                            this.Regis_Status = "Thất bại";
                                            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                            {
                                                this.rowGSMSelected.Cells["regis_status"].Style.BackColor = Color.IndianRed;
                                            }));
                                            otpVal = null;
                                            smsrep = null;
                                            return false;
                                        }
                                        smsrep = null;
                                    }
                                }
                                else if (DateTime.Now.Subtract(port_now).TotalMinutes > 10 && otp_receive_time == 0)
                                {
                                    if (this.login_instance.language == "English")
                                    {
                                        this.Regis_Status = "Failed";
                                    }
                                    else if (this.login_instance.language == "中文")
                                    {
                                        this.Regis_Status = "失败";
                                    }
                                    else
                                    {
                                        this.Regis_Status = "Thất bại";
                                    }
                                    this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                    {
                                        this.rowGSMSelected.Cells["regis_status"].Style.BackColor = Color.IndianRed;
                                    }));
                                    send_otp = 1;
                                    otpVal = null;
                                    return false;
                                }
                            }
                            await Task.Delay(5000);
                            goto loop;
                        }
                        else if (otpVal.Success == "false")
                        {
                            if (this.login_instance.language == "English")
                            {
                                this.Regis_Status = "Failed";
                            }
                            else if (this.login_instance.language == "中文")
                            {
                                this.Regis_Status = "失败";
                            }
                            else
                            {
                                this.Regis_Status = "Thất bại";
                            }
                            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                            {
                                this.rowGSMSelected.Cells["regis_status"].Style.BackColor = Color.IndianRed;
                            }));
                            otpVal = null;
                            return false;
                        }
                    }
                    else
                    {
                        if (this.login_instance.language == "English")
                        {
                            this.Regis_Status = "Initialize failed";
                        }
                        else if (this.login_instance.language == "中文")
                        {
                            this.Regis_Status = "启动失败";
                        }
                        else
                        {
                            this.Regis_Status = "Khởi tạo thất bại";
                        }
                        this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                        {
                            this.rowGSMSelected.Cells["regis_status"].Style.BackColor = Color.IndianRed;
                        }));
                        return false;
                    }
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
            }
            return true;
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
                Regex regex_after_balance = new Regex(@"hien tai [\d,]+");
                DateTime ussd_balance_timeout;
                Guid guid = Guid.NewGuid();
                if (this.Status == "Đang xử lý" || this.Status=="Processing" || this.Status=="处理中")
                {
                    return res;
                }
                this.serial_card_lock = info.Payload.Card_Serial;
                is_lock_sim = false;
                this.after_card_real_amount = "";
                this.after_card_sms = "";
                prepaid_topup = "";
                topupResult = "";
                this.lock_sim_status = 1;
                string info_log = info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                    LoggerManager.LogInfo(info_log);
                    DateTime datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                    DateTime trace_time_vietnam= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
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
                //neu before_balance la null thi dung va huy nap
                 if(string.IsNullOrEmpty(before_balance) || string.IsNullOrEmpty(this.Modem))
                {
                    res = -1;
                    if (string.IsNullOrEmpty(before_balance) && !string.IsNullOrEmpty(this.Modem))
                    {
                        reply_push_recharge = $"{{\"command\":\"PUSH_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":false,\"message\":\"Sim bị lỗi không hiện tài khoản chính.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                    }
                    else if(string.IsNullOrEmpty(this.Modem) && !string.IsNullOrEmpty(before_balance))
                    {
                        reply_push_recharge = $"{{\"command\":\"PUSH_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":false,\"message\":\"Sim bị lỗi không hiện thông tin tài khoản chính.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                    }
                    else
                    {
                        reply_push_recharge = $"{{\"command\":\"PUSH_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":false,\"message\":\"Sim bị lỗi không hiện thông tin loại modem và tài khoản chính.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                    }
                    json_object_push_recharge = JsonConvert.DeserializeObject(reply_push_recharge);
                    res_json_push_recharge = JsonConvert.SerializeObject(json_object_push_recharge, Formatting.Indented);
                    login_instance.wsHelper.sendDataToServer(res_json_push_recharge);
                    string error_res = this.instance.pushSingleSimInfo(phone, 0);
                    if (!string.IsNullOrEmpty(error_res))
                    {
                        this.login_instance.wsHelper.sendDataToServer(error_res);
                        LoggerManager.LogTrace("Lỗi không nhận được balance:" + error_res);
                    }
                    trace_time_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                    report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Sim bị lỗi không nhận được tài khoản chính.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{network}\",\"sim_type\":\"{sim_type}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tổng cước nợ của bạn là:{before_balance}\",\"after_balance_ussd\":\"Tổng cước nợ của bạn là:{after_balance}\",\"sms_time\":\"\",\"card_real_amount\":\"0\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
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
                if (this.login_instance.language == "English")
                {
                    this.Message = this.loadMsg("Processing");
                    this.Status = "Processing";
                }
                else if (this.login_instance.language == "中文")
                {
                    this.Message = this.loadMsg("处理中");
                    this.Status = "处理中";
                }
                else
                {
                    this.Message = this.loadMsg("Bắt đầu xử lý");
                    this.Status = "Đang xử lý";
                }
                this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                    {
                           this.rowGSMSelected.Cells["status"].Style.BackColor = Color.Aquamarine;    
                    }));
                datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                DateTime flush_topup_timeout = DateTime.Now;
                isTopUp = true;
                lock_pin = true;
                while(DateTime.Now.Subtract(flush_topup_timeout).TotalSeconds<15)
                {
                    await Task.Delay(100);
                }
                isTopUp = false;
                lock_pin = false;
                if(!string.IsNullOrEmpty(topupResult))
                {
                    topupResult = "";
                }
                await Task.Delay(1000);
                //Tien hanh nap the
                this.runTopUp(code);
                    while (DateTime.Now.Subtract(topUpResponse).TotalSeconds<60 && string.IsNullOrEmpty(this.topupResult))
                    {
                    await Task.Delay(100);
                    }
                if (!string.IsNullOrEmpty(this.topupResult))
                {
                    //case nap the that bai khi co ussd message tra ve
                    if(this.SimType == "TS")
                    {
                        info_log = "Thông tin nạp:" + info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                        LoggerManager.LogInfo(info_log);
                        res = 0;
                            if (string.IsNullOrEmpty(this.Telco))
                            {
                            report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Sim bị mất kết nối trong quá trình nạp.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{network}\",\"sim_type\":\"{sim_type}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tổng cước nợ của bạn là:{before_balance}\",\"after_balance_ussd\":\"Tổng cước nợ của bạn là:{after_balance}\",\"sms_time\":\"\",\"card_real_amount\":\"0\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
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
                       
                        if(this.is_Topup == false)
                        {
                            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                            {
                                this.rowGSMSelected.Cells["status"].Style.BackColor = Color.IndianRed;
                            }));
                            if (this.login_instance.language == "English")
                            {
                                this.Status = "Failed";
                            }
                            else if (this.login_instance.language == "中文")
                            {
                                this.Status = "失败";
                            }
                            else
                            {
                                this.Status = "Thất bại";
                            }
                            this.Message = this.loadMsg(this.topupResult);
                            after_balance = before_balance;
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
                                trace_time_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                                reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Cước nợ trước của bạn là:{before_balance}\",\"after_balance_ussd\":\"Cước nợ trước của bạn là:{after_balance}\",\"dd:MM:yyyy_hh:mm:ss\":\"\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{trace_time_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                var json_object = JsonConvert.DeserializeObject(reply);
                                string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                LoggerManager.LogTrace(res_json);
                                login_instance.wsHelper.sendDataToServer(res_json);
                            }
                            catch (Exception er)
                            {
                                LoggerManager.LogError("update_recharge_order:" + er.Message);
                            }
                            report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Cước nợ trước của bạn là:{before_balance}\",\"after_balance_ussd\":\"Cước nợ trước của bạn là:{after_balance}\",\"sms_time\":\"{this.sms_response_time}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";

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
                        {
                            res = 1;
                            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                            {
                                this.rowGSMSelected.Cells["status"].Style.BackColor = Color.GreenYellow;
                            }));
                            if (this.login_instance.language == "English")
                            {
                                this.Status = "Success";
                            }
                            else if (this.login_instance.language == "中文")
                            {
                                this.Status = "成功";
                            }
                            else
                            {
                                this.Status = "Thành công";
                            }
                            this.Message = this.loadMsg(this.topupResult);
                            string card_amount = info.Payload.Card_Amount;
                            if (!string.IsNullOrEmpty(this.Charge) && !string.IsNullOrEmpty(this.Charged))
                            {
                                this.Charged = (int.Parse(this.Charged.Replace(",", "").Replace(".", "")) + int.Parse(this.instance.balance_standard(card_amount))).ToString();
                            }
                            double before_balance_value = 0;
                            double after_balance_value = 0;
                            DateTime after_card_real_amount_timeout = DateTime.Now;
                            while (string.IsNullOrEmpty(after_card_real_amount) && DateTime.Now.Subtract(after_card_real_amount_timeout).TotalSeconds<130)
                            {
                                await Task.Delay(100);
                            }
                            if(string.IsNullOrEmpty(after_card_real_amount))
                            {
                                try
                                {
                                    trace_time_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                                    res = 0;
                                    reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"Không nhận được tin nhắn thông báo mệnh giá đã nạp\",\"network\":\"{this.Telco}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Cước nợ trước của bạn là:{before_balance}\",\"after_balance_ussd\":\"Cước nợ trước của bạn là:{after_balance}\",\"dd:MM:yyy_hh:mm:ss\":\"\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{trace_time_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                    var json_object = JsonConvert.DeserializeObject(reply);
                                    string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                    LoggerManager.LogTrace(res_json);
                                    login_instance.wsHelper.sendDataToServer(res_json);
                                    report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Cước nợ trước của bạn là:{before_balance}\",\"after_balance_ussd\":\"Cước nợ trước của bạn là:{after_balance}\",\"sms_time\":\"\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                    var report_after_value = JsonConvert.DeserializeObject(report);

                                    string res_after_value = JsonConvert.SerializeObject(report_after_value);
                                    if (Properties.Settings.Default.reportRechargeOrderList == null)
                                    {
                                        Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                                    }
                                    Properties.Settings.Default.reportRechargeOrderList.Add(res_after_value);
                                    Properties.Settings.Default.Save();
                                    this.instance.transactionUpdate();
                                    is_lock_sim = false;
                                    this.topupResult = "";
                                    return res;
                                }
                                catch (Exception er)
                                {
                                    LoggerManager.LogError("update_recharge_order:" + er.Message);
                                }
                            }
                            try
                            {
                                after_balance =(double.Parse(this.instance.balance_standard(before_balance)) + double.Parse(this.instance.balance_standard(after_card_real_amount))).ToString();
                                this.checkChargeValue = after_balance;
                            }
                            catch(Exception err)
                            {
                                LoggerManager.LogError("after_balance_fail:" + err.Message);
                            }
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
                            card_real_amount = after_card_real_amount;
                            try
                            {
                                trace_time_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                                reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.after_card_sms}\",\"network\":\"{this.Telco}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Cước nợ trước của bạn là:{before_balance}\",\"after_balance_ussd\":\"Cước nợ trước của bạn là:{after_balance}\",\"dd:MM:yyyy_hh:mm:ss\":\"{this.sms_response_time.ToString("dd/MM/yyyy HH:mm:ss")}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{trace_time_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                var json_object = JsonConvert.DeserializeObject(reply);
                                string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                LoggerManager.LogTrace(res_json);
                                login_instance.wsHelper.sendDataToServer(res_json);
                            }
                            catch (Exception er)
                            {
                                LoggerManager.LogError("update_recharge_order:" + er.Message);
                            }
                            string datetime_vietnam_update = this.sms_response_time.ToString("dd/MM/yyyy HH:mm:ss");

                            report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Cước nợ trước của bạn là:{before_balance}\",\"after_balance_ussd\":\"Cước nợ trước của bạn là:{after_balance}\",\"sms_time\":\"{datetime_vietnam_update}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";

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
                            if (string.IsNullOrEmpty(this.Telco))
                            {
                                report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Sim bị mất kết nối trong quá trình nạp.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{network}\",\"sim_type\":\"{sim_type}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tài khoản của bạn là:{before_balance}\",\"after_balance_ussd\":\"Tài khoản của bạn là:{after_balance}\",\"sms_time\":\"{this.sms_response_time}\",\"card_real_amount\":\"0\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
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
                    
                        if (this.is_Topup == false)
                        {  
                            string error_message = this.topupResult;
                            if (error_message.Contains("Dich vu nay khong duoc"))
                            {
                                await this.runUSSD("*901*4*8*11#");
                                await Task.Delay(8000);
                                await this.sendAT($"AT+CUSD=1,\"{serial_card_lock}\",15\r");
                                is_lock_sim = true;
                                DateTime open_lock_timeout = DateTime.Now;
                                while (is_lock_sim && (DateTime.Now.Subtract(open_lock_timeout).TotalSeconds < 90))
                                {
                                    await Task.Delay(100);
                                }
                                 if(!string.IsNullOrEmpty(ussd_lock_sim_status))
                                {
                                    topupResult = ussd_lock_sim_status;
                                }
                            }
                            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                            {
                                this.rowGSMSelected.Cells["status"].Style.BackColor = Color.IndianRed;
                            }));
                            if (this.login_instance.language == "English")
                            {
                                this.Status = "Failed";
                            }
                            else if (this.login_instance.language == "中文")
                            {
                                this.Status = "失败";
                            }
                            else
                            {
                                this.Status = "Thất bại";
                            }
                            this.Message = this.loadMsg(this.topupResult);
                            after_balance = before_balance;
                            balance = after_balance;
                            card_real_amount = "0";
                            try
                            {
                                trace_time_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);

                                reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"dd:MM:yyyy_hh:mm:ss\":\"\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{trace_time_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                var json_object = JsonConvert.DeserializeObject(reply);
                                string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                LoggerManager.LogTrace(res_json);
                                login_instance.wsHelper.sendDataToServer(res_json);
                            }
                            catch (Exception er)
                            {
                                LoggerManager.LogError("update_recharge_order:" + er.Message);
                            }
                            report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"sms_time\":\"\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";

                            var report_json_object = JsonConvert.DeserializeObject(report);

                            string res_report = JsonConvert.SerializeObject(report_json_object);
                            if (Properties.Settings.Default.reportRechargeOrderList == null)
                            {
                                Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                            }
                            Properties.Settings.Default.reportRechargeOrderList.Add(res_report);
                            Properties.Settings.Default.Save();
                            this.instance.transactionUpdate();
                            if (lock_sim_status == 3)
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
                               
                                Match match_after_balance = regex_after_balance.Match(this.topupResult);
                                if (match_after_balance.Success)
                                {
                                    string after_balance_temp = match_after_balance.Value;
                                    Regex reg_after_balance = new Regex(@"[\d,]+");
                                    Match filter_after_balance = reg_after_balance.Match(after_balance_temp);
                                    if (filter_after_balance.Success)
                                    {
                                        after_balance = filter_after_balance.Value;
                                        this.TKC = after_balance;
                                    }
                                }
                                Match match_card_real_amount = regex_ussd.Match(topupResult);
                                if (match_card_real_amount.Success)
                                {
                                    string card_real_amount_temp = match_card_real_amount.Value;
                                    Regex reg_card_amount = new Regex(@"[^\d,]");
                                    card_real_amount = reg_card_amount.Replace(card_real_amount_temp, "");
                                }
                            this.sms_response_time= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                            res = 1;

                            info_log = "Thành công:" + info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                            LoggerManager.LogInfo(info_log);
                            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                            {
                                this.rowGSMSelected.Cells["status"].Style.BackColor = Color.GreenYellow;
                            }));
                            if (this.login_instance.language == "English")
                            {
                                this.Status = "Success";
                            }
                            else if (this.login_instance.language == "中文")
                            {
                                this.Status = "成功";
                            }
                            else
                            {
                                this.Status = "Thành công";
                            }
                            this.Message = this.loadMsg(this.topupResult);
                            string card_amount = info.Payload.Card_Amount;
                            if (!string.IsNullOrEmpty(this.Charge) && !string.IsNullOrEmpty(this.Charged))
                            {
                                this.Charged = (int.Parse(this.Charged.Replace(",", "").Replace(".", "")) + int.Parse(card_amount.Replace(",", "").Replace(".", ""))).ToString();
                            }
                          
                            balance = after_balance;
                        
                           string datetime_vietnam_update =this.sms_response_time.ToString("dd/MM/yyyy HH:mm:ss");
                            trace_time_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);

                            try
                            {
                                reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"dd:MM:yyyy_hh:mm:ss\":\"{this.sms_response_time.ToString("dd/MM/yyyy HH:mm:ss")}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{trace_time_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                var json_object = JsonConvert.DeserializeObject(reply);
                                string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                LoggerManager.LogTrace(res_json);
                                login_instance.wsHelper.sendDataToServer(res_json);
                            }
                            catch (Exception er)
                            {
                                LoggerManager.LogError("update_recharge_order:" + er.Message);
                            }
                            report = $"{{\"command\":\" REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{this.topupResult}\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"sms_time\":\"{datetime_vietnam_update}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
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
                    is_lock_sim = false;
                    this.ussd_lock_sim_status = "";
                    prepaid_topup = "";
                    this.serial_card_lock = "";
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
                        while (string.IsNullOrEmpty(this.ussd_charge) && DateTime.Now.Subtract(ussd_balance_timeout).TotalSeconds < 90)
                        {
                            if (string.IsNullOrEmpty(this.Telco))
                            {
                                res = 0;
                                string err_msg = "Sim bị mất kết nối trong quá trình nạp.";
                                report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Sim bị mất kết nối và không rõ kết quả nạp.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{err_msg}\",\"network\":\"{network}\",\"sim_type\":\"{sim_type}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tong no cuoc cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tong no cuoc cua ban la:{after_balance}\",\"sms_time\":\"\",\"card_real_amount\":\"0\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
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
                        if (string.IsNullOrEmpty(this.ussd_charge))
                        {
                            after_balance = before_balance;
                        }
                        else
                        {
                            after_balance = this.ussd_charge;
                        }
                    }
                    else
                    {
                        this.resetUSSD(this.Port, this.rowGSMSelected);
                        while(string.IsNullOrEmpty(this.ussd_balance) && DateTime.Now.Subtract(ussd_balance_timeout).TotalSeconds < 90)
                        {
                            if (string.IsNullOrEmpty(this.Telco))
                            {
                                res = 0;
                                string err_msg = "Sim bị mất kết nối trong quá trình nạp.";
                                report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Sim bị mất kết nối và không rõ kết quả nạp.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{err_msg}\",\"network\":\"{network}\",\"sim_type\":\"{sim_type}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"sms_time\":\"\",\"card_real_amount\":\"0\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
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
                        if (string.IsNullOrEmpty(this.ussd_balance))
                        {
                            after_balance = before_balance;
                        }
                        else
                        {
                            after_balance = this.ussd_balance;
                        }
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
                            report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"Không xác định được trạng thái nạp.\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"\",\"balance\":\"\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:\",\"sms_time\":\"\",\"card_real_amount\":\"\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
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
                    if (Math.Abs(diff_value) > 0)
                    {   //case nap thanh cong khi khong co ussd message tra ve
                        res = 1;
                        info_log = "Thành công:" + info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                        LoggerManager.LogInfo(info_log);
                        this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                        {
                            this.rowGSMSelected.Cells["status"].Style.BackColor = Color.GreenYellow;
                        }));
                        if (this.login_instance.language == "English")
                        {
                            this.Status = "Undetermined";
                        }
                        else if (this.login_instance.language == "中文")
                        {
                            this.Status = "不确定";
                        }
                        else
                        {
                            this.Status = "Không xác định";
                        }
                        this.Message = "Không nhận được tin nhắn topup trả về";
                        string card_amount = info.Payload.Card_Amount;
                        card_real_amount = "";
                        try
                        {
                            trace_time_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);

                            reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"Không xác định được trạng thái thẻ nạp\",\"network\":\"{this.Telco}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"dd:MM:yyyy_hh:mm:ss\":\"\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{trace_time_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                            var json_object = JsonConvert.DeserializeObject(reply);
                            string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                            LoggerManager.LogTrace(res_json);
                            login_instance.wsHelper.sendDataToServer(res_json);
                        }
                        catch (Exception er)
                        {
                            LoggerManager.LogError("update_recharge_order:" + er.Message);
                        }
                        report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"Nạp thẻ thành công.\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"sms_time\":\"\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
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
                    else if(diff_value==0)
                    {   //case nap that bai khi khong co ussd message tra ve
                        this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                        {
                            this.rowGSMSelected.Cells["status"].Style.BackColor = Color.IndianRed;
                        }));
                        res = 0;
                        info_log = "Thất bại:" + info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                        LoggerManager.LogInfo(info_log);
                        if (this.login_instance.language == "English")
                        {
                            this.Status = "Failed";
                        }
                        else if (this.login_instance.language == "中文")
                        {
                            this.Status = "失败";
                        }
                        else
                        {
                            this.Status = "Thất bại";
                        }
                        string error_msg = "Có lỗi xảy ra khi nạp thẻ";
                        this.Message = this.loadMsg(error_msg);
                        after_balance = this.ussd_balance;
                        card_real_amount = "0";
                       try
                        {
                            trace_time_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);

                            reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{error_msg}\",\"network\":\"{this.Telco}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"dd:MM:yyyy_hh:mm:ss\":\"\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{trace_time_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                            var json_object = JsonConvert.DeserializeObject(reply);
                            string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                            LoggerManager.LogTrace(res_json);
                            login_instance.wsHelper.sendDataToServer(res_json);
                        }
                        catch (Exception er)
                        {
                            LoggerManager.LogError("update_recharge_order:" + er.Message);
                        }
                        report = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{error_msg}\",\"network\":\"{this.Telco}\",\"sim_type\":\"{sim_type}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"sms_time\":\"\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
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
        public async void HandleSerialData(string input)
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
                    if (this.login_instance.language == "English")
                    {
                        this.Status = "Success";
                        this.Message = loadMsg("SIM Card is ready");
                    }
                    else if (this.login_instance.language == "中文")
                    {
                        this.Status = "准备";
                        this.Message = loadMsg("SIM卡准备好了");
                    }
                    else
                    {
                        this.Status = "Sẵn sàng";
                        this.Message = loadMsg("Sim đã sẵn sàng");

                    }
                    this._isSim = true;
                    this.checkSimHasResult = true;
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
                    if (this.login_instance.language == "English")
                    {
                        this.Status = "No Sim";
                        this.Message = loadMsg("No Sim detected");
                    }
                    else if (this.login_instance.language == "中文")
                    {
                        this.Status = "没有SIM卡";
                        this.Message = loadMsg("不发现SIM卡");
                    }
                    else
                    {
                        this.Status = "No Sim";
                        this.Message = loadMsg("Không nhận dạng được SIM");

                    }
                    this.TKC = "";
                    this.TKKM = "";
                    this.sp.Close();
                    this.sp.Open();
                }
                if (!is_modem && input.ToUpper().Contains("QUECTEL"))
                {
                    is_modem = true;
                    string uppercase_value = input.ToUpper();
                    string search_word = "QUECTEL";
                    string pattern = $@"{search_word}(.*)";
                    Match match_words = Regex.Match(uppercase_value, pattern);
                    if(match_words.Success) 
                    {
                        string res_word = match_words.Value;
                        this.Modem = res_word.Trim().Replace("OK", "");
                    }
                }
                if (input.Contains("+COPS:"))
                {
                    string str = input.Replace(" ", "").Replace("\n\r", "");
                    this.Telco = !str.ToUpper().Contains("VIETTEL") ? (!str.ToUpper().Contains("MOBIFONE") ? (!str.ToUpper().Contains("VINAPHONE") ? (!str.ToUpper().Contains("VIETNAMOBILE") ? "Sim tạm thời bị khóa hoặc không có sóng" : "VIETNAMOBILE") : "VINAPHONE") : "MOBIFONE") : "VIETTEL";
                    if (this.Telco == "Sim tạm thời bị khóa hoặc không có sóng")
                    {
                        if (this.login_instance.language == "English")
                        {
                            this.Status = "No Carrier";
                            this.Message = loadMsg("No Telco detected");
                        }
                        else if (this.login_instance.language == "中文")
                        {
                            this.Status = "没有网络";
                            this.Message = loadMsg("不发现网络");
                        }
                        else
                        {
                            this.Status = "No Carrier";
                            this.Message = loadMsg("Không đọc được nhà mạng của Sim");

                        }
                       
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
                                string tkc_viettel_pattern = @"\b\d+(\.\d+)*d\b";
                                Match tkc_viettel = Regex.Match(input, tkc_viettel_pattern);
                                Match match = new Regex(".*?" + char.ConvertFromUtf32(34) + "(\\d+).*?:([0-9\\.]+)d[a-zA-Z0?,:]+(\\d+\\/\\d+\\/\\d+).*").Match(phone_reg);
                                this.Phone = "0" + match.Groups[1].ToString().Substring(2);
                                if (phone_split[1].Contains("."))
                                {
                                    if (tkc_viettel.Success)
                                    {
                                        this.TKC = tkc_viettel.Value.Replace("d", "");
                                    }
                                }
                                else
                                {
                                    Regex reg_digit = new Regex(@"(\b\d+(\.\d+)*d\b");
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
                                if (this.login_instance.language == "English")
                                {
                                    this.Message = loadMsg(this.Port + "detect number successfully:Phone Number:"+this.Phone+" Network:"+this.Telco);
                                }
                                else if (this.login_instance.language == "中文")
                                {
                                    this.Message = loadMsg(this.Port + "获得号码成功：号码:"+this.Phone+" 网络:"+this.Telco);
                                }
                                else
                                {
                                    this.Message = loadMsg(this.Port + " đã nhận dạng thành công: SĐT:" + this.Phone + " Network:" + this.Telco);
                                }
                            }
                            else if (this.is_updateTKC == true && this.Telco.Equals("VIETTEL"))
                            {
                                this.is_updateTKC = false;
                            }
                        }
                        else
                        {
                            if (this.login_instance.language == "English")
                            {
                                this.Phone = "Loading";
                                this.Message = loadMsg(this.Port+" cannot detect phone number");
                            }
                            else if (this.login_instance.language == "中文")
                            {
                                this.Phone = "加载中";
                                this.Message = loadMsg(this.Port+"不获得号码");
                            }
                            else
                            {
                                this.Phone = "Loading";
                                this.Message = loadMsg(this.Port + " không nhận dạng được số điện thoại.");
                            }
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


                                if (input.Contains("Tai khoan chinh"))
                                {
                                    Regex main_balance_regex = new Regex(@"[\d,]+\s+VND");
                                    Match match_main_balance = main_balance_regex.Match(input);
                                    if (match_main_balance.Success)
                                    {
                                        this.TKC = match_main_balance.Value.Replace("VND", "");
                                    }
                                    Regex expire_regex = new Regex(@"(\b\d{1,2}\/\d{1,2}\/\d{4}\b)");
                                    Match match_expire = expire_regex.Match(input);
                                    if (match_expire.Success)
                                    {
                                        this.Expire = match_expire.Value;
                                    }
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
                                }
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
                        LoggerManager.LogInfo($"SMS_RESPONSE_TIME FOR {this.Phone}:{sms_response_time}");
                        for(int i=1;i<value.Length-1;i++)
                        {
                            if(i!=value.Length-2)
                            {
                                mess += value[i] + ",";
                            }
                            else
                            {
                                mess += value[i];
                            }
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
                    if (is_lock_sim)
                    {
                        is_lock_sim = false;
                        if(input.Contains("mo lai quyen nap the")|| input.Contains("su dung so serie the moi"))
                        {
                            this.lock_sim_status = 2;
                        }
                        else
                        {
                            this.lock_sim_status = 3;          
                        }
                        try
                        {
                            string lock_sim_response = input.Split(',')[1];
                            if (!string.IsNullOrEmpty(lock_sim_response))
                            {
                                ussd_lock_sim_status = lock_sim_response.Replace("\"", "");
                            }
                        }
                        catch(Exception er) 
                        {
                            LoggerManager.LogError("parse ussd_lock_sim_status exception:" + er.Message);
                        }
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
                        this.Otp = re;
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
                        {   sms_response_time= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                            Regex regex_ussd = new Regex(@"[\d]+\s");
                            Match match_value=regex_ussd.Match(txt1);
                            if(match_value.Success) 
                            { 
                                Regex reg_card_amount = new Regex(@"[^\d,]");
                                after_card_sms = match_value.Value;
                                after_card_real_amount = reg_card_amount.Replace(match_value.Value, "");
                            }
                        }
                        else if(txt2=="MobiFone")
                        {
                            if(txt1.Contains("da nap"))
                            {
                                prepaid_topup = txt1;
                                
                                this.sms_response_time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
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
                        if (this.login_instance.language == "English")
                        {
                            this.Message = loadMsg(this.Port + " cannot connect");
                        }
                        else if (this.login_instance.language == "中文")
                        {
                            this.Message = loadMsg(this.Port + "失了信号");
                        }
                        else
                        {
                            this.Message = loadMsg("Port " + this.Port + " không thể kết nối");
                        }
                    }
                    else
                    {
                        this._isSim = false;
                        if (this.login_instance.language == "English")
                        {
                            this.Status = "Try Open Port";
                            this.Message = this.Port + " is opening";
                        }
                        else if (this.login_instance.language == "中文")
                        {
                            this.Status = "启动端口";
                            this.Message = this.Port + "正在启动";
                        }
                        else
                        {
                            this.Status = "Try Open Port";
                            this.Message = this.Port + " đang mở cổng";
                        }
                        
                        bool try_open_port = this.TryOpenPort();
                        if (!try_open_port)
                        {
                            if (this.login_instance.language == "English")
                            {
                                this.Status = "Port Close";
                                this.Message = this.Port + " is closed";
                            }
                            else if (this.login_instance.language == "中文")
                            {
                                this.Status = "端口关掉";
                                this.Message = this.Port + "已关掉";
                            }
                            else
                            {
                                this.Status = "Port Close";
                                this.Message = this.Port + " đã đóng";
                            }
                     
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
                                                if (this.login_instance.language == "English")
                                                {
                                                    this.Status = "Try Open Port";
                                                    this.Message = this.Port + " is opening";
                                                }
                                                else if (this.login_instance.language == "中文")
                                                {
                                                    this.Status = "启动端口";
                                                    this.Message = this.Port + "正在启动";
                                                }
                                                else
                                                {
                                                    this.Status = "Try Open Port";
                                                    this.Message = this.Port + " đang mở cổng";
                                                }
                                                this.TKC = "";
                                                this.Expire = "";
                                                this.SimType = "";
                                                this.TKKM = "";
                                                this.Otp = "";
                                                this.Regis_Status = "";
                                                this.rowGSMSelected.Cells["status"].Style.BackColor = Color.White;
                                                this.Charge = "";
                                                this.Charged = "";
                                                this.Modem = "";
                                                this.is_modem = false;
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

                                            if(string.IsNullOrEmpty(this.Modem))
                                            {
                                                await this.sendAT("AT+CGMI");
                                                await Task.Delay(100);
                                            }

                                            if ((this.Telco == "" || this.Telco == "Sim tạm thời bị khóa hoặc không có sóng") && DateTime.Now.Subtract(this.lastReportNetwork).TotalSeconds > 5.0)
                                            {
                                                this.lastReportNetwork = DateTime.Now;
                                                await this.sendAT("AT+COPS?");
                                                await Task.Delay(100);
                                            }
                                            if (!this.CarrierHasResult && this.Telco != "" && this.Telco != "Sim tạm thời bị khóa hoặc không có sóng" && DateTime.Now.Subtract(this.lastReportPhone).TotalSeconds > 10)
                                            {  if (is_updateTKC == false)
                                                {
                                                    if (this.login_instance.language == "English")
                                                    {
                                                        this.Status = "Ready";
                                                        this.Message = loadMsg(this.Port + " is getting phone number");
                                                        this.Phone = "Loading";
                                                    }
                                                    else if (this.login_instance.language == "中文")
                                                    {
                                                        this.Status = "准备";
                                                        this.Message = loadMsg(this.Port + "开始认识号码");
                                                        this.Phone = "加载中";
                                                    }
                                                    else
                                                    {
                                                        this.Status = "Sẵn sàng";
                                                        this.Message = loadMsg(this.Port + " bắt đầu nhận dạng SĐT");
                                                        this.Phone = "Loading";

                                                    }
                                                    this.lastReportPhone = DateTime.Now;
                                                    this._isPhone = true;
                                                }
                                                else
                                                {
                                                    
                                                    this.lastReportPhone = DateTime.Now;
                                                    if (this.login_instance.language == "English")
                                                    {
                                                        this.Status = "Ready";
                                                        this.Phone = "Loading";
                                                    }
                                                    else if (this.login_instance.language == "中文")
                                                    {
                                                        this.Status = "准备";
                                                        this.Phone = "加载中";
                                                    }
                                                    else
                                                    {
                                                        this.Status = "Sẵn sàng";
                                                        this.Phone = "Loading";
                                                    }
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

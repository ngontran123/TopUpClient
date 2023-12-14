using Guna.UI2.WinForms.Enums;
using NaptienMaster.GSM;
using NaptienMaster.Items;
using NaptienMaster.ResponseItem;
using NaptienMaster.Services;
using Newtonsoft.Json;
using NLog.LayoutRenderers;
using NLog.LayoutRenderers.Wrappers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Primitives;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace NaptienMaster
{  
    public partial class Form1 : Form
    {
        private static Form1 instance;
        public string baudrate = "";
        public string blackListPort = "";
        
        public int new_port_num = -1;
        private int num_of_sim = 0;
        private int max_num_of_sim = 0;
        public bool is_load_port_activated = false;
        public bool openPort = false;
        public bool init_count = true;
        public SemaphoreSlim checkLock = new SemaphoreSlim(5);
        public List<GsmObject> gsmObject = new List<GsmObject>();
        public List<string> phone_list = new List<string>();
        public List<string> temp_phone_list = new List<string>();
        public object lockLoadData = new object();
        public Login login_instance = Login.ReturnLoginInstance();
        private System.Timers.Timer push_sim_timer = new System.Timers.Timer();
        private TimeZoneInfo vietnam_standard_time = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");      
        private List<string> current_transaction_list = new List<string>();
        private List<string> filter_transaction_list = new List<string>();
        public static Form1 ReturnInstance()
        {
            return instance;
        }
        public Form1()
        {   instance= this;
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.Icon = null;
            this.Size = new Size(1942, 830);
            UpdateGUI.dataForm += new UIViewRow(UpdateDataGridViewRow);
        }

        public void addPortSetting(int size)
        {
            try
            {

                int initial_size = Properties.Settings.Default.PortCustom.Count;

                for (int i = initial_size - 1; i < size + (initial_size - 1); i++)
                {
                    Properties.Settings.Default.PortCustom.Add($"Cổng {i + 1}");
                    Properties.Settings.Default.Save();
                }

            }
            catch (Exception er)
            {
                this.loadData("add port setting:"+er.Message);
            }
        }
        public void setupPortSetting()
        {
            if (Properties.Settings.Default.PortCustom == null)
            {
                Properties.Settings.Default.PortCustom = new System.Collections.Specialized.StringCollection();
            }
            int size = Properties.Settings.Default.PortCustom.Count;

            if (size < new_port_num)
            {
                int remain = new_port_num - size;
                addPortSetting(remain);
            }
            for (int i = 0; i < dataGSM.RowCount; i++)
            {
                DataGridViewRow row = dataGSM.Rows[i];
                dataGSM.Invoke(new MethodInvoker(() =>
                {
                    row.Cells["port_custom"].Value = Properties.Settings.Default.PortCustom[i];
                }));
            }
        }
        public void updatePortSetting(DataGridView datagrid)
        {
            for (int i = 0; i < datagrid.RowCount; i++)
            {
                DataGridViewRow row = datagrid.Rows[i];
                if (Properties.Settings.Default.PortCustom[i] != null)
                {
                    try
                    {
                        Properties.Settings.Default.PortCustom[i] = row.Cells["port_custom"].Value.ToString();
                    }
                    catch (Exception er)
                    {
                        this.loadData("update port setting"+er.Message);
                    }
                }
            }
            Properties.Settings.Default.Save();
        }
        public DataGridViewRow newRow()
        {
            int rowId = -1;
            try
            {
                this.dataGSM.Invoke(new Action(() => rowId = this.dataGSM.Rows.Add()));
            }
            catch(Exception ex)
            {
                loadData("new row"+ex.Message);
            }
            return this.dataGSM.Rows[rowId];
        }
        public void updateSetting()
        {
            InforSettingSection inforConfig = (InforSettingSection)ConfigurationManager.GetSection("infoSet");
            baudrate = inforConfig.Baudrate;
        }
        public void updateCom()
        {
            AppSettingSection appConfig = (AppSettingSection)ConfigurationManager.GetSection("appSet");
            blackListPort = "";
            foreach (AppSettingElement ele in appConfig.collect)
            {
                blackListPort += ele.Com + ",";
            }
            if (blackListPort.Length > 0)
            {
                blackListPort = blackListPort.Remove(blackListPort.Length - 1);
            }
        }
        public void UpdateDataGridViewRow(DataGridViewRow row,string name,string value)
        {
            try
            {
                if (row == null || instance == null)
                {
                    return;
                }
                this.dataGSM.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        row.Cells[name].Value = value;
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }));
            }
            catch(Exception ex) { loadData("datagridviewrow:"+ex.Message); }
        }
        private void settingBtn_Click(object sender, EventArgs e)
        {
        }
        public async Task handlingServerEvent()
        { 
            handle_loop:
            try
            {
                if (login_instance.wsHelper.Client_List_Amount.Count > 0)
                {
                    var client_amount_info_list = login_instance.wsHelper.Client_List_Amount;

                    foreach (var client_amount in client_amount_info_list)
                    {
                        var res_client_amount = JsonConvert.DeserializeObject<ClientAmount>(client_amount);
                        string phone_update = res_client_amount.Payload.Phone;
                        string recharged = res_client_amount.Payload.Amount_Recharged;
                        string need_recharge = res_client_amount.Payload.Amount_Need_Recharge;
                        var num = updateClientAmountInfoAction(phone_update, recharged, need_recharge).First();
                        int num_res = num.Value;
                        string telco_res = num.Key;
                        sendUpdateClientAmountRes(num_res, phone_update, telco_res);
                        await Task.Delay(500);
                        login_instance.wsHelper.Client_List_Amount = login_instance.wsHelper.Client_List_Amount.Remove(client_amount);
                    }
                }
                await Task.Delay(100);
                if (login_instance.wsHelper.Balance_Info_List.Count > 0)
                {
                    var balance_info_list = login_instance.wsHelper.Balance_Info_List;
                    foreach (var balance_info in balance_info_list)
                    {
                        var res_balance_info = JsonConvert.DeserializeObject<Balance_Request>(balance_info);
                        string phone_balance = res_balance_info.Payload.Phone;
                        int num = await sendBalanceInfoAction(phone_balance);
                        await Task.Delay(500);
                        login_instance.wsHelper.Balance_Info_List = login_instance.wsHelper.Balance_Info_List.Remove(balance_info);
                    }
                }
                await Task.Delay(100);
                if (login_instance.wsHelper.Recharge_Order_List.Count > 0)
                {
                    var recharge_order_list = login_instance.wsHelper.Recharge_Order_List;
                    foreach (var order in recharge_order_list)
                    {
                        new Task(async () =>
                    {
                        try
                        {
                            var res_order_info = JsonConvert.DeserializeObject<RechargeOrder>(order);
                            login_instance.wsHelper.Recharge_Order_List = login_instance.wsHelper.Recharge_Order_List.Remove(order);
                            string phone_order = res_order_info.Payload.Phone;
                            string code = res_order_info.Payload.Card_Code;
                            GsmObject gsm = gsmObject.SingleOrDefault(p => p.Phone == phone_order);
                            if (gsm != null)
                            {
                                int val = await gsm.OrderHandling(phone_order, code, res_order_info);
                            }
                            else
                            {
                                Guid guid = Guid.NewGuid();
                                DateTime datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                                string reply_push_recharge = $"{{\"command\":\"PUSH_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":false,\"message\":\"Sim này không tồn tại.\",\"extra_data\":{{\"phone\":\"{res_order_info.Payload.Phone}\",\"card_serial\":\"{res_order_info.Payload.Card_Serial}\",\"card_code\":\"{res_order_info.Payload.Card_Code}\",\"card_amount\":\"{res_order_info.Payload.Card_Amount}\",\"task_id\":\"{res_order_info.Payload.Task_Id}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                                var json_object_push_recharge = JsonConvert.DeserializeObject(reply_push_recharge);
                                string res_json_push_recharge = JsonConvert.SerializeObject(json_object_push_recharge, Formatting.Indented);
                                login_instance.wsHelper.sendDataToServer(res_json_push_recharge);
                                return;
                            }
                            gsm = null;
                            res_order_info = null;
                        }
                        catch (Exception er)
                        {
                            LoggerManager.LogError("order_object_handling:" + er.Message);
                            this.loadData("order_object_handling:" + er.Message);
                        }
                    }).Start();
                        await Task.Delay(1000);
                    }
                }
                if(login_instance.wsHelper.Report_Recharge_Order_List.Count > 0)
                {
                    var report_recharge_order_list = login_instance.wsHelper.Report_Recharge_Order_List;
                    foreach (var report_order in report_recharge_order_list)
                    {
                        var report_info = JsonConvert.DeserializeObject<ReportRechargeOrder>(report_order);
                        login_instance.wsHelper.Report_Recharge_Order_List = login_instance.wsHelper.Report_Recharge_Order_List.Remove(report_order);
                        string report_phone = report_info.Payload.Phone;
                        string card_serial = report_info.Payload.Card_Serial;
                        string card_code = report_info.Payload.Card_Code;
                        string card_amount = report_info.Payload.Card_Amount;
                        string task_id = report_info.Payload.Task_Id;
                        int val = await reportOrderHandling(report_phone, card_serial, card_code, card_amount, task_id);
                        await Task.Delay(500);
                    }
                }
                await Task.Delay(5000);
                goto handle_loop;
            }
            catch(Exception er)
            {
                LoggerManager.LogError("Error in handleEvent:" + er.Message);
            }
        }

       
        public async Task<int> sendBalanceInfoAction(string phone)
        {
            int res = 0;
            try
            {
                DateTime ussd_waiting = DateTime.Now;
                string reply = "";
                GsmObject gsm = gsmObject.SingleOrDefault(p => p.Phone == phone);
                Guid guid = Guid.NewGuid();
                string balance_info = "";
                if (gsm != null)
                {
                    gsm.resetUSSD(gsm.Port, gsm.rowGSMSelected);
                    while (string.IsNullOrEmpty(gsm.ussd_balance))
                    {
                        await Task.Delay(100);
                    }
                    balance_info = gsm.ussd_balance;
                    if(!string.IsNullOrEmpty(gsm.ussd_balance))
                    {
                        balance_info = $"Tài khoản của bạn là {gsm.ussd_balance}";
                    }
                    if(!string.IsNullOrEmpty(balance_info))
                    {
                        string balance = gsm.TKC;
                        DateTime datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                        reply = $"{{\"command\":\"BALANCE_INFO_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Lấy số dư thành công\",\"extra_data\":{{\"phone\":\"{phone}\",\"balance\":\"{balance}\",\"ussd_balance\":\"{balance_info}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                        res = 1;
                    }
                    else
                    {
                        DateTime datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                        reply = $"{{\"command\":\"BALANCE_INFO_RESPONSE_ACTION\",\"payload\":{{\"success\":false,\"message\":\"Lấy số dư thất bại\",\"extra_data\":{{\"phone\":\"{phone}\",\"balance\":\"{-1}\",\"ussd_balance\":\"{balance_info}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                    }
                }
                else
                {
                    DateTime datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                    reply = $"{{\"command\":\"BALANCE_INFO_RESPONSE_ACTION\",\"payload\":{{\"success\":false,\"message\":\"Không tìm thấy số điện thoại\",\"extra_data\":{{\"phone\":\"{phone}\",\"balance\":\"{-1}\",\"ussd_balance\":\"{balance_info}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\"}}";
                }
                var json_ob = JsonConvert.DeserializeObject(reply);
                string res_json = JsonConvert.SerializeObject(json_ob,Formatting.Indented);
                
                gsm = null;
                login_instance.wsHelper.sendDataToServer(res_json);
                LoggerManager.LogTrace(res_json);
            }
            catch(Exception er)
            {
                loadData("send balance info:"+er.Message);
                LoggerManager.LogError(er.Message);
            }
            return res;
        }
        public async Task<int> OrderHandling(string phone,string code,RechargeOrder info)
        {
            int res = -1;
            try
            {
                DateTime topUpResponse= DateTime.Now;
                string reply = "";
                string reply_push_recharge = "";
                string report = "";
                string before_balance = "";
                string after_balance = "";
                string card_real_amount = "";
                string balance = "";
                GsmObject gsm = gsmObject.SingleOrDefault(p => p.Phone == phone);
                DateTime ussd_balance_timeout;
                Guid guid = Guid.NewGuid();
                    if (gsm != null)
                    {
                    if (gsm.Status == "Đang xử lý")
                    {
                        return res;
                    }
                    string info_log = info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                    LoggerManager.LogInfo(info_log);
                    DateTime datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                    reply_push_recharge = $"{{\"command\":\"PUSH_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":\"true\",\"message\":\"Bắt đầu xử lý\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\"}}";
                        var json_object_push_recharge = JsonConvert.DeserializeObject(reply_push_recharge);
                        string res_json_push_recharge = JsonConvert.SerializeObject(json_object_push_recharge, Formatting.Indented);
                        login_instance.wsHelper.sendDataToServer(res_json_push_recharge);
                        before_balance = gsm.TKC;
                    if(string.IsNullOrEmpty(before_balance))
                    {
                        res = -1;
                        return res;
                    }
                        gsm.Message = gsm.loadMsg("Bắt đầu xử lý");
                        gsm.Status = "Đang xử lý";
                        this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                        {
                            if (gsm != null)
                            {
                                gsm.rowGSMSelected.Cells["status"].Style.BackColor = Color.Aquamarine;
                            }
                        }));
                        gsm.runTopUp(code);
                        while (DateTime.Now.Subtract(topUpResponse).TotalSeconds < 50 && string.IsNullOrEmpty(gsm.topupResult))
                        {
                            await Task.Delay(100);
                        }
                        if (!string.IsNullOrEmpty(gsm.topupResult))
                        {
                            if (gsm.is_Topup == false)
                            {
                                this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                {
                                        gsm.rowGSMSelected.Cells["status"].Style.BackColor = Color.IndianRed;
                                }));
                            info_log ="Thất bại:"+info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                            LoggerManager.LogInfo(info_log);
                            res = 0;
                                gsm.Status = "Thất bại";
                                gsm.Message = gsm.loadMsg(gsm.topupResult);
                                ussd_balance_timeout = DateTime.Now;
                                gsm.resetUSSD(gsm.Port, gsm.rowGSMSelected);
                                while (string.IsNullOrEmpty(gsm.ussd_balance))
                                {  if (DateTime.Now.Subtract(ussd_balance_timeout).TotalSeconds > 20)
                                {
                                    try
                                    {
                                        if (gsm != null)
                                        {
                                            gsm.resetUSSD(gsm.Port, gsm.rowGSMSelected);
                                        }
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
                             
                                after_balance = gsm.ussd_balance;
                                double before_balance_value;
                                double after_balance_value;
                            before_balance_value = double.Parse(balance_standard(before_balance));
                            after_balance_value = double.Parse(balance_standard(after_balance));
                            balance = after_balance_value.ToString();
                            card_real_amount = (after_balance_value - before_balance_value).ToString();
                            try
                            {
                                 datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);

                                reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{gsm.topupResult}\",\"network\":\"{gsm.Telco}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\"}}";

                                var json_object = JsonConvert.DeserializeObject(reply);
                                string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                login_instance.wsHelper.sendDataToServer(res_json);
                            }
                            catch(Exception er)
                            {
                                LoggerManager.LogError("update_recharge_order:" + er.Message);
                            }
                                report = $"{{\"command\":\" REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":\"false\",\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"message_response\":\"{gsm.topupResult}\",\"network\":\"{gsm.Telco}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\"}}";

                                var report_json_object = JsonConvert.DeserializeObject(report);

                                string res_report = JsonConvert.SerializeObject(report_json_object);
                                if (Properties.Settings.Default.reportRechargeOrderList == null)
                                {
                                    Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                                }
                                Properties.Settings.Default.reportRechargeOrderList.Add(res_report);
                                Properties.Settings.Default.Save();
                                transactionUpdate();
                            string error_message = gsm.topupResult;
                            if(error_message.Contains("Dich vu nay khong duoc"))
                            {
                                string error_res =pushSingleSimInfo(phone, 0);
                                if (!string.IsNullOrEmpty(error_res))
                                {
                                    this.login_instance.wsHelper.sendDataToServer(error_res);
                                    LoggerManager.LogTrace("Lỗi dịch vụ không cho phép:"+error_res);
                                }
                            }
                            }
                            else
                            {
                             res = 1;
                             info_log = "Thành công:"+info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                            LoggerManager.LogInfo(info_log);
                            this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                {
                                    if (gsm != null)
                                    {
                                        gsm.rowGSMSelected.Cells["status"].Style.BackColor = Color.GreenYellow;
                                    }
                                }));
                                gsm.Status = "Thành công";
                                gsm.Message = gsm.loadMsg(gsm.topupResult);
                                ussd_balance_timeout = DateTime.Now;
                            gsm.resetUSSD(gsm.Port, gsm.rowGSMSelected);
                            while (string.IsNullOrEmpty(gsm.ussd_balance))
                            {
                                if (DateTime.Now.Subtract(ussd_balance_timeout).TotalSeconds > 20)
                                {
                                    try
                                    {
                                        if (gsm != null)
                                        {
                                            gsm.resetUSSD(gsm.Port, gsm.rowGSMSelected);
                                        }
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
                            after_balance = gsm.ussd_balance;
                                string card_amount = info.Payload.Card_Amount;
                                if (!string.IsNullOrEmpty(gsm.Charge) && !string.IsNullOrEmpty(gsm.Charged))
                                {
                                    gsm.Charged = (int.Parse(gsm.Charged.Replace(",", "").Replace(".", "")) + int.Parse(card_amount.Replace(",", "").Replace(".", ""))).ToString();
                                }
                                double before_balance_value;
                                double after_balance_value;
                            before_balance_value = double.Parse(balance_standard(before_balance));
                            after_balance_value = double.Parse(balance_standard(after_balance));
                            balance = after_balance_value.ToString();
                                card_real_amount = (after_balance_value - before_balance_value).ToString();
                            datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                            try
                            {
                                reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{gsm.topupResult}\",\"network\":\"{gsm.Telco}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\"}}";
                                var json_object = JsonConvert.DeserializeObject(reply);
                                string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                login_instance.wsHelper.sendDataToServer(res_json);
                            }
                            catch(Exception er)
                            {
                                LoggerManager.LogError("update_recharge_order:" + er.Message);
                            }
                                report = $"{{\"command\":\" REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":\"true\",\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"message_response\":\"{gsm.topupResult}\",\"network\":\"{gsm.Telco}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\"}}";
                                var report_json_object = JsonConvert.DeserializeObject(report);
                                string res_report = JsonConvert.SerializeObject(report_json_object);
                                if (Properties.Settings.Default.reportRechargeOrderList == null)
                                {
                                    Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                                }
                                Properties.Settings.Default.reportRechargeOrderList.Add(res_report);
                                Properties.Settings.Default.Save();
                                transactionUpdate();
                            }
                            gsm.topupResult = "";
                        }
                        else
                        {  
                        if(gsm.lock_pin)
                        {
                            gsm.lock_pin = false;
                        }
                           gsm.Message = "Tiến hành tính toán thẻ.";
                        ussd_balance_timeout = DateTime.Now;
                        gsm.resetUSSD(gsm.Port, gsm.rowGSMSelected);
                        while (string.IsNullOrEmpty(gsm.ussd_balance))
                        {
                            if (DateTime.Now.Subtract(ussd_balance_timeout).TotalSeconds > 20)
                            {
                                try
                                {
                                    if (gsm != null)
                                    {
                                        gsm.resetUSSD(gsm.Port, gsm.rowGSMSelected);
                                    }
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
                        after_balance = gsm.ussd_balance;
                        double before_balance_value;
                        double after_balance_value;
                        before_balance_value = double.Parse(balance_standard(before_balance));
                        after_balance_value = double.Parse(balance_standard(after_balance));
                        double diff_value = (after_balance_value - before_balance_value);
                            balance = after_balance_value.ToString();
                            if (diff_value > 0)
                            {
                                res = 1;
                            res = 1;
                            info_log = "Thành công:" + info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                            LoggerManager.LogInfo(info_log);
                            this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                {
                                    if (gsm != null)
                                    {
                                        gsm.rowGSMSelected.Cells["status"].Style.BackColor = Color.GreenYellow;
                                    }
                                }));
                                gsm.Status = "Thành công";
                                gsm.Message = "Nạp thẻ thành công.";
                                string card_amount = info.Payload.Card_Amount;
                                if (!string.IsNullOrEmpty(gsm.Charge) && !string.IsNullOrEmpty(gsm.Charged))
                                {
                                    gsm.Charged = (int.Parse(gsm.Charged.Replace(",", "").Replace(".", "")) + int.Parse(card_amount.Replace(",", "").Replace(".", ""))).ToString();
                                }
                                card_real_amount = diff_value.ToString();
                            datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                            try
                            {
                                reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"Bạn đã nạp {card_real_amount} VND vào TKC.\",\"network\":\"{gsm.Telco}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\"}}";
                                var json_object = JsonConvert.DeserializeObject(reply);
                                string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                login_instance.wsHelper.sendDataToServer(res_json);
                            }
                            catch(Exception er)
                            {
                                LoggerManager.LogError("update_recharge_order:" + er.Message);
                            }
                                report = $"{{\"command\":\" REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":\"true\",\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"message_response\":\"Nạp thẻ thành công.\",\"network\":\"{gsm.Telco}\",\"status\":\"2\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\"}}";
                                var report_json_object = JsonConvert.DeserializeObject(report);
                                string res_report = JsonConvert.SerializeObject(report_json_object);
                                if (Properties.Settings.Default.reportRechargeOrderList == null)
                                {
                                    Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                                }
                                Properties.Settings.Default.reportRechargeOrderList.Add(res_report);
                                Properties.Settings.Default.Save();
                                transactionUpdate();
                            }
                            else
                            {
                                this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                {
                                    if (gsm != null)
                                    {
                                        gsm.rowGSMSelected.Cells["status"].Style.BackColor = Color.IndianRed;
                                    }
                                }));
                                res = 0;
                            info_log = "Thất bại:" + info.Payload.Phone + " " + info.Payload.Card_Serial + " " + info.Payload.Card_Code + " " + info.Payload.Card_Amount;
                            LoggerManager.LogInfo(info_log);
                            gsm.Status = "Thất bại";
                            string error_msg = "Có lỗi xảy ra khi nạp thẻ";
                            gsm.Message = gsm.loadMsg(error_msg);
                            after_balance = gsm.ussd_balance;
                            card_real_amount = (after_balance_value - before_balance_value).ToString();
                            datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                            try
                            {
                                reply = $"{{\"command\":\"UPDATE_RECHARGE_ORDER_RESULT_ACTION\",\"payload\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"ussd_message\":\"{gsm.topupResult}\",\"network\":\"{gsm.Telco}\",\"status\":\"3\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\"}}";
                                var json_object = JsonConvert.DeserializeObject(reply);
                                string res_json = JsonConvert.SerializeObject(json_object, Formatting.Indented);
                                login_instance.wsHelper.sendDataToServer(res_json);
                            }
                            catch(Exception er)
                            {
                                LoggerManager.LogError("update_recharge_order:" + er.Message);
                            }
                                report = $"{{\"command\":\" REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":\"false\",\"message\":\"Truy vấn trạng thái thẻ thành công.\",\"extra_data\":{{\"phone\":\"{info.Payload.Phone}\",\"card_serial\":\"{info.Payload.Card_Serial}\",\"card_code\":\"{info.Payload.Card_Code}\",\"card_amount\":\"{info.Payload.Card_Amount}\",\"task_id\":\"{info.Payload.Task_Id}\",\"message_response\":\"{error_msg}\",\"network\":\"{gsm.Telco}\",\"status\":\"1\",\"extra_data\":{{\"before_balance\":\"{before_balance}\",\"after_balance\":\"{after_balance}\",\"balance\":\"{balance}\",\"before_balance_ussd\":\"Tai khoan chinh cua ban la:{before_balance}\",\"after_balance_ussd\":\"Tai khoan chinh cua ban la:{after_balance}\",\"card_real_amount\":\"{card_real_amount}\"}}}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam}\",\"trace_side\":\"cs\"}}";
                                var report_json_object = JsonConvert.DeserializeObject(report);
                                string res_report = JsonConvert.SerializeObject(report_json_object);
                                if (Properties.Settings.Default.reportRechargeOrderList == null)
                                {
                                    Properties.Settings.Default.reportRechargeOrderList = new System.Collections.Specialized.StringCollection();
                                }
                                Properties.Settings.Default.reportRechargeOrderList.Add(res_report);
                                Properties.Settings.Default.Save();
                                transactionUpdate();
                            }
                        }
                    }
                else
                {
                    return res;
                }
            }
            catch(Exception er)
            {
                LoggerManager.LogError("order_handling:"+er.Message);
            }
            return res;
        }
        private Dictionary<string,int> updateClientAmountInfoAction(string phone,string charged,string need_recharge)
        {
            Dictionary<string, int> res = new Dictionary<string, int>();
            try
            {
                GsmObject gsm = gsmObject.Find(p => p.Phone == phone);
                if(gsm!=null)
                {  
                   gsm.Charge = need_recharge;
                   gsm.Charged = charged;
                   gsm.Message = gsm.loadMsg("Đã cập nhập thông tin nạp thẻ.");
                    string telco = telcoSymbols(gsm.Telco);
                    res.Add(telco, 1);
                }
                gsm = null;
            }
            catch(Exception er)
            {
                res.Add(string.Empty,0);
                this.loadData("update client amount"+er.Message);
                LoggerManager.LogError(er.Message);
            }
            res.Add(string.Empty, 0);
            return res;
        }
        public void sendUpdateClientAmountRes(int num, string phone,string telco)
        {
            string res = "";
            Guid guid = Guid.NewGuid();
            DateTime datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
            string date_time_vietnam = datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss");
            if (num == 1)
            {
                res = $"{{\"command\":\"UPDATE_CLIENT_AMOUNT_INFO_RESPONSE_ACTION\",\"payload\":{{\"success\":true,\"message\":\"Lưu dữ liệu thành công\",\"extra_data\":{{\"phone\":\"{phone}\",\"telecom\":\"{telco}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{date_time_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
            }
            else
            {
                res = $"{{\"command\":\"UPDATE_CLIENT_AMOUNT_INFO_RESPONSE_ACTION\",\"payload\":{{\"success\":false,\"message\":\"Không tìm thấy sđt này.\",\"extra_data\":{{\"phone\":\"{phone}\",\"telecom\":\"{telco}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{date_time_vietnam}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
            }
            var json_ob = JsonConvert.DeserializeObject(res);
            string send_res = JsonConvert.SerializeObject(json_ob,Formatting.Indented);
            this.login_instance.wsHelper.sendDataToServer(send_res);
            LoggerManager.LogTrace(send_res);
        }
        public async Task<int> reportOrderHandling(string phone, string card_serial, string card_code, string card_amount,string task_id)
        {
            int res = -1;
            try
            {
                if (Properties.Settings.Default.reportRechargeOrderList != null)
                {
                    foreach (var report in Properties.Settings.Default.reportRechargeOrderList)
                    {
                        var report_detail = JsonConvert.DeserializeObject<ResponseReportRechargeOrderList>(report);
                        string phone_report = report_detail.Payload.Extra_Data.Phone;
                        string card_serial_report = report_detail.Payload.Extra_Data.Card_Serial;
                        string card_code_report = report_detail.Payload.Extra_Data.Card_Code;
                        string card_amount_report = report_detail.Payload.Extra_Data.Card_Amount;
                        string task_id_report = report_detail.Payload.Extra_Data.Task_Id;
                        if ((phone_report.Equals(phone) && card_serial_report.Equals(card_serial) && card_amount_report.Equals(card_amount) && card_code_report.Equals(card_code)) || (task_id.Equals(task_id_report)))
                        {
                            res = 1;
                            login_instance.wsHelper.sendDataToServer(report);
                            return res;
                        }
                        await Task.Delay(100);
                    }
                }
                Guid guid = Guid.NewGuid();
                string reply = $"{{\"command\":\"REPORT_RECHARGE_ORDER_RESPONSE_ACTION\",\"payload\":{{\"success\":false,\"message\":\"Không tìm thấy thẻ.\",\"extra_data\":{{\"phone\":\"{phone}\",\"card_serial\":\"{card_serial}\",\"card_code\":\"{card_code}\",\"card_amount\":\"{card_amount}\",\"task_id\":\"{task_id}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                var json_ob = JsonConvert.DeserializeObject(reply);
                var res_rep = JsonConvert.SerializeObject(json_ob, Formatting.Indented);
                login_instance.wsHelper.sendDataToServer(res_rep);
            }
            catch (Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
            return res;
        }
        private async Task<int> countAvailableSim()
        {
            int num_of_port = 0;
            foreach(DataGridViewRow row in dataGSM.Rows)
            {
                if (!string.IsNullOrEmpty(row.Cells["sdt"].Value.ToString()))
                {
                    num_of_port++;
                    await Task.Delay(100);
                }
            }
            return num_of_port;
        }
        private void loadPortsBtn_Click(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            int port_nums = ports.Length;
            blackListPort = blackListPort.ToUpper();
            string[] values = blackListPort.Split(',');
            if (port_nums == 0)
            {   if (this.login_instance.language == "English")
                {
                    MessageBox.Show("No port detected.", "No port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (this.login_instance.language == "中文")
                {
                    MessageBox.Show("不发现端口.", "没有端口", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Không phát hiện cổng trên thiết bị.", "Không phát hiện cổng", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                return;
            }
            for (int i = 0; i < ports.Length; i++)
            {
                if (values.Contains(ports[i]))
                {
                    port_nums--;
                }
            }
            if (this.login_instance.language == "English")
            {
                num_of_port.Text = "Number of port:" + port_nums.ToString() + " port";
            }
            else if (this.login_instance.language == "中文")
            {
                num_of_port.Text = "端口数量:" + port_nums.ToString() + "端口";

            }
            else
            {
                num_of_port.Text = "Số cổng:" + port_nums.ToString() + " cổng";
            }
            if (port_nums != new_port_num)
            {
                this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                {
                    dataGSM.Rows.Clear();
                    dataGSM.Refresh();
                }));
                new_port_num = port_nums;
                Task.Run((async () =>
                {
                    await Task.Delay(1200);
                    for (int i = 0; i < ports.Length; i++)
                    {
                        string port = ports[i];
                        if (!values.Contains(port))
                        {
                            try
                            {
                                Form1.ReferenceStart reference = new Form1.ReferenceStart(this.runOpen);
                                this.Invoke(reference, port);
                                reference = null;
                            }
                            catch (Exception er)
                            {
                                Console.WriteLine(er);
                            }
                            port = null;
                        }
                        else
                        {
                            foreach (DataGridViewRow row in dataGSM.Rows)
                            {
                                if(values.Contains(row.Cells["port"].Value.ToString()))
                                {
                                    dataGSM.Rows.RemoveAt(row.Index);
                                }
                            }
                        }
                    }
                    ports = null;
                }
                  )).ContinueWith(x => is_load_port_activated = true);
            }
        }
        public delegate void ReferenceStart(string port);

        public async Task initWebView()
        {
            await webView21.EnsureCoreWebView2Async(null);
        }
        public async void initWebBrowser()
        {
            await initWebView();
            webView21.CoreWebView2.Navigate("https://docs.google.com/document/d/1BPjAlXDB1afGUAWJGmgVjVKgtBnMzA8F/edit?usp=sharing&ouid=100722237979935794478&rtpof=true&sd=true");
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            updateSetting();
            updateCom();
            handlingServerEvent();
            removeAllExpireDate();
            initWebBrowser();
            Assembly assembly = Assembly.GetEntryAssembly();
            Version version = assembly.GetName().Version;
            loadAbout(version.ToString());
            if (this.login_instance.language == "中文")
            {
                this.version.Text = "版本:" + version.ToString();

            }
            else {
                this.version.Text = "Version:" + version.ToString();
                 }
            this.pc_id_label.Text = "Pc_Id:"+this.login_instance.pc_id_value;
            this.statusFilter.SelectedIndex = 0;
            Task.Run(async() =>
            {
            connection_status:
                if (login_instance.wsHelper.is_connect)
                {
                    Image green_dot = Properties.Resources.green_dot;
                    connection_state.Image = green_dot;
                }
                else
                {
                    Image red_dot = Properties.Resources.red_dot;
                    connection_state.Image = red_dot;
                }
                await Task.Delay(5000);
                goto connection_status;
            });
        }
        public void runOpen(string port)
        {
            try
            {
                GsmObject ob = new GsmObject(port);
                loadData("Đã mở cổng " + port);
            }
            catch(Exception er)
            {
                this.loadData(er.Message);
            }
        }

        private void checkPortsBtn_Click(object sender, EventArgs e)
        {
            setupPortSetting();
            int row_count = dataGSM.RowCount;
            if (row_count < new_port_num)
            {
                return;
            }
            if (is_load_port_activated)
            {
                if (!openPort)
                {
                    Task.Run(async () =>
                    {
                        this.checkPortsBtn.Enabled = false;
                        for (int i = 0; i < dataGSM.RowCount; i++)
                        {
                            string port_name = dataGSM.Rows[i].Cells[1].Value.ToString();
                            PortName pn = new PortName(port_name, dataGSM.Rows[i]);
                            PortHandling(pn);
                            await Task.Delay(200);
                        }
                    }
                    ).ContinueWith(t => { this.checkPortsBtn.Enabled = true; });
                    Task.Run(async () =>
                    {
                    count_num_sim:
                        num_of_sim = await countAvailableSim();
                        await Task.Delay(15000);
                        goto count_num_sim;
                    });
                    Task.Run(async () =>
                    {   while(num_of_sim==0)
                        {
                            await Task.Delay(100);
                        }
                        if (num_of_sim != 0)
                        {
                            DateTime sim_delay = DateTime.Now;
                            while (phone_list.Count != num_of_sim)
                            {
                                if (DateTime.Now.Subtract(sim_delay).TotalMinutes > 3)
                                {
                                    break;
                                }
                                else
                                {
                                    await Task.Delay(100);
                                }
                                
                            }
                         await Task.Delay(60000);
                         foreach(string phone in phone_list)
                         {
                                temp_phone_list.Add(phone);
                         }
                            await Task.Delay(1500);
                            string res =pushListSimInfo();
                            if (!string.IsNullOrEmpty(res))
                            {   if (res.Equals("-1"))
                                {
                                    string err_modem = "";
                                    string err_title = "";
                                    if(this.login_instance.language=="English")
                                    {
                                        err_modem = "This application only supports modems use chip QUECTEL.Please change modem which use chip Quectel to contiunue using this application.";
                                        err_title = "Incompatible Modem";
                                    }
                                    else if(this.login_instance.language=="中文")
                                    {
                                        err_modem = "此应用只支持给QUECTEL芯片的调制解调器.请你更换使用QUECTEL芯片的调制解调器来继续使用应用.";
                                        err_title = "调制解调器不相容";
                                    } 
                                    else
                                    {
                                       err_modem= "Phần mềm chỉ hỗ trợ cho các modem sử dụng dòng chip Quectel.Vui lòng đổi sang modem có dòng chip Quectel để tiếp tục sử dụng.";
                                       err_title = "Modem không tương thích";
                                    }
                                    DialogResult dialog_modem = MessageBox.Show(err_modem,err_title, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    if(dialog_modem==DialogResult.OK) 
                                    {
                                        Application.Exit();
                                    }
                                }
                                else
                                {   
                                    this.login_instance.wsHelper.sendDataToServer(res);
                                    LoggerManager.LogTrace(res);
                                }
                            }
                        add_new_sim:
                            int res_handling = await updateNewSim();
                            await Task.Delay(5000);
                            goto add_new_sim;
                        }
                    });
                    openPort = true;
                }
                else
                {
                    foreach (DataGridViewRow row in dataGSM.Rows)
                    {
                        string port = row.Cells[1].Value.ToString();
                        row.Cells["status"].Style.BackColor = Color.White;
                        GsmObject ob = gsmObject.SingleOrDefault(t => t.Port == port);
                        if (ob == null)
                        {
                            return;
                        }
                        if (!ob.sp.IsOpen)
                        {
                            ob.sp.Open();
                        }
                        ob.reset(port, row);
                    }
                }
            }
            else
            { 
                if (this.login_instance.language == "English")
                {
                    MessageBox.Show("Load the port first before checking", "Check port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (this.login_instance.language == "中文")
                {
                    MessageBox.Show("先加载端口", "检查端口", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Không phát hiện cổng trên thiết bị.", "Không phát hiện cổng", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }
        public string findTaskId(string phone,string card_serial,string card_code,string card_amount)
        {
            string task_id = "";
            try
            {
                foreach(var card in Properties.Settings.Default.rechargeOrderList)
                {
                    var recharge_order = JsonConvert.DeserializeObject<RechargeOrder>(card);
                    string phone_order = recharge_order.Payload.Phone;
                    string card_serial_order = recharge_order.Payload.Card_Serial;
                    string card_code_order = recharge_order.Payload.Card_Code;
                    string card_amount_order = recharge_order.Payload.Card_Amount;
                    string task_id_order = recharge_order.Payload.Task_Id;
                    if(phone_order.Equals(phone)&&card_serial_order.Equals(card_serial)&&card_code_order.Equals(card_code)&&card_amount_order.Equals(card_amount))
                    {
                        task_id = task_id_order;
                        return task_id;
                    }
                }
            }
            catch(Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
            return task_id;
        }
        private void transactionAllHistoryUpdateSpecialList(System.Collections.Specialized.StringCollection data)
        {
            this.transactionGridView.BeginInvoke(new MethodInvoker(() =>
            {
                try
                {
                    if (data != null)
                    {
                        foreach (var transaction in data)
                        {
                            int index = this.transactionGridView.Rows.Add();
                            var transaction_detail = JsonConvert.DeserializeObject<ResponseReportRechargeOrderList>(transaction);
                            string phone_order = transaction_detail.Payload.Extra_Data.Phone;
                            string before_balance = transaction_detail.Payload.Extra_Data.Extra_Data.Before_Balance;
                            string after_balance = transaction_detail.Payload.Extra_Data.Extra_Data.After_Balance;
                            string date_order = transaction_detail.Trace_Time;
                            string card_amount_order = transaction_detail.Payload.Extra_Data.Card_Amount;
                            string card_real_amount_order = transaction_detail.Payload.Extra_Data.Extra_Data.Card_Real_Amount;
                            string status_order = transaction_detail.Payload.Extra_Data.Status;
                            string status = "";
                            string msg_response = transaction_detail.Payload.Extra_Data.Ussd_Message;
                            string network = transaction_detail.Payload.Extra_Data.Network;
                            if (status_order == "3")
                            {
                                if (login_instance.language == "English")
                                {
                                    status = "Failed";
                                }
                                else if (login_instance.language == "中文")
                                {
                                    status = "失败";
                                }
                                else
                                {
                                    status = "Thất bại";
                                }
                            }
                            else if (status_order == "2")
                            {
                                if (login_instance.language == "English")
                                {
                                    status = "Success";
                                }
                                else if (login_instance.language == "中文")
                                {
                                    status = "成功";
                                }
                                else
                                {
                                    status = "Thành công";
                                }

                            }
                            else
                            {
                                if (login_instance.language == "English")
                                {
                                    status = "Undetermined";
                                }
                                else if (login_instance.language == "中文")
                                {
                                    status = "不确定";
                                }
                                else
                                {
                                    status = "Không xác định";
                                }
                            }
                            string card_serial_order = transaction_detail.Payload.Extra_Data.Card_Serial;
                            string card_code_order = transaction_detail.Payload.Extra_Data.Card_Code;
                            string task_id_order = transaction_detail.Payload.Extra_Data.Task_Id;
                            string sim_type_order = transaction_detail.Payload.Extra_Data.Sim_Type;
                            string sms_response = transaction_detail.Payload.Extra_Data.Extra_Data.Sms_Time;
                            this.transactionGridView.Rows[index].Cells["phone_trans"].Value = phone_order;
                            this.transactionGridView.Rows[index].Cells["charge_before"].Value = before_balance;
                            this.transactionGridView.Rows[index].Cells["charge_after"].Value = after_balance;
                            this.transactionGridView.Rows[index].Cells["time_sms"].Value = sms_response;
                            this.transactionGridView.Rows[index].Cells["charged_time"].Value = date_order;
                            this.transactionGridView.Rows[index].Cells["status_charge"].Value = status;
                            this.transactionGridView.Rows[index].Cells["money_topup"].Value = card_amount_order;
                            this.transactionGridView.Rows[index].Cells["real_topup"].Value = card_real_amount_order;
                            this.transactionGridView.Rows[index].Cells["task_id"].Value = task_id_order;
                            this.transactionGridView.Rows[index].Cells["card_code"].Value = card_serial_order;
                            this.transactionGridView.Rows[index].Cells["error_msg"].Value = msg_response;
                            this.transactionGridView.Rows[index].Cells["network_transac"].Value = network;
                            this.transactionGridView.Rows[index].Cells["simtype_transac"].Value = sim_type_order;
                        }
                    }
                }
                catch (Exception er)
                {
                    LoggerManager.LogError(er.Message);
                }
            }));
        }
        private void transactionAllHistoryUpdate(List<string> data)
        {
            this.transactionGridView.BeginInvoke(new MethodInvoker(() =>
            {
                try
                {  if (data != null)
                    {   
                        foreach (var transaction in data)
                        {
                            int index = this.transactionGridView.Rows.Add();
                            var transaction_detail = JsonConvert.DeserializeObject<ResponseReportRechargeOrderList>(transaction);
                            string phone_order = transaction_detail.Payload.Extra_Data.Phone;
                            string before_balance = transaction_detail.Payload.Extra_Data.Extra_Data.Before_Balance;
                            string after_balance = transaction_detail.Payload.Extra_Data.Extra_Data.After_Balance;
                            string date_order = transaction_detail.Trace_Time;
                            string card_amount_order = transaction_detail.Payload.Extra_Data.Card_Amount;
                            string card_real_amount_order = transaction_detail.Payload.Extra_Data.Extra_Data.Card_Real_Amount;
                            string status_order = transaction_detail.Payload.Extra_Data.Status;
                            string status = "";
                            string msg_response = transaction_detail.Payload.Extra_Data.Ussd_Message;
                            string network = transaction_detail.Payload.Extra_Data.Network;
                            string sim_time = transaction_detail.Payload.Extra_Data.Extra_Data.Sms_Time;
                            if (status_order == "3")
                            {
                                if (login_instance.language == "English")
                                {
                                    status = "Failed";
                                }
                                else if (login_instance.language == "中文")
                                {
                                    status = "失败";
                                }
                                else
                                {
                                    status = "Thất bại";
                                }
                            }
                            else if(status_order=="2")
                            {
                                if (login_instance.language == "English")
                                {
                                    status = "Success";
                                }
                                else if (login_instance.language == "中文")
                                {
                                    status = "成功";
                                    
                                }
                                else
                                {
                                    status = "Thành công";
                                }
                            }
                            else
                            {
                                if (login_instance.language == "English")
                                {
                                    status = "Undetermined";
                                }
                                else if (login_instance.language == "中文")
                                {
                                    status = "不确定";
                                }
                                else
                                {
                                    status = "Không xác định";
                                }
                            }
                            string card_serial_order = transaction_detail.Payload.Extra_Data.Card_Serial;
                            string card_code_order = transaction_detail.Payload.Extra_Data.Card_Code;
                            string task_id_order = transaction_detail.Payload.Extra_Data.Task_Id;
                            string sim_type_order = transaction_detail.Payload.Extra_Data.Sim_Type;
                            this.transactionGridView.Rows[index].Cells["phone_trans"].Value = phone_order;
                            this.transactionGridView.Rows[index].Cells["charge_before"].Value = before_balance;
                            this.transactionGridView.Rows[index].Cells["charge_after"].Value = after_balance;
                            this.transactionGridView.Rows[index].Cells["charged_time"].Value = date_order;
                            this.transactionGridView.Rows[index].Cells["status_charge"].Value = status;
                            this.transactionGridView.Rows[index].Cells["money_topup"].Value = card_amount_order;
                            this.transactionGridView.Rows[index].Cells["real_topup"].Value = card_real_amount_order;
                            this.transactionGridView.Rows[index].Cells["task_id"].Value = task_id_order;
                            this.transactionGridView.Rows[index].Cells["card_code"].Value = card_serial_order;
                            this.transactionGridView.Rows[index].Cells["error_msg"].Value = msg_response;
                            this.transactionGridView.Rows[index].Cells["network_transac"].Value = network;
                            this.transactionGridView.Rows[index].Cells["simtype_transac"].Value = sim_type_order;
                            this.transactionGridView.Rows[index].Cells["time_sms"].Value = sim_time;
                            
                        }
                    }
                }
                catch(Exception er)
                {
                    LoggerManager.LogError(er.Message);
                }
            }));
        }
      
        public void transactionUpdate()
        {
            this.transactionGridView.BeginInvoke(new MethodInvoker(() =>
            {
                try
                {
                    int index = this.transactionGridView.Rows.Add();
                    var transaction = Properties.Settings.Default.reportRechargeOrderList[Properties.Settings.Default.reportRechargeOrderList.Count - 1];
                    current_transaction_list.Add(transaction);
                    var transaction_detail = JsonConvert.DeserializeObject<ResponseReportRechargeOrderList>(transaction);
                    string phone_order = transaction_detail.Payload.Extra_Data.Phone;
                    string before_balance = transaction_detail.Payload.Extra_Data.Extra_Data.Before_Balance;
                    string after_balance = transaction_detail.Payload.Extra_Data.Extra_Data.After_Balance;
                    string date_order = transaction_detail.Trace_Time;
                    string card_amount_order = transaction_detail.Payload.Extra_Data.Card_Amount;
                    string card_real_amount_order = transaction_detail.Payload.Extra_Data.Extra_Data.Card_Real_Amount;
                    string status_order = transaction_detail.Payload.Extra_Data.Status;
                    string status = "";
                    string mess_info = transaction_detail.Payload.Extra_Data.Ussd_Message;
                    string network = transaction_detail.Payload.Extra_Data.Network;
                    string sim_time = transaction_detail.Payload.Extra_Data.Extra_Data.Sms_Time;
                    if (status_order == "3")
                    {  if (login_instance.language == "English")
                        {
                            status = "Failed";
                        }
                        else if (login_instance.language == "中文")
                        {
                            status = "失败";
                        }
                        else
                        {
                            status = "Thất bại";
                        }
                    }
                    else if(status_order == "2")
                    {
                        if (login_instance.language == "English")
                        {
                            status = "Success";
                        }
                        else if (login_instance.language == "中文")
                        {
                            status = "成功";
                        }
                        else
                        {
                            status = "Thành công";
                        }
                    }
                    else
                    {
                        if (login_instance.language == "English")
                        {
                            status = "Undetermined";
                        }
                        else if (login_instance.language == "中文")
                        {
                            status = "不确定";
                        }
                        else
                        {
                            status = "Không xác định";
                        }
                    }
                    string card_serial_order = transaction_detail.Payload.Extra_Data.Card_Serial;
                    string card_code_order = transaction_detail.Payload.Extra_Data.Card_Code;
                    string task_id_order = transaction_detail.Payload.Extra_Data.Task_Id;
                    string sim_type_order = transaction_detail.Payload.Extra_Data.Sim_Type;
                    this.transactionGridView.Rows[index].Cells["phone_trans"].Value = phone_order;
                    this.transactionGridView.Rows[index].Cells["charge_before"].Value = before_balance;
                    this.transactionGridView.Rows[index].Cells["charge_after"].Value = after_balance;
                    this.transactionGridView.Rows[index].Cells["charged_time"].Value = date_order;
                    this.transactionGridView.Rows[index].Cells["status_charge"].Value = status;
                    this.transactionGridView.Rows[index].Cells["money_topup"].Value = card_amount_order;
                    this.transactionGridView.Rows[index].Cells["real_topup"].Value = card_real_amount_order;
                    this.transactionGridView.Rows[index].Cells["task_id"].Value = task_id_order;
                    this.transactionGridView.Rows[index].Cells["card_code"].Value = card_serial_order;
                    this.transactionGridView.Rows[index].Cells["error_msg"].Value = mess_info;
                    this.transactionGridView.Rows[index].Cells["network_transac"].Value = network;
                    this.transactionGridView.Rows[index].Cells["simtype_transac"].Value = sim_type_order;
                    this.transactionGridView.Rows[index].Cells["time_sms"].Value = sim_time;
                }
                catch(Exception er)
                {
                    LoggerManager.LogError(er.Message);
                }
            }));
        }
        public void PortHandling(object state)
        {
            PortName pn = (PortName)state;
            Task.Run(async () =>
            {
                await checkLock.WaitAsync();
                try
                {
                    string port = pn.port;
                    try
                    {
                        Form1.Reference reference = new Form1.Reference(this.run);
                        this.Invoke(reference, port, pn.k);
                    }
                    catch (Exception er)
                    {
                       loadData("port handling:"+er.Message);
                    }
                    port = null;
                    pn = null;
                }
                finally
                {
                    checkLock.Release();
                }
            }
            );
        }
        public delegate void Reference(string x, DataGridViewRow y);
        public void run(string port, DataGridViewRow i)
        {
            try
            {
                GsmObject obj = new GsmObject(port, i);
                gsmObject.Add(obj);
                this.loadData("Đã lấy thành công thông tin cổng " + port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public class PortName
        {
            public string port { get; set; }
            public DataGridViewRow k { get; set; }
            public PortName(string p, DataGridViewRow i)
            {
                this.port = p;
                this.k = i;
            }
        }
        public string simDataPattern(SimInfo sim)
        {
            string res = "";
            if (sim != null) 
            {
                res = $"{{\"com_port\":\"{sim.Port}\",\"phone\":\"{sim.Phone}\",\"telecom\":\"{sim.Telecom}\",\"phone_type\":\"{sim.PhoneType}\",\"balance\":\"{sim.Balance}\",\"expires_at\":\"{sim.Expire}\",\"status\":\"{sim.Status}\"}}";
            }
            
            return res;
        }
        private string telcoSymbols(string telco)
        {
            switch(telco)
            {
                case "VIETTEL":return "VTT";
                case "MOBIFONE":return "VMS";
                case "VINAPHONE":return "VNP";
                case "VIETNAMOBILE":return "VMN";
            }
            return "";          
        }
        public string balance_standard(string balance)
        {
            string res = balance;
            if (balance.Contains("₫"))
            {
                balance = balance.Replace("₫", "").Trim();
            }
            if (balance.Contains(",") && !balance.Contains("."))
            {
                string[] data_balance_split = balance.Split(',');
                string tail_data_balance = data_balance_split[1];
                string data_balance = "";
                if (tail_data_balance == "00" || tail_data_balance == "00 ")
                {
                    data_balance = data_balance_split[0];
                }
                else
                {
                    data_balance = balance.Replace(",", "");
                }
                res = data_balance;
            }
            else if (balance.Contains(".") && !balance.Contains(","))
            {
                string[] data_balance_split = balance.Split('.');
                string tail_balance = data_balance_split[1];
                string data_balance = "";
                if (tail_balance == "00" || tail_balance == "00 ")
                {
                    data_balance = data_balance_split[0];
                }
                else
                {
                    data_balance = balance.Replace(".", "");
                }
                res = data_balance;
            }
            else if (balance.Contains(".") && balance.Contains(","))
            {
                int index_dot = balance.IndexOf(".");
                int index_colon = balance.IndexOf(",");
                string data_balance = "";
                if (index_dot < index_colon)
                {
                    string[] data_balance_split = balance.Split(',');
                    data_balance = data_balance_split[0].Replace(".", "");
                }
                else
                {
                    string[] data_balance_split = balance.Split('.');
                    data_balance = data_balance_split[0].Replace(",", "");
                }
                res = data_balance;
            }
           if (string.IsNullOrEmpty(balance))
            {
                res = "0";
            }
            return res;
        }
        private string pushListSimInfo()
        {
            int count_quectel = 0;
            string res = $"{{\"command\":\"PUSH_SIM_PORT_LIST_ACTION\",\"payload\":[";
           
            foreach(DataGridViewRow row in dataGSM.Rows)
            {
                string telco = telcoSymbols(row.Cells["telco"].Value.ToString());
                if (!string.IsNullOrEmpty(telco))
                {   
                    string phone = row.Cells["sdt"].Value.ToString();
                    string port_com = row.Cells["port"].Value.ToString();
                    string phone_type = row.Cells["simtype"].Value.ToString();
                    string expire = row.Cells["expire"].Value.ToString();
                    string balance = row.Cells["tkc"].Value.ToString().Trim();
                    string data_balance = balance_standard(balance);
                    string modem = row.Cells["modem"].Value.ToString().Trim();
                    if(!string.IsNullOrEmpty(phone) && modem.Contains("QUECTEL"))
                    {
                        count_quectel++;
                    }
                    if (!string.IsNullOrEmpty(data_balance))
                    {
                        balance = data_balance;
                    }
                    string phone_status = "";
                    Regex reg = new Regex("[^0-9]");
                    string phone_format = reg.Replace(phone, "");
                    if (string.IsNullOrEmpty(phone_format))
                    {
                        phone_status = "1";
                    }
                    else
                    {
                        phone_status = "2";
                    }
                    SimInfo sim = new SimInfo(phone, port_com, telco, phone_type, balance, expire, phone_status);
                    string sim_res = simDataPattern(sim);
                    res += sim_res + ",";
                }
            }
            if (count_quectel == 0)
            {
                return "-1";
            }
            else
            {
                Guid guid = Guid.NewGuid();
                DateTime datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                res += $"],\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                var res_ob = JsonConvert.DeserializeObject(res);
                res = JsonConvert.SerializeObject(res_ob, Formatting.Indented);
                LoggerManager.LogInfo(res);
            }
            return res;
        }
        public string pushSingleSimInfo(string phone,int status_push)
        {
            try
            {
                string res = $"{{\"command\":\"PUSH_SIM_PORT_LIST_ACTION\",\"payload\":[";
                foreach(DataGridViewRow row in dataGSM.Rows)
                {
                    string phone_push = row.Cells["sdt"].Value.ToString();
                    if(phone_push.Equals(phone))
                    {
                        string telco = telcoSymbols(row.Cells["telco"].Value.ToString());
                        if (!string.IsNullOrEmpty(telco))
                        {
                            string port_com = row.Cells["port"].Value.ToString();
                            string phone_type = row.Cells["simtype"].Value.ToString();
                            if(string.IsNullOrEmpty(phone_type))
                            {
                                phone_type = "TT";
                            }
                            string expire = row.Cells["expire"].Value.ToString();
                            string balance = (row.Cells["tkc"].Value.ToString());
                            string data_balance = balance_standard(balance);
                            if (!string.IsNullOrEmpty(data_balance))
                            {
                                balance = data_balance;
                            }
                            string phone_status = "";
                            if (status_push == 0)
                            {
                                phone_status = "0";
                            }
                            else if(status_push==1)
                            {
                                Regex reg = new Regex("[^0-9]");
                                string phone_format = reg.Replace(phone, "");
                                if (string.IsNullOrEmpty(phone_format))
                                {
                                    phone_status = "1";
                                }
                                else
                                {
                                    phone_status = "2";
                                }
                            }
                            SimInfo sim = new SimInfo(phone, port_com, telco, phone_type, balance, expire, phone_status);
                            string sim_res = simDataPattern(sim);
                            res += sim_res;
                        }
                        break;
                    }

                }
                Guid guid = Guid.NewGuid();
                DateTime datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                res += $"],\"trace_id\":\"{guid}\",\"trace_time\":\"{datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
                var res_ob = JsonConvert.DeserializeObject(res);
                res = JsonConvert.SerializeObject(res_ob, Formatting.Indented);
                LoggerManager.LogInfo("push single sim:" + res);
                return res;
            }
            catch(Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
            string res_false = $"{{\"command\":\"PUSH_SIM_PORT_LIST_ACTION\",\"payload\":{{\"status\":\"false\",\"message\":\"Không tìm thấy sđt\"}}";
            Guid guidd = Guid.NewGuid();
            DateTime datetime_vietnam_zone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
            res_false += $",\"trace_id\":\"{guidd}\",\"trace_time\":\"{datetime_vietnam_zone.ToString("dd/MM/yyyy HH:mm:ss")}\",\"trace_side\":\"cs\",\"min_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_MIN_VERSION")}\",\"current_version_code\":\"{Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION")}\",\"current_agent_version\":\"{Environment.GetEnvironmentVariable("AGENT_VERSION")}\"}}";
            var res_obb = JsonConvert.DeserializeObject(res_false);
            res_false = JsonConvert.SerializeObject(res_obb, Formatting.Indented);
            return res_false;
        }
        public async Task<int> updateNewSim()
        {
            int res = 0;
            try
            {
                foreach(string phone in phone_list)
                {
                    if(!temp_phone_list.Contains(phone))
                    {
                        await Task.Delay(1000);
                        DateTime time_out = DateTime.Now;
                        var gsm = gsmObject.SingleOrDefault(p => p.Phone == phone);
                        if(gsm!=null)
                        {
                            while(string.IsNullOrEmpty(gsm.SimType) && DateTime.Now.Subtract(time_out).TotalMinutes<1)
                            {
                                await Task.Delay(100);
                            }
                            gsm = null;
                        }
                        string new_sim_res = pushSingleSimInfo(phone, 1);
                        if(!string.IsNullOrEmpty(new_sim_res))
                        {
                            this.login_instance.wsHelper.sendDataToServer(new_sim_res);
                        }
                        temp_phone_list.Add(phone);
                        await Task.Delay(100);
                    }
                }
                res = 1;
                return res;
            }
            catch(Exception er)
            {
                this.loadData("update new sim:" + er.Message);
                LoggerManager.LogError("update sim:"+er.Message);
            }
            return res;
        }
        public void removeAllExpireDate()
        {
            try
            {
                if (Properties.Settings.Default.reportRechargeOrderList != null)
                {
                    for(int i=Properties.Settings.Default.reportRechargeOrderList.Count-1;i>=0;i--)
                    {
                        var report_detail = JsonConvert.DeserializeObject<ResponseReportRechargeOrderList>(Properties.Settings.Default.reportRechargeOrderList[i]);
                        string report_date = report_detail.Trace_Time;
                        try
                        {

                            DateTime convert_report_date = DateTime.Parse(report_date);

                            if (DateTime.Now.Subtract(convert_report_date).TotalDays >= 5)
                            {
                                Properties.Settings.Default.reportRechargeOrderList.Remove(Properties.Settings.Default.reportRechargeOrderList[i]);
                            }
                        }
                        catch(Exception err)
                        {
                            LoggerManager.LogError("parse datetime:" + err.Message);
                            Properties.Settings.Default.reportRechargeOrderList.Remove(Properties.Settings.Default.reportRechargeOrderList[i]);
                        }
                    }
                    Properties.Settings.Default.Save();
                }
            }
            catch(Exception er)
            {
                LoggerManager.LogError("remove_expire_date"+er.Message);
            }
        }
        
        public void sendMockPattern()
        {
            SimInfo sim = new SimInfo("0123456789", "COM1", "VIETTEL", "TT", "20000", "22-2-2013", "2");
            string res = simDataPattern(sim);
            this.login_instance.wsHelper.sendDataToServer(res);
        }
        private void deleteAllHistoryTransaction()
        {
            try
            { 
               if(Properties.Settings.Default.reportRechargeOrderList.Count>0)
                {
                Properties.Settings.Default.reportRechargeOrderList.Clear();
                }
            }
            catch(Exception er)
            {
                LoggerManager.LogError("delete_history"+er.Message);
            }
        }
        private void deleteAllEmptyNoteTransaction()
        {
            try
            { 
             if(Properties.Settings.Default.reportRechargeOrderList.Count>0)
                {
                  for(int i=Properties.Settings.Default.reportRechargeOrderList.Count-1;i>=0;i--)
                    {
                        var report_detail = JsonConvert.DeserializeObject<ResponseReportRechargeOrderList>(Properties.Settings.Default.reportRechargeOrderList[i]);
                        string note = report_detail.Payload.Extra_Data.Ussd_Message;
                        if(string.IsNullOrEmpty(note))
                        {
                            Properties.Settings.Default.reportRechargeOrderList.Remove(Properties.Settings.Default.reportRechargeOrderList[i]);
                        }
                    }
                    Properties.Settings.Default.Save();
                }
            }
            catch(Exception er)
            {
                LoggerManager.LogError("deleteEmtyNote:"+er.Message);
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {   
            if(e.CloseReason==CloseReason.UserClosing||e.CloseReason==CloseReason.TaskManagerClosing||e.CloseReason==CloseReason.WindowsShutDown||e.CloseReason==CloseReason.FormOwnerClosing)
            {
                bool can_close = true;
                foreach(DataGridViewRow row in dataGSM.Rows)
                {
                    string status = row.Cells["status"].Value.ToString();
                    if(status=="Đang xử lý")
                    {
                        can_close = false;
                        break;
                    }
                }
                if(!can_close)
                {
                    LoggerManager.LogTrace("Không thể thoát chương trình.");
                    MessageBox.Show("Không thể thoát chương trình trong quá trình nạp thẻ.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
                else
                {
                    LoggerManager.LogTrace("Thoát chương trình");
                    LoggerManager.LogConnectTrace("Application Exit.");
                    e.Cancel = false;
                    Environment.Exit(Environment.ExitCode);
                }
            }
        }

        private void dataGSM_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            this.dataGSM.Rows[e.RowIndex].Cells[0].Value = e.RowIndex + 1;
        }

        private void dataGSM_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if(e.Column.Index==1)
            {
                int n1 = int.Parse(e.CellValue1.ToString().Replace("COM", ""));
                int n2 = int.Parse(e.CellValue2.ToString().Replace("COM", ""));
                e.SortResult = n1.CompareTo(n2);
                e.Handled = true;
            }
        }
        public void loadData(string message)
        {
            lock(this.lockLoadData)
            {
                
                    this.logTxtBox.BeginInvoke(new MethodInvoker(() =>
                {
                    try
                    {
                        this.logTxtBox.Text = this.logTxtBox.Text + DateTime.Now + "=>" + message + Environment.NewLine;
                        this.logTxtBox.SelectionStart = this.logTxtBox.TextLength;
                        this.logTxtBox.ScrollToCaret();
                    }
                   catch(Exception e)
                    {
                        LoggerManager.LogError(e.Message);
                    }
                }));
               
            }
                
        }
        public void loadAbout(string version)
        {
            
            this.aboutTxTBox.BeginInvoke(new MethodInvoker(() =>
            {
                string header = "Phần mềm này được cung cấp miễn phí cho ACE sử dụng nạp thẻ cào"+Environment.NewLine;
                string ver_info = $"Phiên bản hiện tại:{version}"+Environment.NewLine;
                string info = "Sử dụng tốt nhất trên:\r\n-Máy tính: Windows 10 trở lên, RAM 4GB trở lên, ổ cứng SSD\r\n- GSM Modems: Chip Quectel M26/ EC20 \r\n- SIMBank: (liên hệ quản lý để được hỗ trợ) \r\n";
                this.aboutTxTBox.Text = header + ver_info + info;            
            }));
        }
        private void num_of_port_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void transactionGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
          
        }

        private void settingBtn_Click_1(object sender, EventArgs e)
        {
            Setting setting = new Setting();
            setting.ShowDialog();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (!openPort)
            {
                if (this.login_instance.language == "English")
                {
                    MessageBox.Show("There is error while saving port setting", "Saving port setting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (this.login_instance.language == "中文")
                {
                    MessageBox.Show("保存端口设置发生错误", "保存端口设置", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Lỗi lưu cấu hình", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                return;
            }
            updatePortSetting(dataGSM);
            if (this.login_instance.language == "English")
            {
                MessageBox.Show("Saving port setting successfully", "Saving port setting", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (this.login_instance.language == "中文")
            {
                MessageBox.Show("保存端口设置成功", "保存端口设置", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Đã lưu cấu hình port", "Lưu cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {

        }

        private void copySĐTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                DataGridViewRow row = dataGSM.CurrentRow;
                if (row != null)
                {
                    string phone = "";
                    try
                    {
                        phone = row.Cells["sdt"].Value.ToString();
                    }
                    catch (Exception er)
                    {
                        LoggerManager.LogError("copy row null:" + er.Message);
                        this.loadData("copy row null:" + er.Message);
                    }
                    if (string.IsNullOrEmpty(phone))
                    {
                        if (this.login_instance.language == "English")
                        {
                            MessageBox.Show("Cannot detect phone number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (this.login_instance.language == "中文")
                        {
                            MessageBox.Show("不发现电话信号", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Cồng này chưa có sim hoặc sim chưa có sóng.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return;
                    }
                    Clipboard.SetDataObject(phone);
                    row = null;
                    if (this.login_instance.language == "English")
                    {
                        MessageBox.Show("Copy phone number successfully", "Copy phone number", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (this.login_instance.language == "中文")
                    {
                        MessageBox.Show("复制电话成功", "复制电话", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Đã copy số điện thoại.", "Copy thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception er)
            {
                LoggerManager.LogError("Copy port:" + er.Message);
                this.loadData("Copy port:" + er.Message);
            }
        }

        private void tảiLạiToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                DataGridViewRow row = dataGSM.CurrentRow;
                if (row != null)
                {
                    string port = row.Cells[1].Value.ToString();
                    GsmObject gsm = gsmObject.SingleOrDefault(x => x.Port == port);
                    if (gsm != null)
                    {
                        gsm.reset(gsm.Port, gsm.rowGSMSelected);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogError("Reset row:" + ex.Message);
                this.loadData("Reset row:" + ex.Message);
            }
        }

        private void copyDanhSáchCổngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                DataGridViewRow row = dataGSM.CurrentRow;
                if (row != null)
                {
                    string port = "";
                    try
                    {
                        port = row.Cells["port"].Value.ToString();
                    }
                    catch (Exception er)
                    {
                        LoggerManager.LogError(er.Message);
                        this.loadData(er.Message);
                    }
                    if (string.IsNullOrEmpty(port))
                    {  if (this.login_instance.language == "English")
                        {
                            MessageBox.Show("Cannot detect signal for this port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (this.login_instance.language == "中文")
                        {
                            MessageBox.Show("不发现网络信号", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Cồng này chưa có sim hoặc sim chưa có sóng.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return;
                        }

                    Clipboard.SetDataObject(port);
                    row = null;
                    if (this.login_instance.language == "English")
                    {
                        MessageBox.Show("Copy port successfully", "Copy port", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (this.login_instance.language == "中文")
                    {
                        MessageBox.Show("复制端口成功", "复制端口", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Đã copy cổng window.", "Copy thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception er)
            {
                LoggerManager.LogError("Copy port:" + er.Message);
                this.loadData("Copy port:" + er.Message);
            }
        }

        private void exportToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.InitialDirectory = @"D:\GSMInfo.xlsx";
                dialog.Title = "Save GSM Sim InFo To Excel";
                dialog.RestoreDirectory = true;
                dialog.DefaultExt = "xlsx";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                string path = dialog.FileName;
                Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
                app.Interactive = false;
                while(app.Interactive==true)
                {
                    try
                    {
                        app.Interactive = false;
                    }
                    catch(Exception er)
                    {
                        LoggerManager.LogError(er.Message);
                    }
                }
                try
                {
                    Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
                    Microsoft.Office.Interop.Excel._Worksheet worksheet = workbook.ActiveSheet;
                    app.Visible = true;
                    worksheet = workbook.Sheets["Sheet1"];
                    worksheet.Name = "GSM Sim Info";
                    for (int i = 1; i < 14; i++)
                    {
                        worksheet.Cells[1, i] = dataGSM.Columns[i - 1].HeaderText;
                    }
                    for (int i = 0; i < dataGSM.RowCount; i++)
                    {
                        if (dataGSM.Rows[i] != null)
                        {
                            for (int j = 0; j < 13; j++)
                            {
                                if (worksheet != null && dataGSM.Rows[i].Cells[j].Value != null)
                                {
                                    if (j == 7 || j == 8)
                                    {
                                        worksheet.Cells[i + 2, j + 1] = (dataGSM.Rows[i].Cells[j].Value.ToString().Replace(".", "").Replace(",", ""));
                                    }
                                    else
                                    {
                                        worksheet.Cells[i + 2, j + 1] = (dataGSM.Rows[i].Cells[j].Value.ToString());
                                    }
                                }
                            }
                        }
                    }
                    workbook.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    workbook.Close();
                    app.Quit();
                }
              finally
                {
                    app.Interactive = true;
                }
            }
            catch (Exception er)
            {
                this.loadData("excel:"+er.Message);
            }
        }

        private void transactionGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            this.transactionGridView.Rows[e.RowIndex].Cells[0].Value = (e.RowIndex + 1);
        }
        public void filter_first_condition()
        {  if (filter_transaction_list.Count>0)
            {
                List<string> filter = new List<string>();
                foreach (var element in filter_transaction_list)
                {
                    var transaction_detail = JsonConvert.DeserializeObject<ResponseReportRechargeOrderList>(element);
                    string status = transaction_detail.Payload.Extra_Data.Status;
                    if (status == "3")
                    {
                        filter.Add(element);
                    }
                }
                transactionAllHistoryUpdate(filter);
                filter = null;
            }
            else
            {
                List<string> filter = new List<string>();
                foreach (var element in current_transaction_list)
                {
                    var transaction_detail = JsonConvert.DeserializeObject<ResponseReportRechargeOrderList>(element);
                    string status = transaction_detail.Payload.Extra_Data.Status;
                    if (status == "3")
                    {
                        filter.Add(element);
                    }
                }
                transactionAllHistoryUpdate(filter);
                filter = null;
            }
        }
        public void filter_second_condition()
        {
            if (filter_transaction_list.Count > 0)
            {
                List<string> filter = new List<string>();
                foreach (var element in filter_transaction_list)
                {
                    var transaction_detail = JsonConvert.DeserializeObject<ResponseReportRechargeOrderList>(element);
                    string status = transaction_detail.Payload.Extra_Data.Status;
                    if (status == "2")
                    {
                        filter.Add(element);
                    }
                }
                transactionAllHistoryUpdate(filter);
                filter = null;
            }
            else
            {
                List<string> filter = new List<string>();
                foreach (var element in current_transaction_list)
                {
                    var transaction_detail = JsonConvert.DeserializeObject<ResponseReportRechargeOrderList>(element);
                    string status = transaction_detail.Payload.Extra_Data.Status;
                    if (status == "2")
                    {
                        filter.Add(element);
                    }
                }
                transactionAllHistoryUpdate(filter);
                filter = null;
            }
        }
        public void filter_not_confirm_condition()
        {
            if (filter_transaction_list.Count > 0)
            {
                List<string> filter = new List<string>();
                foreach (var element in filter_transaction_list)
                {
                    var transaction_detail = JsonConvert.DeserializeObject<ResponseReportRechargeOrderList>(element);
                    string status = transaction_detail.Payload.Extra_Data.Status;
                    if (status == "1")
                    {
                        filter.Add(element);
                    }
                }
                transactionAllHistoryUpdate(filter);
                filter = null;
            }
            else
            {
                List<string> filter = new List<string>();
                foreach (var element in current_transaction_list)
                {
                    var transaction_detail = JsonConvert.DeserializeObject<ResponseReportRechargeOrderList>(element);
                    string status = transaction_detail.Payload.Extra_Data.Status;
                    if (status == "1")
                    {
                        filter.Add(element);
                    }
                }
                transactionAllHistoryUpdate(filter);
                filter = null;
            }
        }
        public void filter_third_condition(string phone_or_telco_or_task_id)
        {   if (Properties.Settings.Default.reportRechargeOrderList != null)
            {
                System.Collections.Specialized.StringCollection filter = new System.Collections.Specialized.StringCollection();
                foreach (var element in Properties.Settings.Default.reportRechargeOrderList)
                {
                    var transaction_detail = JsonConvert.DeserializeObject<ResponseReportRechargeOrderList>(element);
                    string phone = transaction_detail.Payload.Extra_Data.Phone;
                    string taskId = transaction_detail.Payload.Extra_Data.Task_Id;
                    string network = transaction_detail.Payload.Extra_Data.Network;
                    if (taskId == phone_or_telco_or_task_id || phone==phone_or_telco_or_task_id || network==phone_or_telco_or_task_id.ToUpper())
                    {
                        filter_transaction_list.Add(element);
                        filter.Add(element);
                    }
                }
                transactionAllHistoryUpdateSpecialList(filter);
                filter = null;
            }
        }
     
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.transactionGridView.BeginInvoke(new MethodInvoker(() =>
        {
            int status_selected = this.statusFilter.SelectedIndex;
            if(this.transactionGridView.RowCount>0)
            {
                this.transactionGridView.Rows.Clear();
                this.transactionGridView.Refresh();
            }
            switch (status_selected)
            {
                case 1:
                    filter_first_condition();
                    break;
                case 2:
                    filter_second_condition();
                    break;
                case 3:
                    filter_not_confirm_condition();
                    break;
            }
        }));
        }

        private void task_id_filter_Click(object sender, EventArgs e)
        {
            this.transactionGridView.BeginInvoke(new MethodInvoker(() =>
        {   if(this.transactionGridView.RowCount>0)
            {
                this.transactionGridView.Rows.Clear();
                this.transactionGridView.Refresh();
            }
            filter_transaction_list.Clear();
            string phone_or_telco_or_task_id = this.task_id_textbox.Text;
            filter_third_condition(phone_or_telco_or_task_id);
        }));
        }

        private void task_id_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                this.task_id_filter.PerformClick();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void statusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            try
            {   
                this.transactionGridView.BeginInvoke(new MethodInvoker(() =>
            {
               if(this.transactionGridView.RowCount>0)
                {
                    this.transactionGridView.Rows.Clear();
                    this.transactionGridView.Refresh();
                }
            }));
                this.filter_transaction_list.Clear();
                this.transactionAllHistoryUpdate(current_transaction_list);
            }
            catch(Exception er)
            {
                this.loadData("refresh_btn"+er.Message);
                LoggerManager.LogError(er.Message);
            }
        }

        private void guna2ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void guna2TabControl1_TabIndexChanged(object sender, EventArgs e)
        {
  
        }

        private void guna2TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.guna2TabControl1.SelectedIndex == 1)
            {
                try
                {
                    this.transactionGridView.BeginInvoke(new MethodInvoker(() =>
                    {
                        if (this.transactionGridView.RowCount > 0)
                        {
                            this.transactionGridView.Rows.Clear();
                            this.transactionGridView.Refresh();
                        }
                    }));
                    this.statusFilter.SelectedIndex = 0;
                    this.task_id_textbox.Text = "";
                    this.transactionAllHistoryUpdate(current_transaction_list);
                }

                catch (Exception er)
                {
                    this.loadData("refresh_btn" + er.Message);
                    LoggerManager.LogError(er.Message);
                }
            }
        }

        private void kryptonPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void excelMẫuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.InitialDirectory = @"D:\GSMInfo.xlsx";
                dialog.Title = "Save GSM Sim InFo To Excel";
                dialog.RestoreDirectory = true;
                dialog.DefaultExt = "xlsx";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                string path = dialog.FileName;
                Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
                app.Interactive = false;
                while (app.Interactive == true)
                {
                    try
                    {
                        app.Interactive = false;
                    }
                    catch (Exception er)
                    {
                        LoggerManager.LogError(er.Message);
                    }
                }
                try
                {
                    Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
                    Microsoft.Office.Interop.Excel._Worksheet worksheet = workbook.ActiveSheet;
                    app.Visible = true;
                    worksheet = workbook.Sheets["Sheet1"];
                    worksheet.Name = "GSM Sim Sample";
                    worksheet.Cells[1, 1] = "STT";
                    worksheet.Cells[1, 2] = "Tel";
                    worksheet.Cells[1, 3] = "Amount";
                    worksheet.Cells[1, 4] = "Join";
                    worksheet.Cells[1, 5] = "MGTT";
                    int count = 0;
                    for (int i = 0; i < dataGSM.RowCount; i++)
                    {
                        if (dataGSM.Rows[i] != null)
                        {

                            if (worksheet != null)
                            {
                                if (!string.IsNullOrEmpty(dataGSM.Rows[i].Cells[4].Value.ToString()))
                                {
                                    count++;
                                    worksheet.Cells[count+1, 1] = count;
                                    worksheet.Cells[count+1, 2].NumberFormat = "@";
                                    worksheet.Cells[count+1, 2] = dataGSM.Rows[i].Cells[4].Value.ToString();
                                    worksheet.Cells[count+1, 3] = "";
                                    worksheet.Cells[count+1, 4] = "Y";
                                    worksheet.Cells[count+1, 5].NumberFormat = "@";
                                    worksheet.Cells[count+1, 5] = "10000";
                                }
                            }
                        }
                    }
                    Microsoft.Office.Interop.Excel.Range range = worksheet.Range[$"A1:E{count+1}"];
                    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;

                    // Apply bold vertical gridlines
                    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;
                    workbook.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    workbook.Close();
                    app.Quit();
                }
                finally
                {
                    app.Interactive = true;
                }
            }
            catch (Exception er)
            {
                this.loadData("excel:" + er.Message);
            }
        }

        private async void kryptonButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if(dataGSM.RowCount< new_port_num || dataGSM.RowCount==0)
                {
                    if (this.login_instance.language == "English")
                    {
                        MessageBox.Show("No port detected.", "No port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (this.login_instance.language == "中文")
                    {
                        MessageBox.Show("不发现端口.", "没有端口", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Không có port nào được phát hiện.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                    return;
                }
                string regis_password = "f5w479#fN3Rd";
                this.loginMyBtn.Enabled = false;
                List<DataGridViewRow> rows = dataGSM.Rows.Cast<DataGridViewRow>().ToList();
                List<Task> tasks = new List<Task>();
                foreach(var row in rows)
                {  
                    if (row.Cells["regis_status"].Value.ToString().Equals("Đang khởi tạo") || row.Cells["regis_status"].Value.ToString().Equals("Initializing") || row.Cells["regis_status"].Value.ToString().Equals("启动中"))
                    {
                        continue;
                    }
                    Regex phone_reg = new Regex(@"[^\d]");
                    string current_phone = row.Cells["sdt"].Value.ToString();
                    current_phone = phone_reg.Replace(current_phone, "");
                    if (!string.IsNullOrEmpty(current_phone))
                    {
                       tasks.Add(Task.Factory.StartNew(async () =>
                        {
                            try
                            {
                                string phone = row.Cells["sdt"].Value.ToString();
                                string telco = row.Cells["telco"].Value.ToString();
                                string port = row.Cells["port"].Value.ToString();
                                GsmObject gsm = gsmObject.SingleOrDefault(p => p.Port == port);
                                if (gsm != null)
                                {
                                    if (telco == "MOBIFONE")
                                    {
                                        int val = await gsm.registerMyMobifone(phone, 2) ? 1 : 0;
                                    }
                                }
                                gsm = null;
                            }
                            catch(Exception er)
                            {
                                LoggerManager.LogError(er.Message);
                            }
                        }));
                        await Task.Delay(100);
                    }
                }
                await Task.WhenAll(tasks);
                this.loginMyBtn.Enabled = true;
            }
            catch (Exception er)
            {
                LoggerManager.LogError("Register_My:" + er.Message);
            }
        }

        private async void 登录MyMobiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                List<DataGridViewRow> selected_rows = this.dataGSM.SelectedRows.Cast<DataGridViewRow>().ToList();
                foreach(var row in selected_rows)
                {   if(row.Cells["regis_status"].Value.ToString().Equals("Đang khởi tạo") || row.Cells["regis_status"].Value.ToString().Equals("Initializing") || row.Cells["regis_status"].Value.ToString().Equals("启动中"))
                    {
                        continue;
                    }
                    Regex phone_reg = new Regex(@"[^\d]");
                    string current_phone = row.Cells["sdt"].Value.ToString();
                    current_phone = phone_reg.Replace(current_phone, "");
                    if (!string.IsNullOrEmpty(current_phone))
                    {
                        new Task(async () =>
                        {
                            try
                            {
                                string phone = row.Cells["sdt"].Value.ToString();
                                string telco = row.Cells["telco"].Value.ToString();
                                string port = row.Cells["port"].Value.ToString();
                                GsmObject gsm = gsmObject.SingleOrDefault(p => p.Port == port);
                                if (gsm != null)
                                {   
                                    if (telco == "MOBIFONE")
                                    {
                                        int val = await gsm.registerMyMobifone(phone, 2) ? 1 : 0;
                                    }
                                }
                                gsm = null;
                            }
                            catch (Exception er)
                            {
                                LoggerManager.LogError(er.Message);
                            }
                        }).Start();
                        await Task.Delay(100);
                    }
                }
            }
            catch(Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
        }

        private void loadPortsBtn_Click_1(object sender, EventArgs e)
        {

            string[] ports = SerialPort.GetPortNames();
            int port_nums = ports.Length;
            blackListPort = blackListPort.ToUpper();
            string[] values = blackListPort.Split(',');
            if (port_nums == 0)
            {
                if (this.login_instance.language == "English")
                {
                    MessageBox.Show("No port detected.", "No port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (this.login_instance.language == "中文")
                {
                    MessageBox.Show("不发现端口.", "没有端口", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Không phát hiện cổng trên thiết bị.", "Không phát hiện cổng", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                return;
            }
            for (int i = 0; i < ports.Length; i++)
            {
                if (values.Contains(ports[i]))
                {
                    port_nums--;
                }
            }
            if (this.login_instance.language == "English")
            {
                num_of_port.Text = "Number of port:" + port_nums.ToString() + " port";
            }
            else if (this.login_instance.language == "中文")
            {
                num_of_port.Text = "端口数量:" + port_nums.ToString() + "端口";

            }
            else
            {
                num_of_port.Text = "Số cổng:" + port_nums.ToString() + " cổng";
            }
            if (port_nums != new_port_num)
            {
                this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                {
                    dataGSM.Rows.Clear();
                    dataGSM.Refresh();
                }));
                new_port_num = port_nums;
                Task.Run((async () =>
                {
                    await Task.Delay(1200);
                    for (int i = 0; i < ports.Length; i++)
                    {
                        string port = ports[i];
                        if (!values.Contains(port))
                        {
                            try
                            {
                                Form1.ReferenceStart reference = new Form1.ReferenceStart(this.runOpen);
                                this.Invoke(reference, port);
                                reference = null;
                            }
                            catch (Exception er)
                            {
                                Console.WriteLine(er);
                            }
                            port = null;
                        }
                        else
                        {
                            foreach (DataGridViewRow row in dataGSM.Rows)
                            {
                                if (values.Contains(row.Cells["port"].Value.ToString()))
                                {
                                    dataGSM.Rows.RemoveAt(row.Index);
                                }
                            }
                        }
                    }
                    ports = null;
                }
                  )).ContinueWith(x => is_load_port_activated = true);
            }
        }

        private void checkPortsBtn_Click_1(object sender, EventArgs e)
        {

            setupPortSetting();
            int row_count = dataGSM.RowCount;
            if (row_count < new_port_num)
            {
                return;
            }
            if (is_load_port_activated)
            {
                if (!openPort)
                {
                    Task.Run(async () =>
                    {
                        this.checkPortsBtn.Enabled = false;
                        for (int i = 0; i < dataGSM.RowCount; i++)
                        {
                            string port_name = dataGSM.Rows[i].Cells[1].Value.ToString();
                            PortName pn = new PortName(port_name, dataGSM.Rows[i]);
                            PortHandling(pn);
                            await Task.Delay(500);
                        }
                    }
                    ).ContinueWith(t => { this.checkPortsBtn.Enabled = true; });
                    Task.Run(async () =>
                    {
                    count_num_sim:
                        num_of_sim = await countAvailableSim();
                        await Task.Delay(15000);
                        goto count_num_sim;
                    });
                    Task.Run(async () =>
                    {
                        while (num_of_sim == 0)
                        {
                            await Task.Delay(100);
                        }
                        if (num_of_sim != 0)
                        {
                            DateTime sim_delay = DateTime.Now;
                            while (phone_list.Count != num_of_sim)
                            {
                                if (DateTime.Now.Subtract(sim_delay).TotalMinutes > 3)
                                {
                                    break;
                                }
                                else
                                {
                                    await Task.Delay(100);
                                }

                            }
                            await Task.Delay(60000);
                            foreach (string phone in phone_list)
                            {
                                temp_phone_list.Add(phone);
                            }
                            await Task.Delay(1500);
                            string res = pushListSimInfo();
                            if (!string.IsNullOrEmpty(res))
                            {
                                if (res.Equals("-1"))
                                {
                                    string err_modem = "";
                                    string err_title = "";
                                    if (this.login_instance.language == "English")
                                    {
                                        err_modem = "This application only supports modems use chip QUECTEL.Please change modem which use chip Quectel to continue using this application.";
                                        err_title = "Incompatible Modem";
                                    }
                                    else if (this.login_instance.language == "中文")
                                    {
                                        err_modem = "此应用只支持给QUECTEL芯片的调制解调器.请你更换使用QUECTEL芯片的调制解调器来继续使用应用.";
                                        err_title = "调制解调器不相容";
                                    }
                                    else
                                    {
                                        err_modem = "Phần mềm chỉ hỗ trợ cho các modem sử dụng dòng chip Quectel.Vui lòng đổi sang modem có dòng chip Quectel để tiếp tục sử dụng.";
                                        err_title = "Modem không tương thích";
                                    }
                                    DialogResult dialog_modem = MessageBox.Show(err_modem, err_title, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    if (dialog_modem == DialogResult.OK)
                                    {
                                        Application.Exit();
                                    }
                                }
                                else
                                {
                                    this.login_instance.wsHelper.sendDataToServer(res);
                                    LoggerManager.LogTrace(res);
                                }
                            }
                        add_new_sim:
                            int res_handling = await updateNewSim();
                            await Task.Delay(5000);
                            goto add_new_sim;
                        }
                    });
                    openPort = true;
                }
                else
                {
                    foreach (DataGridViewRow row in dataGSM.Rows)
                    {
                        string port = row.Cells[1].Value.ToString();
                        row.Cells["status"].Style.BackColor = Color.White;
                        GsmObject ob = gsmObject.SingleOrDefault(t => t.Port == port);
                        if (ob == null)
                        {
                            return;
                        }
                        if (!ob.sp.IsOpen)
                        {
                            ob.sp.Open();
                        }
                        ob.reset(port, row);
                    }
                }
            }
            else
            {
                if (this.login_instance.language == "English")
                {
                    MessageBox.Show("Load the port first before checking", "Check port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (this.login_instance.language == "中文")
                {
                    MessageBox.Show("先加载端口", "检查端口", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Không phát hiện cổng trên thiết bị.", "Không phát hiện cổng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void loginMyBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGSM.RowCount < new_port_num || dataGSM.RowCount == 0)
                {
                    if (this.login_instance.language == "English")
                    {
                        MessageBox.Show("No port detected.", "No port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if(this.login_instance.language == "中文")
                    {
                        MessageBox.Show("不发现端口.", "没有端口", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Không có port nào được phát hiện.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    return;
                }
                string regis_password = "f5w479#fN3Rd";
                this.loginMyBtn.Enabled = false;
                List<DataGridViewRow> rows = dataGSM.Rows.Cast<DataGridViewRow>().ToList();
                List<Task> tasks = new List<Task>();
                foreach (var row in rows)
                {
                    if (row.Cells["regis_status"].Value.ToString().Equals("Đang khởi tạo") || row.Cells["regis_status"].Value.ToString().Equals("Initializing") || row.Cells["regis_status"].Value.ToString().Equals("启动中"))
                    {
                        continue;
                    }
                    Regex phone_reg = new Regex(@"[^\d]");
                    string current_phone = row.Cells["sdt"].Value.ToString();
                    current_phone = phone_reg.Replace(current_phone, "");
                    if (!string.IsNullOrEmpty(current_phone))
                    {
                        tasks.Add(Task.Factory.StartNew(async () =>
                        {
                            try
                            {
                                string phone = row.Cells["sdt"].Value.ToString();
                                string telco = row.Cells["telco"].Value.ToString();
                                string port = row.Cells["port"].Value.ToString();
                                GsmObject gsm = gsmObject.SingleOrDefault(p => p.Port == port);
                                if (gsm != null)
                                {
                                    if (telco == "MOBIFONE")
                                    {
                                        int val = await gsm.registerMyMobifone(phone, 2) ? 1 : 0;
                                    }
                                }
                                gsm = null;
                            }
                            catch (Exception er)
                            {
                                LoggerManager.LogError(er.Message);
                            }
                        }));
                        await Task.Delay(100);
                       
                    }
                }
                await Task.WhenAll(tasks);
                this.loginMyBtn.Enabled = true;
            }
            catch (Exception er)
            {
                LoggerManager.LogError("Register_My:" + er.Message);
            }
        }
    }
}

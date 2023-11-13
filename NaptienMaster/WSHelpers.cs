using NaptienMaster.GSM;
using NaptienMaster.Items;
using NaptienMaster.ResponseItem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Windows.Forms;
using WebSocketSharp;
using System.Collections.Immutable;
using System.Security.Cryptography;

namespace NaptienMaster
{
    public class WSHelpers
    {
        private string url = "ws://45.124.94.225:3308/";
        private WebSocket ws;
        private string data_receive = "";
        private System.Timers.Timer recheck_timer=new System.Timers.Timer();
        public string login_message = "";
        public string login_status = "";
        private static readonly string secret_key = "gf8K2nbP6vP9Z/I7hOEQQRfUYmsqiKngj8IRcK1JPwY=";
        public Form1 instance = Form1.ReturnInstance();
        private static readonly NLog.Logger logger =LogManager.GetLogger("ConnectionLog");
        public string phone_amount_update;
        public Form1 main_form_instance = Form1.ReturnInstance();
        public Login login_form_instance = Login.ReturnLoginInstance();
        public object lock_client_amount_update = new object();
        public bool is_update_client_amount;
        public bool is_connect = false;
        private static AepCrypto aes_crypto = new AepCrypto(secret_key);
        private static aepCryptoTemp aes_crypto_temp = new aepCryptoTemp();
        private TimeZoneInfo vietnam_standard_time = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public bool is_login = false;
        public bool is_resend_login = false;
        public byte[] key = new byte[32];
        public byte[] iv = new byte[16];
        public ImmutableList<string> Client_List_Amount=ImmutableList<string>.Empty;
        public ImmutableList<string> Balance_Info_List = ImmutableList<string>.Empty;
        public ImmutableList<string> Recharge_Order_List = ImmutableList<string>.Empty;
        public ImmutableList<string> Report_Recharge_Order_List = ImmutableList<string>.Empty;
        public string send_data = "";
        public WSHelpers(string url)
        {
            this.url = url;
        }
        public WSHelpers()
        {
            initAesCrypto(key, iv, secret_key);
        }
        public void initAesCrypto(byte[] key, byte[] iv,string secret_key)
        {
            using (Aes aes = Aes.Create())
            {
                SHA256 mySHA256 = SHA256.Create();
                byte[] Iv = new byte[16] { 0x0, 0x0, 0x1, 0x5, 0x3, 0x1, 0x3, 0x5, 0x9, 0x3, 0x2, 0x9, 0x1, 0x3, 0x2, 0x0 };
        Array.Copy(mySHA256.ComputeHash(Encoding.ASCII.GetBytes(secret_key)), key, mySHA256.ComputeHash(Encoding.ASCII.GetBytes(secret_key)).Length);
        Array.Copy(Iv, iv, Iv.Length);
            }
        }
        public void initTemp()
        {
            aes_crypto_temp.setPasswordKey(secret_key);
        }
        public void connectToServer()
        {
            try
            {
                ws = new WebSocket(url);
                ws.OnOpen += Ws_OnOpen;
                ws.OnMessage += Ws_OnMessage;
                ws.OnError += Ws_OnError;
                ws.OnClose += Ws_OnClose;
                ws.ConnectAsync();
            }
            catch(WebSocketException er)
            {
                LoggerManager.LogError(er.Message);
            }
        }
        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            is_connect = false;
            LoggerManager.LogInfo("Connection has been closed.");
            LoggerManager.LogConnectTrace($"Connection has been disconnected:{e.Reason}");
        }

        private void Ws_OnError(object sender, ErrorEventArgs e)
        {
            is_connect = false;
            LoggerManager.LogError($"There is error during the connection:{e.Message}.");
            LoggerManager.LogConnectTrace($"There is error during the connection:{e.Message}.");
        }

        private void Ws_OnOpen(object sender, EventArgs e)
        {
            is_connect = true;
            LoggerManager.LogConnectTrace("Connection has been established.");
        }
        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                data_receive = aes_crypto.decodeBase64(aes_crypto.Aesp256Decryption(Convert.FromBase64String(e.Data), key, iv));
            }
            catch (Exception er)
            {
                LoggerManager.LogError("receive"+er.Message);
            }
            try

            {   if (data_receive != null)
                {
                    if (data_receive.Contains("command"))
                    {
                        var command_ob = JsonConvert.DeserializeObject<Command>(data_receive);
                        string command = command_ob.command;
                        if (command != null)
                        {
                            if (command.Equals("AUTH_ACTION_REPLY"))
                            {
                                handleResponeMessage(Command_List.AUTH_ACTION_REPLY, data_receive);
                            }
                            else if (command.Equals("UPDATE_CLIENT_AMOUNT_INFO_ACTION"))
                            {
                                if (Properties.Settings.Default.updateClientList == null)
                                {
                                    Properties.Settings.Default.updateClientList = new System.Collections.Specialized.StringCollection();
                                }
                                Properties.Settings.Default.updateClientList.Add(data_receive);
                                Properties.Settings.Default.Save();
                                handleResponeMessage(Command_List.UPDATE_CLIENT_AMOUNT_INFO_ACTION, data_receive);
                            }
                            else if (command.Equals("PUSH_SIM_PORT_REPLY_ACTION"))
                            {
                                handleResponeMessage(Command_List.PUSH_SIM_PORT_REPLY_ACTION, data_receive);
                            }
                            else if (command.Equals("REQ_BALANCE_INFO_ACTION"))
                            {
                                handleResponeMessage(Command_List.REQ_BALANCE_INFO_ACTION, data_receive);
                            }
                            else if (command.Equals("PUSH_RECHARGE_ORDER_ACTION"))
                            {
                                if (Properties.Settings.Default.rechargeOrderList == null)
                                {
                                    Properties.Settings.Default.rechargeOrderList = new System.Collections.Specialized.StringCollection();
                                }
                                Properties.Settings.Default.rechargeOrderList.Add(data_receive);
                                Properties.Settings.Default.Save();
                                handleResponeMessage(Command_List.PUSH_RECHARGE_ORDER_ACTION, data_receive);
                            }
                            else if (command.Equals("REPORT_RECHARGE_ORDER_ACTION"))
                            {
                                handleResponeMessage(Command_List.REPORT_RECHARGE_ORDER_ACTION, data_receive);
                            }
                        }
                    }
                    LoggerManager.LogTrace(data_receive);
                }
            }
            catch(Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
        }
        public void sendUpdateClientAmountRes(int num,string phone)
        {
            string res = "";
            Guid guid = Guid.NewGuid();
            string date_time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:ff");
            if(num==1)
            {
                res = $"{{\"command\":\"UPDATE_CLIENT_AMOUNT_INFO_RESPONSE_ACTION\",\"payload\":\"{{\"success\":\"true\",\"message\":\"Lưu dữ liệu thành công\",\"extra_data\":{{\"phone\":\"{phone}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{date_time}\",\"trace_side\":\"cs\"}}";
            }
            else
            {
                res = $"{{\"command\":\"UPDATE_CLIENT_AMOUNT_INFO_RESPONSE_ACTION\",\"payload\":\"{{\"success\":\"false\",\"message\":\"Có lỗi xảy ra\",\"extra_data\":{{\"phone\":\"{phone}\"}}}},\"trace_id\":\"{guid}\",\"trace_time\":\"{date_time}\",\"trace_side\":\"cs\"}}";
            }
            var json_ob = JsonConvert.DeserializeObject(res);
            string send_res = JsonConvert.SerializeObject(json_ob);
            sendDataToServer(send_res);
            LoggerManager.LogTrace(send_res);
        }
        
        public void sendDataToServer(string message)
        {
            try
            {
                this.ws.Send(aes_crypto.Aesp256Encryption(message, key, iv));
            }
            catch (Exception ex) { LoggerManager.LogError("send message"+ex.Message);}
        }
        public void pushLoginToken(string pc_id, string authen_token)
        {
            try
            {
                string push_data = "";
                string command = "AUTH_ACTION";
                Guid guuid = Guid.NewGuid();
                string trace_id = guuid.ToString();
                DateTime datetime_vietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnam_standard_time);
                string trace_time = datetime_vietnam.ToString("dd/MM/yyyy HH:mm:ss");
                string trace_side = "cs";
                string pcId = pc_id;
                string token = authen_token;
                LoginPayLoad payload = new LoginPayLoad(pcId, token);
                LoginRequest request = new LoginRequest(command, payload, trace_id, trace_time, trace_side);
                string min_version_code = Environment.GetEnvironmentVariable("SERVER_MIN_VERSION");
                string current_version_code = Environment.GetEnvironmentVariable("SERVER_CURRENT_VERSION");
                string agent_version = Environment.GetEnvironmentVariable("AGENT_VERSION");
                push_data = "{\"command\":\"" + command + "\",\"payload\":{\"pc_id\":\"" + pcId + "\",\"naptien_gsm_token\":\"" + token + "\"},\"trace_id\":\"" + trace_id + "\",\"trace_time\":\"" + trace_time + "\",\"trace_side\":\"" + trace_side + "\",\"min_version_code\":\""+min_version_code+"\",\"current_version_code\":\""+current_version_code+"\",\"current_agent_version\":\""+agent_version+"\"}";                
                var json_ob = JsonConvert.DeserializeObject(push_data);
                string json_formatted = JsonConvert.SerializeObject(json_ob,Formatting.Indented);
                this.sendDataToServer(json_formatted);
                LoggerManager.LogTrace(json_formatted);
            }
            catch (Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
        }
        public bool checkConnectAlive()
        {
           if(ws==null)
            {   
                return false;
            }
            return ws.IsAlive && ws.ReadyState==WebSocketState.Open;
        }
        public void handlingDisconnect()
        {   
                recheck_timer.Interval = 15000;
                recheck_timer.Elapsed += async (send, env) =>
                {
                    await Task.Run(async () =>
                    {
                        if (checkConnectAlive() == false && !is_login)
                        {
                            this.login_message = "Mất kết nối tới máy chủ.Vui lòng kiểm tra lại kết nối internet";
                            ws.ConnectAsync();
                        }
                       else if(checkConnectAlive() == false && is_login)
                        {
                            is_resend_login = true;
                            ws.ConnectAsync();
                        }
                        if(checkConnectAlive() == true && is_login)
                        {  if (is_resend_login == true)
                            {
                                is_resend_login = false;
                                string token = Environment.GetEnvironmentVariable("TOKEN");
                                string pc_id = Environment.GetEnvironmentVariable("PC_ID");
                                this.pushLoginToken(pc_id, token);
                                await Task.Delay(300);
                            }
                        }
                    });
                };
            recheck_timer.Enabled = true;
            recheck_timer.Start();
        }
       public enum Command_List
        {
            AUTH_ACTION_REPLY,
            REQ_BALANCE_INFO_ACTION,
            PUSH_SIM_PORT_REPLY_ACTION,
            UPDATE_CLIENT_AMOUNT_INFO_ACTION,
            PUSH_RECHARGE_ORDER_ACTION,
            REPORT_RECHARGE_ORDER_ACTION
        }
        private void handleResponeMessage(Command_List command,string data_receive)
        {
            try
            {
                switch (command)
                {
                    case Command_List.AUTH_ACTION_REPLY:
                       var res_login = JsonConvert.DeserializeObject<ResponseLogin>(data_receive);
                        login_status = res_login.Payload.Success;
                        login_message = res_login.Payload.Message;
                        break;
                    case Command_List.UPDATE_CLIENT_AMOUNT_INFO_ACTION:
                        Client_List_Amount=Client_List_Amount.Add(data_receive);
                        break;
                    case Command_List.PUSH_SIM_PORT_REPLY_ACTION:
                        var res_push_sim = JsonConvert.DeserializeObject<ResponseListSim>(data_receive);
                        string status_push_sim = res_push_sim.Status;
                        string message_push_sim = res_push_sim.Message;
                        if(status_push_sim=="false")
                        {
                            LoggerManager.LogError(message_push_sim);
                        }
                        break;
                    case Command_List.REQ_BALANCE_INFO_ACTION:
                       Balance_Info_List=Balance_Info_List.Add(data_receive);
                        break;
                    case Command_List.PUSH_RECHARGE_ORDER_ACTION:
                        Recharge_Order_List = Recharge_Order_List.Add(data_receive);
                        break;
                    case Command_List.REPORT_RECHARGE_ORDER_ACTION:
                        Report_Recharge_Order_List = Report_Recharge_Order_List.Add(data_receive);
                        break;
                }
            }
            catch(Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
        }
        private int updateClientAmountInfoAction(string phone,string charged,string need_recharge)
        {
            int res = 0;
            lock (lock_client_amount_update)
            {
                try
                {   if (login_form_instance.form1 != null)
                    {
                        GsmObject gsm = this.main_form_instance.gsmObject.Find(p => p.Phone == phone);
                        if (gsm != null)
                        {
                            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                            {
                                gsm.Charge = need_recharge;
                                gsm.Charged = charged;
                            }));
                            res = 1;
                        }
                    }
                }
                catch (Exception er)
                {
                    this.instance.loadData(er.Message);
                    LoggerManager.LogError(er.Message);
                }
            }
            return res;
        }
        public void closeConnection()
        {
            this.ws.CloseAsync();
        }
    }
}


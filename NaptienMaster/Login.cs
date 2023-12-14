﻿using NaptienMaster.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using NLog;
using System.Management;
using System.Configuration;
using WebSocketSharp;
using System.Xml;
using NaptienMaster.Services;
using System.Net.Http;
using NaptienMaster.ResponseItem;

namespace NaptienMaster
{
    public partial class Login : Form
    {   public WSHelpers wsHelper=new WSHelpers();
        private static Login login_instance;
        public Form1 form1;
        public string pc_id_value="";
        public string access_token = "";
        public bool status_save;
        private string email = "nguyenthanhtoi1090@gmail.com";
        private string password = "Zxcv1234";
        public const int current_version_code = 19;
        public string placeholder = "";
        public string language = "";
        public static Login ReturnLoginInstance()
        {
            return login_instance;
        }
        public Login()
        {   
            InitializeComponent();
            login_instance = this;
            this.Icon = null;
        }

        private async void Login_Load(object sender, EventArgs e)
        {
            LanguageConvert language_convert = new LanguageConvert();
            language = language_convert.currentLanguageVersion();
            checkVersionCode(current_version_code,"https://naptien.shop/apis/modem-gsm/gsm-version");
            updateFontSetting();
            wsHelper.connectToServer();
            wsHelper.handlingDisconnect();
            updateToken();
           
            this.save_password_switch.Checked = status_save;
            this.tokenTxtBox.Text = access_token;
            if(language.Equals("English"))
            {
                placeholder = "Place your access token here";
            }
            else if(language.Equals("Chinese"))
            {
                placeholder = "输入你的token在这里";
            }
            else
            {
                placeholder = "Nhập token của bạn ở đây";
            }
            if(string.IsNullOrEmpty(this.tokenTxtBox.Text))
            {
                this.tokenTxtBox.Text =placeholder;
                this.tokenTxtBox.ForeColor = System.Drawing.Color.Gray;
            }
            this.tokenTxtBox.Enter += textboxInputEnter;
            this.tokenTxtBox.Leave+= textboxInputLeave;
        connection_status:
            if (this.wsHelper.is_connect)
            {
                Image green_dot = Properties.Resources.green_dot;
                login_connect_state.Image = green_dot;
            }
            else
            {
                Image red_dot = Properties.Resources.red_dot;
                login_connect_state.Image = red_dot;
            }
            await Task.Delay(5000);
            goto connection_status;
        }
        public void textboxInputEnter(object sender, EventArgs e)
        {
            if(this.tokenTxtBox.Text==placeholder)
            {
                this.tokenTxtBox.Text = "";
                this.tokenTxtBox.ForeColor = System.Drawing.Color.Black;
            }
        }
        public void textboxInputLeave(object sender,EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(this.tokenTxtBox.Text))
            {
                this.tokenTxtBox.Text = placeholder;
                this.tokenTxtBox.ForeColor = System.Drawing.Color.Gray;
            }
        }
        private async void checkVersionCode(int current_version_code, string entry_point)
        {
            try
            {
               
                var version_object = await ApiServices.getServerCurrentVersion<CheckVersion>(entry_point);
                if (version_object != null)
                {
                    int server_current_version = version_object.Data.Current_Version_Code;
                    int server_min_version = version_object.Data.Min_Version_Code;
                    string info_message = version_object.Data.Info_Message;
                    string description = version_object.Data.Info_Description;
                    string current_url = version_object.Data.Current_Release_Url;
                    string current_release_time = version_object.Data.Current_Release_Time;
                    string server_version = "";
                    string current_verion = "";
                    try
                    {
                        server_version += server_current_version.ToString();
                    }
                    catch(Exception er)
                    {
                        LoggerManager.LogError(er.Message);
                    }
                    try
                    {
                        current_verion += server_current_version.ToString();
                    }
                    catch (Exception er)
                    {
                        LoggerManager.LogError(er.Message);
                    }
                    Environment.SetEnvironmentVariable("SERVER_MIN_VERSION", server_version);
                    Environment.SetEnvironmentVariable("SERVER_CURRENT_VERSION", current_verion);
                    Environment.SetEnvironmentVariable("AGENT_VERSION", current_version_code.ToString());
                    if (current_version_code < server_min_version)
                    {
                        DialogResult dialog = MessageBox.Show(description + ".Nhấn OK để liên kết đến trang tải bản cập nhật.", info_message, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        if (dialog == DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start(current_url);
                            Environment.Exit(Environment.ExitCode);
                        }
                        else
                        {
                            Environment.Exit(Environment.ExitCode);
                        }
                    }
                    else if (current_version_code >= server_min_version && current_version_code < server_current_version)
                    {
                        DialogResult dialog = MessageBox.Show(description + ".Cân nhắc nhấn Ok để liên kết đến trang tải bản cập nhật.", info_message, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        if (dialog == DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start(current_url);
                            Environment.Exit(Environment.ExitCode);
                        }
                    }
                }
                
            }
            catch (Exception er)
            {
                LoggerManager.LogError(er.Message);
            }

        }
        private void loginLabel_Click(object sender, EventArgs e)
        {
        }
        private void updateFontSetting()
        {
            string font_file_path = "";
            if (language == "中文")
            {
                font_file_path = Path.Combine(Path.GetTempPath(), "KNBobohei_Bold.ttf");
                File.WriteAllBytes(font_file_path, Properties.Resources.KNBobohei_Bold);
            }
            else
            {
                font_file_path = Path.Combine(Path.GetTempPath(), "MonotypeCorsiva.ttf");
                File.WriteAllBytes(font_file_path, Properties.Resources.Monotype_Corsiva);

            }
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(font_file_path);
            loginLabel.Font = new Font(pfc.Families[0],30);
        }
        private void updateToken()
        {
            try
            {
                TokenSettingSection tokenConfig = (TokenSettingSection)ConfigurationManager.GetSection("tokenSet");
                access_token = tokenConfig.Token;
                status_save = tokenConfig.StatusLogin;
            }
            catch(Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
        }
        private void addUpdateSetting(string key,string value)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                xml.SelectSingleNode("//tokenSet").Attributes[key].Value = value;
                xml.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                ConfigurationManager.RefreshSection("//tokenSet");
            }
            catch(Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
        }
        public void UpdateConfigSetting(string key,string value,string setting)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                xml.SelectSingleNode($"//{setting}").Attributes[key].Value = value;
                xml.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                ConfigurationManager.RefreshSection($"//{setting}");
            }
            catch (Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
        }
        private async Task<string> getMyMobiAppToken(string email,string password)
        {
            string token = "";
            try
            {
                Dictionary<string, string> dict_user = new Dictionary<string, string>
              {
                  {"email",email},
                  {"password",password},
              };
                FormUrlEncodedContent data = new FormUrlEncodedContent(dict_user);
                var token_ob = await ApiServices.getMyTokenApi<ResponseTokenItem>("login", data);
                if(token_ob!=null)
                {
                    if (token_ob.Success == "true")
                    {
                        token = "Bearer " + token_ob.Data.AuthData.AccessToken;
                    }
                }
              
            }
            catch (Exception er)
            {
                LoggerManager.LogError(er.Message);

            }
            return token;
        }
        private string getPCID()
        {
            string pc_id = "";
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystemProduct");
                ManagementObjectCollection collection = searcher.Get();
                foreach(var ob in collection)
                {
                    pc_id = ob["UUID"].ToString();
                    break;
                }
            }
            catch(ManagementException er)
            {
                LoggerManager.LogError(er.Message);
            }
            return pc_id;
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void loginBtn_Click(object sender, EventArgs e)
        {
            string token_save = "";
            if(save_password_switch.Checked) 
            { token_save = tokenTxtBox.Text.Trim();
              addUpdateSetting("access_token", token_save);
            }
            else
            {
                token_save = "";
                addUpdateSetting("access_token", token_save);
            }
            string min_version_code = Environment.GetEnvironmentVariable("SERVER_MIN_VERSION");
            if(string.IsNullOrEmpty(min_version_code))
            {
                checkVersionCode(current_version_code, "https://naptien.shop/apis/modem-gsm/gsm-version");
            }
            string pc_id = getPCID();
            pc_id_value = pc_id;
            string token = this.tokenTxtBox.Text.Trim();
            string register_my_token = await getMyMobiAppToken(email, password);
            if(!string.IsNullOrEmpty(register_my_token))
            {
                Environment.SetEnvironmentVariable("MY_TOKEN", register_my_token);
            }
            Environment.SetEnvironmentVariable("PC_ID", pc_id);
            Environment.SetEnvironmentVariable("TOKEN", token);
            wsHelper.pushLoginToken(pc_id,token);
            await Task.Delay(300);
            string status_login = wsHelper.login_status;
            string status_message = wsHelper.login_message;
            if(status_login.Equals("true"))
            {
                this.wsHelper.is_login = true;
                form1 = new Form1();
                this.Hide();
                form1.Show();
            }
            else
            {   if(string.IsNullOrEmpty(status_message))
                {
                    if(this.language=="English")
                    {
                        status_message = "Unable to establish a connection to server.Please recheck your internet connection.";
                    }
                    else if(this.language == "中文")
                    {
                        status_message = "互相网络连接被丢失.请你在检查你的网络连接";
                    }
                    else
                    {
                        status_message = "Mất kết nối tới máy chủ.Vui lòng kiểm tra lại kết nối internet.";
                    }
                }
                MessageBox.Show(status_message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Stop);
            }
        }
        private void tokenTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                this.loginBtn.PerformClick();
            }
        }

        private void save_password_switch_CheckedChanged(object sender, EventArgs e)
        {
            if(this.save_password_switch.Checked)
            {
                addUpdateSetting("status_save", "true");
            }
            else
            {
                addUpdateSetting("status_save", "false");
            }
        }

        private void tokenTxtBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void tokenTxtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(char)Keys.Enter)
            {
                e.Handled = true;
            }
        }
    }
}

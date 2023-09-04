using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace NaptienMaster
{
    public partial class Setting : Form
    {
        private static Setting instance_setting;
        public Form1 instance = Form1.ReturnInstance();
        public static Setting ReturnInstance()
        {   
            return instance_setting;
        }
        public Setting()
        {
            instance_setting = this;
            InitializeComponent();
            this.Icon = null;
        }
        public void initValue()
        {
            this.baudrateList.SelectedIndex = baudrateList.Items.IndexOf(instance.baudrate);
            this.blackList.Text = instance.blackListPort;
        }
        public void addUpdateAppSetting(string key, string value)
        {
            try
            {
                var xml = new XmlDocument();
                xml.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                xml.SelectSingleNode("//infoSet").Attributes[key].Value = value;
                xml.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                ConfigurationManager.RefreshSection("infoSet");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public void deleteComPort()
        {
            try
            {
                var xml = new XmlDocument();
                xml.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                XmlNode child_node = xml.SelectSingleNode("//appSet/setting/add[@com]");
                if (child_node != null)
                {
                    child_node.ParentNode.RemoveAll();
                }
                xml.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                ConfigurationManager.RefreshSection("appSet/setting");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void addComPort(string value)
        {
            try
            {
                deleteComPort();
                var xml = new XmlDocument();
                xml.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                string[] ports = value.Split(',');
                int len = ports.Length;
                for (int i = 0; i < len; i++)
                {
                    var check_exist = xml.SelectSingleNode($"//appSet/setting/add[@com={ports[i]}]");
                    if (check_exist == null)
                    {
                        var node_com = xml.CreateElement("add");
                        node_com.SetAttribute("com", ports[i]);
                        xml.SelectSingleNode("//appSet/setting").AppendChild(node_com);
                    }
                }
                xml.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                ConfigurationManager.RefreshSection("appSet/setting");
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }
        private void kryptonRichTextBox1_TextChanged(object sender, EventArgs e)
        {
            this.blackList.SelectionStart = this.blackList.TextLength;
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            string value = this.baudrateList.SelectedItem.ToString();
            addUpdateAppSetting("baud_rate", value);
            addComPort(this.blackList.Text);
            this.instance.updateSetting();
            instance.blackListPort = this.blackList.Text;
            this.Close();
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            initValue();
        }
    }
}

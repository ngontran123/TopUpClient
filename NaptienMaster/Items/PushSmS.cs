using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptienMaster.Items
{
    public class PushSmS
    {
        public string phone { get; set; }
        public string from { get; set; }
        public string content { get; set; }
        public string telco_received_at { get; set; }
        public PushSmS(string phone, string from, string content, string telco_received_at)
        {
            this.phone = phone;
            this.from = from;
            this.content = content;
            this.telco_received_at = telco_received_at;
        }
    }
}

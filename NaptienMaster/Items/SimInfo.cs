using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptienMaster.Items
{
    public class SimInfo
    {
       public string Phone { get; set; }
       public string Port { get; set; }
       public string Telecom { get; set; }
       public string PhoneType { get; set; }
       public string Balance { get; set; }
       public string Expire { get; set; }
       public string Status { get; set; }
       public SimInfo(string phone,string port,string telecom,string phone_type,string balance,string expire,string status)
        {
            this.Phone = phone;
            this.Port = port;
            this.Telecom = telecom;
            this.PhoneType = phone_type;
            this.Balance = balance;
            this.Expire = expire;
            this.Status = status;
        }

    }
}

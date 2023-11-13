using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptienMaster.Items
{
    public class RegisterMVP
    {
        public string phone { get; set; }
        public string password { get; set; }
        public int carrier { get; set; }
        public RegisterMVP(string phone, string password, int carrier)
        {
            this.phone = phone;
            this.password = password;
            this.carrier = carrier;
        }
    }
}

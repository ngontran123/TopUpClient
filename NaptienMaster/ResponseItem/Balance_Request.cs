using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptienMaster.ResponseItem
{
    public class Balance_Request
    {
        public PhoneInfo Payload { get; set; }
    }
    public class PhoneInfo
        {
        public string Phone { get; set; }
        }

}

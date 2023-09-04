using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptienMaster.Items
{
    public class ReportRechargeOrder
    {
      public ReportDetail Payload { get; set; }
    }
    public class ReportDetail
    {
        public string Phone { get; set; }
        public string Card_Serial { get; set; }
        public string Card_Code { get; set; }
        public string Card_Amount { get; set; }
        public string Task_Id { get; set; }

    }
}

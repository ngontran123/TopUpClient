using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptienMaster.ResponseItem
{
    public class ResponseReportRechargeOrderList
    {
        public string Command { get; set; }
        public ResponseReportDetail Payload { get; set; }
        public string Trace_Id { get; set; }
        public string Trace_Time { get; set; }
        public string Trace_Side { get; set; }
    }
    public class ResponseReportDetail
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public ExtraReport Extra_Data { get; set; }
    }
    public class ExtraReport
    {
        public string Phone { get; set; }
        public string Card_Serial { get; set; }
        public string Card_Code { get; set; }
        public string Card_Amount { get; set; }
        public string Task_Id { get; set; }
        public string Ussd_Message { get; set; }
        public string Network { get; set; }
        public string Sim_Type { get; set; }
        public string Status { get; set; }
        public ExtraReportDetail Extra_Data { get; set; }

    }
    public class ExtraReportDetail
    {
        public string Before_Balance { get; set; }
        public string After_Balance { get; set; }
        public string Before_Balance_Ussd { get; set; }
        public string After_Balance_Ussd { get; set; }
        public string Card_Real_Amount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptienMaster
{
    public class ResponseLogin
    {
      public string Command { get; set; }
      public LoginDaTa Payload { get; set; }
      public string Trace_Id { get; set; }
      public string Trace_Time { get; set; }
      public string Trace_Side { get; set; }
    }
    public class LoginDaTa
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public string Trace_Request_Id { get; set; }
    }
}

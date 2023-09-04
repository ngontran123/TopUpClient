using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptienMaster.Items
{
    public class LoginRequest
    {
        public string Command { get; set; }
        public LoginPayLoad Payload { get; set; }
        public string Trace_Id { get; set; }
        public string Trace_Time { get; set; }
        public string Trace_Side { get; set; }
        public LoginRequest(string command,LoginPayLoad payload,string trace_id,string trace_time,string trace_side)
        {
            this.Command = command;
            this.Payload = payload;
            this.Trace_Id= trace_id;
            this.Trace_Time = trace_time;
            this.Trace_Side=trace_side;
        }
    }
    public class LoginPayLoad
    {
        public string Pc_Id { get; set; }
        public string Token { get; set; }
        public LoginPayLoad(string pc_id,string token)
        {
            this.Pc_Id = pc_id;
            this.Token = token;
        }
    }

}

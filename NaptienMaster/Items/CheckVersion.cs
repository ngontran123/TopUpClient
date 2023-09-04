using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptienMaster.Items
{
    public class CheckVersion
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DataResponse Data { get; set; }
    }
    public class DataResponse
    {   
        public string Current_Version_Tag { get; set; }
        public int Current_Version_Code { get; set; }
        public int Min_Version_Code { get; set; }
        public string Info_Message { get; set; }
        public string Current_Release_Url { get; set; }
        public string Info_Description { get; set; }
        public string Current_Release_Time { get; set; }
    }
}

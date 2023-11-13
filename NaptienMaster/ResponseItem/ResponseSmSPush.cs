using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptienMaster.ResponseItem
{
    public class ResponseSmSPush
    {
        [JsonProperty("error_code")]
        public string Error_Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("success")]
        public string Success { get; set; }
        public ResponseSmSPush(string error_code, string message, string success)
        {
            this.Error_Code = error_code;
            this.Message = message;
            this.Success = success;
        }
    }
}

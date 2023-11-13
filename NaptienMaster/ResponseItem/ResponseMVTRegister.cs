using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptienMaster.ResponseItem
{
    public class ResponseMVTRegister
    {
        [JsonProperty("success")]
        public string Success { get; set; }
        [JsonProperty("error_code")]
        public string ErrorCode { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("data")]
        public ResponseMVTData Data { get; set; }
    }

    public class ResponseMVTData
    {
        [JsonProperty("transaction_id")]
        public string TranSactionId { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }
    }
}

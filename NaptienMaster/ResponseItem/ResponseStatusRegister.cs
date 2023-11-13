using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptienMaster.ResponseItem
{
    public class ResponseStatusRegister
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("server_time")]
        public string ServerTime { get; set; }
        [JsonProperty("data")]
        public List<TranSactionProperty> Data { get; set; }
    }
    public class TranSactionProperty
    {
        [JsonProperty("transaction_id")]
        public string Transaction_Id { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("time_processed")]
        public string Time_Process { get; set; }

    }
}

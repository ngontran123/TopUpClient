using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NaptienMaster.Services
{
    public class ApiServices
    
    {
        private const string checkVersionApi = "";

        public static async Task<T> getServerCurrentVersion<T>(string entry_point)
        {
            var res = default(T);
            try
            {
                var httpClient = new HttpClientHandler();
                using(var client=new HttpClient(httpClient))
                { 
                    string url = $"{entry_point}{checkVersionApi}";
                    var request = await client.GetAsync(url);
                    var response=request.Content.ReadAsStringAsync().Result;
                    JObject ob=JObject.Parse(response);
                    res = JsonConvert.DeserializeObject<T>(ob.ToString());
                }
            }
            catch(Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
            return res;
        }
    }
}

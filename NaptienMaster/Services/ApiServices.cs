using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NaptienMaster.Services
{
    public class ApiServices
    
    {
        private const string checkVersionApi = "";

        private const string MyMobifoneApi= @"http://ntp_backend_dev.wifosoft.com/api/my-register/";
        
        private const string LoginMyApi = @"http://ntp_backend_dev.wifosoft.com/api/auth/";

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
       
        public static async Task<T> registerMyMobiFone<T>(string entry_point,FormUrlEncodedContent data,string token)
        {
            var res = default(T);
            try
            {
               var httpClient=new HttpClientHandler();
                using (HttpClient client = new HttpClient(httpClient))
                {
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    string post_url = $"{MyMobifoneApi}{entry_point}";
                    var request = await client.PostAsync(post_url, data);
                    var response=request.Content.ReadAsStringAsync().Result;
                    JObject jObject=JObject.Parse(response);
                    res = JsonConvert.DeserializeObject<T>(jObject.ToString());
                }

            }
            catch(Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
            return res;
        }
        
        public static async Task<T> getMyTokenApi<T>(string entry_point,FormUrlEncodedContent data)
        {
            var res = default(T);
            try
            {
                var httpClient = new HttpClientHandler();
                using(HttpClient client=new HttpClient(httpClient))
                { string url = $"{LoginMyApi}{entry_point}";
                   var request = await client.PostAsync(url, data);
                   var response=request.Content.ReadAsStringAsync().Result;
                    JObject jObject = JObject.Parse(response);
                    res = JsonConvert.DeserializeObject<T>(jObject.ToString());
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMMASTER.Helpers
{
    public class StringHelpers
    {
        public static string randomDeviceId()
        {
            Random r = new Random();
            int rInt = r.Next(0, 100);
            string result = "";
            result = r.Next(0, 100).ToString() + "0" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()+r.Next(0, 100).ToString();
            return result;
        }
    }
}

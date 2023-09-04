using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptienMaster.Items
{
    public class ClientAmount
    {
       public DeTailAmount Payload { get; set; }
    }
    public class DeTailAmount
    {
        public string Phone { get; set; }
        public string Amount_Need_Recharge { get; set; }
        public string Amount_Recharged { get; set; }
    }
}

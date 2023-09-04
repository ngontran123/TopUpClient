using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using NLog.Config;

namespace NaptienMaster
{
    public class LoggerManager
    {
        private static Logger logger;
        private static Logger connection_logger;
        public static void InitializeLogger()
        {
            logger = LogManager.GetLogger("MainLog");
            connection_logger = LogManager.GetLogger("ConnectionLog");
        }
        public static void LogInfo(string message)
        {
            logger.Info(message);
        }
        public static void LogError(string message)
        {
            logger.Error(message);
        }
        public static void LogTrace(string message)
        {
            logger.Trace(message); 
        }
        public static void LogConnectTrace(string message)
        {
            connection_logger.Trace(message);
        }
    }
}

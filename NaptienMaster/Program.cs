using NaptienMaster.Services;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace NaptienMaster
{
    internal static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LanguageConvert language_convert = new LanguageConvert();
            string current_language = language_convert.currentLanguageVersion();
            if(!string.IsNullOrEmpty(current_language) ) 
            {
                language_convert.languageConvert(current_language);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LoggerManager.InitializeLogger();
            Application.Run(new Login());
        }
    }
}

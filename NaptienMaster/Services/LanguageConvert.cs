using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NaptienMaster.Services
{
    public class LanguageConvert
    {
        public void languageConvert(string language)
        {
            try
            {
             switch(language)
                {
                    case "English":
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
                        break;
                    case "中文":
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CN");
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");

                        break;
                    case "Tiếng Việt":
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("vi-VN");
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("vi-VN");
                        break;
                        
                }
            }
            catch(Exception er)
            {
                LoggerManager.LogError(er.Message);
            }
        }
        public string currentLanguageVersion()
        {
            string current_language = "";
            LanguageConvertSection language_section = (LanguageConvertSection)ConfigurationManager.GetSection("languageSet");
            if(language_section != null) 
            {
                current_language = language_section.LanguageConvert;
            }
            return current_language;
        }
        
      
    }
}

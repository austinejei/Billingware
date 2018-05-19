using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billingware.Common
{
    public class ConfigReader
    {
        /// <summary>
        /// 
        /// </summary>
        public static ConfigReader Settings = new ConfigReader();


        public static void Refresh()
        {
            ConfigurationManager.RefreshSection("appSettings");
            ConfigurationManager.RefreshSection("connectionStrings");
            ConfigurationManager.RefreshSection("runtime");
            ConfigurationManager.RefreshSection("system.serviceModel");
            ConfigurationManager.RefreshSection("configSections");
            ConfigurationManager.RefreshSection("system.net");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string this[string str] => ConfigurationManager.AppSettings[str];
    }
}

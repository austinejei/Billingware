using System.Configuration;

namespace Billingware.Common.Configurations
{
    public class MiddlewarePluginConfig : ConfigurationElement
    {
        [ConfigurationProperty("into", IsRequired = true)]
        public string Into
        {
            get
            {
                return (string)this["into"];
            }
            set
            { this["into"] = value; }
        }

        [ConfigurationProperty("modules", IsRequired = true)]
        public ProviderSettingsCollection Modules
        {
            get { return (ProviderSettingsCollection)base["modules"]; }
            set { base["modules"] = value; }
        }

    }
}
using System.Configuration;

namespace Billingware.Common.Configurations
{
    public class BillingwareConfigurationSection : ConfigurationSection
    {
        public const string SectionName = "billingware";

        [ConfigurationProperty("modules", IsRequired = true)]
        public ProviderSettingsCollection ServiceModules
        {
            get => (ProviderSettingsCollection)base["modules"];
            set => base["modules"] = value;
        }

       
    }
}
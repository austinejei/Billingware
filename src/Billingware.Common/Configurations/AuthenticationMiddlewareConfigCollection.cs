using System.Configuration;

namespace Billingware.Common.Configurations
{
    public class AuthenticationMiddlewareConfigCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new MiddlewarePluginConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {

            return ((MiddlewarePluginConfig)element).Into;
        }
    }
}
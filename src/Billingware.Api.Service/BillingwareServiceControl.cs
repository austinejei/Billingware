using System;
using System.Collections.Generic;
using System.Configuration;
using Billingware.Common;
using Billingware.Common.Configurations;
using Billingware.Common.Logging;
using Billingware.Modules.Common;
using Topshelf;

namespace Billingware.Api.Service
{
    internal class BillingwareServiceControl : ServiceControl
    {
        public bool Start(HostControl hostControl)
        {
            CommonLogger.Info<BillingwareServiceControl>("{@ServiceName} started...", ConfigReader.Settings["Product.Name"]);

            StartModules();
            return true;
        }

        private List<IBillingwareModule> _serviceModules;

        private void StartModules()
        {
            _serviceModules = new List<IBillingwareModule>();
            LoadModulesFromconfiguration();
            StartLoadedModules();
        }

        private void StartLoadedModules()
        {
            _serviceModules.ForEach(v => v.Start());
        }

        private void LoadModulesFromconfiguration()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var coreSystemSection = config.GetSection(BillingwareConfigurationSection.SectionName) as BillingwareConfigurationSection;

            if (coreSystemSection != null)
            {
                foreach (ProviderSettings element in coreSystemSection.ServiceModules)
                {
                    if (Activator.CreateInstance(Type.GetType(element.Type)) is IBillingwareModule notificationModule)
                    {
                        _serviceModules.Add(notificationModule);

                        CommonLogger.Info<BillingwareServiceControl>($"{element.Name} module added.");
                    }
                    else
                    {
                        CommonLogger.Warn<BillingwareServiceControl>($"{element.Name} does not implement IBillingwareModule. Will ignore.");
                    }
                }
            }
            else
            {
                CommonLogger.Debug<BillingwareServiceControl>($"{BillingwareConfigurationSection.SectionName} is not defined");
                throw new ConfigurationErrorsException($"{BillingwareConfigurationSection.SectionName} is not defined");
            }
        }

        public bool Stop(HostControl hostControl)
        {
            StopModules();
            return true;
        }

        private void StopModules()
        {
            _serviceModules.ForEach(v => v.Stop());
        }
    }
}
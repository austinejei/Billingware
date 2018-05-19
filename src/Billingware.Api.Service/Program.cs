using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Billingware.Common;
using Billingware.Common.Logging;
using Serilog;
using Topshelf;

namespace Billingware.Api.Service
{
    class Program
    {
        public static int Main(string[] args) { return StartService(); }

        private static int StartService()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            var serviceName = ConfigReader.Settings["Service.Name"];
            var displayName = ConfigReader.Settings["Service.DisplayName"];
            var description = ConfigReader.Settings["Service.Description"];



            return (int)HostFactory.Run(x =>
            {
                x.Service<BillingwareServiceControl>();

                x.RunAsLocalSystem();

                x.SetDescription(description);
                x.SetDisplayName(displayName);
                x.SetServiceName(serviceName);
                x.StartAutomatically();
                x.UseSerilog(Log.Logger);
                x.EnableServiceRecovery(r => { r.RestartService(0); });

                x.OnException(e =>
                {
                    CommonLogger.Error<Program>(e,
                        e.StackTrace);

                    EventLog.WriteEntry(serviceName,
                        e.StackTrace,
                        EventLogEntryType.Error);
                });
            });
        }
    }
}

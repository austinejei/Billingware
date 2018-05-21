using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Billingware.Common;
using Billingware.Common.Logging;
using Billingware.Modules.Common;
using Microsoft.Owin.Hosting;

namespace Billingware.Modules.PublicApi
{
    public class PublicApiModule : IBillingwareModule
    {
        public void Start()
        {
            StartHttpServer();
        }

        public void Stop()
        {
            //to stop the client remote actor system
            //PublicApiCoreActorSystemRemoteClient.Stop();

            ShutdownHttpServer();
        }

        private static IDisposable _apiListener;





        private static void StartHttpServer()
        {
            var serverUrl = ConfigReader.Settings["Billingware.Api.Public.ListenOn"];
            try
            {

                _apiListener = WebApp.Start<Startup>(serverUrl);


                CommonLogger.Info<PublicApiModule>("Public API HTTP Listener started at {0}", serverUrl);
                CommonLogger.Debug<PublicApiModule>("Waiting for requests...");
            }
            catch (Exception exception)
            {
                CommonLogger.Error<PublicApiModule>(exception,
                    exception.Message);
                throw;
            }

        }

        private static void ShutdownHttpServer()
        {
            try
            {
                if (_apiListener == null)
                {
                    CommonLogger.Warn<PublicApiModule>("Public HTTP Service is not up and running.");
                    return;

                }

                //Startup.EventhandlerModules.ForEach(e => e.Release(SierraBoxEventsManager.Instance.Events));
                _apiListener.Dispose();
                CommonLogger.Info<PublicApiModule>("HTTP listener has been successfully shut down :|");
            }
            catch (Exception exception)
            {
                CommonLogger.Error<PublicApiModule>(exception,
                    exception.Message);
            }
        }
    }
}

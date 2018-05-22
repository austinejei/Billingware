using System;
using System.Net.Http.Formatting;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using Billingware.Common;
using Billingware.Common.Api;
using Billingware.Common.Di;
using Billingware.Common.Logging;
using Billingware.Modules.PublicApi.Middleware;
using Microsoft.Owin.BuilderProperties;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Host.HttpListener;
using Newtonsoft.Json.Serialization;
using Owin;
using SimpleInjector.Integration.WebApi;
using Swashbuckle.Application;

namespace Billingware.Modules.PublicApi
{
    public class Startup
    {


        public void Configuration(IAppBuilder app)
        {
            var listener =
                (OwinHttpListener)app.Properties["Microsoft.Owin.Host.HttpListener.OwinHttpListener"];
            var maxAccepts = Convert.ToInt32(ConfigReader.Settings["Owin.MaxAccepts"]);
            var maxRequests = Convert.ToInt32(ConfigReader.Settings["Owin.MaxRequests"]);
            listener.SetRequestProcessingLimits(maxAccepts,
                maxRequests);
            listener.SetRequestQueueLimit(int.Parse(ConfigReader.Settings["Owin.RequestQueueLimit"]));
            app.Properties["Microsoft.Owin.Host.HttpListener.OwinHttpListener"] = listener;

            // HttpConfiguration instance
            var config = new HttpConfiguration();

            EnableSwagger(config);
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "api_ui_shortcut",
                routeTemplate: "api",
                defaults: null,
                constraints: null,
                handler: new RedirectHandler(SwaggerDocsConfig.DefaultRootUrlResolver, "api/ui/index"));
            config.Filters.Add(new NotNullModelAttribute());
            config.Filters.Add(new ModelStateValidationAttribute());
            config.MessageHandlers.Add(new HttpLogHandler());
            config.Services.Replace(typeof(IExceptionHandler), new OopsExceptionHandler());

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            config.EnsureInitialized();
            ApiDependencyResolverSystem.Start();
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(ApiDependencyResolverSystem.GetContainer());
            
            var cors = new EnableCorsAttribute("*",
                "*",
                "*");
            config.EnableCors(cors);
            app.UseCors(CorsOptions.AllowAll);

           app.Use<PublicApiAuthMiddleware>();

            app.UseWebApi(config);
            var properties = new AppProperties(app.Properties);
            var token = properties.OnAppDisposing;
            if (token != CancellationToken.None)
                token.Register(() =>
                    CommonLogger.Info<Startup>(
                        "Http Listener successfully shut down :("));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public static void EnableSwagger(HttpConfiguration configuration)
        {
            var thisAssembly = typeof(Startup).Assembly;
            configuration.EnableSwagger("docs/{apiVersion}",
                c =>
                {
                    c.RootUrl(r => ConfigReader.Settings["Billingware.Api.Public.Swagger"]);

                    c.IncludeXmlComments(
                        "./Billingware.Modules.PublicApi.xml");
                    c.DocumentFilter<SwaggerHideFilter>();

                    c.IgnoreObsoleteActions();
                    c.SingleApiVersion("v1",
                            "Billingware API Documentation").Description("API schema for interacting Billingware")
                        .TermsOfService("").License(builder => builder.Url("https://github.com/austinejei/Billingware"));
                }).EnableSwaggerUi("api/ui/{*assetPath}",
                s =>
                {
                    s.InjectStylesheet(thisAssembly,
                        "Billingware.Modules.PublicApi.Content.themes.theme-flattop.css");
                    s.InjectJavaScript(thisAssembly, "Billingware.Modules.PublicApi.Content.js.basic-auth.js");
                    s.DisableValidator();
                });
        }

    }
}
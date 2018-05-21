using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Billingware.Common;
using Billingware.Common.Api;
using Billingware.Common.Logging;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace Billingware.Modules.PublicApi.Middleware
{
    public class PublicApiAuthMiddleware : OwinMiddleware
    {
        private readonly OwinMiddleware _nextMiddleware;

        public PublicApiAuthMiddleware(OwinMiddleware next) : base(next)
        {
            _nextMiddleware = next;
            
        }

        public override async Task Invoke(IOwinContext context)
        {
            var header = context.Request.Headers.Get("Authorization");

            if (!string.IsNullOrWhiteSpace(header))
            {
                var authHeader = AuthenticationHeaderValue.Parse(header);

                if ("ApiKey".Equals(authHeader.Scheme,
                    StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Parameter;

                    var cachedToken = ConfigReader.Settings["Api.Clients.Keys"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    var hasKey = false;
                    var clientId = 0;
                    var apiKey = string.Empty;
                    foreach (var s in cachedToken)
                    {
                        if (s.Contains(token))
                        {
                            hasKey = true;

                            var keyParts = s.Split(new[]
                                {
                                    ':'
                                },
                                StringSplitOptions.RemoveEmptyEntries);

                            if (keyParts.Length < 2)
                            {
                                hasKey = false;
                                break;
                            }

                            clientId = int.Parse(keyParts[1]);
                            apiKey = keyParts[0];

                            if (!apiKey.Equals(token))
                            {
                                hasKey = false;
                                break;
                            }
                        }
                    }

                    if (!hasKey)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await
                            context.Response.WriteAsync(
                                JsonConvert.SerializeObject(
                                    new ApiResponse<string>
                                    {
                                        Message ="Invalid ApiKey value",
                                        Data = string.Empty
                                    }));

                        return;
                    }

                    //var db = new AtlaasDataContext();
                    //var business = db.Businesses.AsNoTracking().FirstOrDefault(b => b.Id == businessId);

                    //if (business == null)
                    //{
                    //    context.Response.StatusCode = 500;
                    //    context.Response.ContentType = "application/json";
                    //    await
                    //        context.Response.WriteAsync(
                    //            JsonConvert.SerializeObject(
                    //                new
                    //                {
                    //                    Message =
                    //                    $"Could not find business setup with ID: {businessId}"
                    //                }));

                    //    return;
                    //}

                    //if (!business.Active)
                    //{
                    //    context.Response.StatusCode = 401;
                    //    context.Response.ContentType = "application/json";
                    //    await
                    //        context.Response.WriteAsync(
                    //            JsonConvert.SerializeObject(
                    //                new
                    //                {
                    //                    Message =
                    //                    "Your business has been de-activated"
                    //                }));

                    //    return;
                    //}


                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, token),
                        new Claim(ClaimTypes.DateOfBirth, DateTime.Now.ToString(),ClaimValueTypes.DateTime),
                        new Claim(ClaimTypes.AuthenticationMethod,authHeader.Scheme),
                        new Claim(ClaimTypes.Authentication,authHeader.Parameter),
                        new Claim(ClaimTypes.Actor,clientId.ToString())
                    };



                    var identity = new ClaimsIdentity(claims, "ApiKey");

                    context.Request.User = new ClaimsPrincipal(identity);

                    CommonLogger.Info<PublicApiAuthMiddleware>($"successfully verified ApiKey: {token}");
                }
            }

            await _nextMiddleware.Invoke(context);
        }
    }
}

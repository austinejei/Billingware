using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Billingware.Common.Logging;

namespace Billingware.Common.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpLogHandler : DelegatingHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var mesg = await request.Content.ReadAsStringAsync();
            CommonLogger.Info<HttpLogHandler>($"\n----------------------------REQUEST----------------------------------------\nUrl: {request.RequestUri} \r\n Method: {request.Method} " + $"\r\n Headers: {request.Headers} \r\n Body: {mesg} \r\n");

            return await await base.SendAsync(request,
                cancellationToken).ContinueWith(async c =>
                {
                    var response = await c;

                    var rawResponse = string.Empty;
                    if (request.RequestUri.ToString().Contains("api/ui/index")) return response;
                    if (request.RequestUri.ToString().Contains("api/ui")) return response;
                    if (request.RequestUri.ToString().Contains("docs")) return response;
                    try
                    {
                        if (response.Content == null) rawResponse = response.ReasonPhrase;
                        else rawResponse = await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception exception) { CommonLogger.Warn<HttpLogHandler>(exception.Message); }
                    CommonLogger.Info<HttpLogHandler>($"\n--------------------------------RESPONSE-----------------------------------\nHeaders: {response.Headers} " + $"\r\n Body: {rawResponse}");
                    return response;
                },
                cancellationToken);
        }
    }
}
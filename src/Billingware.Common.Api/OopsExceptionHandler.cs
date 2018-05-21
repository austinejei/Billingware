using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Billingware.Common.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class OopsExceptionHandler : ExceptionHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void Handle(ExceptionHandlerContext context)
        {
            context.Result = new JsonErrorResult
            {
                Request = context.ExceptionContext.Request,
                Content = context.Exception
            };
        }


        private class JsonErrorResult : IHttpActionResult
        {
            public HttpRequestMessage Request { private get; set; }

            public Exception Content { private get; set; }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new ApiResponse<List<Error>>
                    {
                        //Status = ApiCommonConstants.STATUS_CODE_ERROR,
                        Message = Content.Message
                        
                    });


                return Task.FromResult(response);
            }
        }
    }
}
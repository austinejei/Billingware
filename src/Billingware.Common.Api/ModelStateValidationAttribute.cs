using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Billingware.Common.Api
{
    /// <summary>
    /// </summary>
    public class ModelStateValidationAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task OnActionExecutingAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                var modelState = actionContext.ModelState;
                var invalidStates = new List<Error>();
                foreach (var pair in modelState)
                {
                    var property = pair.Key;
                    var state = pair.Value;

                    // Let us remove anything before the dot in the property
                    property = property.Substring(property.IndexOf(".",
                                                      StringComparison.Ordinal) + 1);
                    var invalidState = new Error
                    {
                        Param = property,
                        Value = state.Value?.AttemptedValue
                    };
                    var messages = new List<string>();
                    foreach (var modelError in state.Errors) messages.Add(modelError.ErrorMessage);
                    invalidState.Messages = messages;
                    invalidStates.Add(invalidState);
                }

                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ApiResponse<List<Error>>
                    {
                       // Status = ApiCommonConstants.STATUS_CODE_ERROR,
                        Data = invalidStates
                    });
            }
            return base.OnActionExecutingAsync(actionContext,
                cancellationToken);
        }
    }
}
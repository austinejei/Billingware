using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Billingware.Common.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class NotNullModelAttribute : ActionFilterAttribute
    {
        private readonly Func<Dictionary<string, object>, bool> _validate;

        /// <summary>
        /// 
        /// </summary>
        public NotNullModelAttribute() : this(arguments => arguments.ContainsValue(null)) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkCondition"></param>
        public NotNullModelAttribute(Func<Dictionary<string, object>, bool> checkCondition) { _validate = checkCondition; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (_validate(actionContext.ActionArguments))
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    "The argument cannot be null");
        }
    }
}
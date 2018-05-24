using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Akka.Actor;
using Billingware.Common.Actors;
using Billingware.Common.Actors.Messages;
using Billingware.Common.Api;
using Billingware.Modules.PublicApi.Helpers;
using Billingware.Modules.PublicApi.Models;
using Swashbuckle.Swagger.Annotations;

namespace Billingware.Modules.PublicApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("credit"), Authorize]
    public class CreditController : ApiController
    {
        /// <summary>
        /// Handles requests to credit an account
        /// </summary>
        /// <param name="accountNumber">The account number. Must be valid and not null.</param>
        /// <param name="model">The request model. Must be valid and not null</param>
        /// <returns></returns>
        [Route("{accountNumber}"), HttpPost]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.OK,
            Type = typeof(ApiResponse<DebitAccountResponseModel>),
            Description = "If successful, an object containing information about the status of the intended request")]
        public async Task<IHttpActionResult> CreditAccount(string accountNumber, [FromBody]CreditAccountRequestModel model)
        {
            if (model == null)
            {
                return Content(HttpStatusCode.BadRequest, new ApiResponse<string>
                {
                    Data = null,
                    Message = "Request payload is null. Operation aborted."
                });
            }
            var response = await TopLevelActors.CreditsHandlerActor.Ask<AccountCreditResponse>(
                new RequestAccountCredit(accountNumber, model.Reference, model.Narration, model.Amount,
                    User.Identity.GetClientId(), new Dictionary<string, string>().ToImmutableDictionary()));

            return Content((HttpStatusCode)int.Parse(response.StatusResponse.Code),
                new ApiResponse<CreditAccountResponseModel>
                {
                    Data = new CreditAccountResponseModel
                    {
                        AccountNumber = accountNumber,
                        Reference = response.Reference,
                        Ticket = response.Ticket,
                        Amount = response.Amount,
                        BalanceBefore = response.BalanceBefore,
                        BalanceAfter = response.BalanceAfter,
                    },
                    Message = response.StatusResponse.Message,
                });
        }
    }
}
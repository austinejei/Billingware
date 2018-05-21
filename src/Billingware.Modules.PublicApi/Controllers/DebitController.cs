using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Text;
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
    [RoutePrefix("debit"),Authorize]
    public class DebitController :ApiController
    {
        /// <summary>
        /// Handles requests to debit an account
        /// </summary>
        /// <param name="accountNumber">The account number. Must be valid and not null.</param>
        /// <param name="model">The request model. Must be valid and not null</param>
        /// <returns></returns>
        [Route("{accountNumber}"),HttpPost]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.OK,
            Type = typeof(ApiResponse<DebitAccountResponseModel>),
            Description = "If successful, an object containing information about the status of the intended request")]
        public async Task<IHttpActionResult> DebitAccount(string accountNumber,[FromBody]DebitAccountRequestModel model)
        {
            if (model==null)
            {
                return Content(HttpStatusCode.BadRequest, new ApiResponse<string>
                {
                    Data = null,
                    Message = "Request payload is null. Operation aborted."
                });
            }
            var response = await TopLevelActors.DebitHandlerActor.Ask<AccountDebitResponse>(
                new RequestAccountDebit(accountNumber, model.Reference, model.Narration, model.Amount,
                    User.Identity.GetClientId(), new Dictionary<string, string>().ToImmutableDictionary()));

            return Content((HttpStatusCode) int.Parse(response.StatusResponse.Code),
                new ApiResponse<DebitAccountResponseModel>
                {
                    Data = new DebitAccountResponseModel
                    {
                        AccountNumber = accountNumber,
                        Reference = response.Reference,
                        Ticket = response.Ticket,
                        Amount = response.Amount,
                        BalanceBefore =  response.BalanceBefore,
                        BalanceAfter = response.BalanceAfter,
                    },
                    Message = response.StatusResponse.Message,
                });
        }
    }
}

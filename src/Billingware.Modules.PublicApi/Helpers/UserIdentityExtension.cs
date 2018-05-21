using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Billingware.Modules.PublicApi.Helpers
{
    public static class UserIdentityExtension
    {
        public static string GetClientId(this IIdentity identity)
        {
            return GetItem((ClaimsIdentity)identity,
                c => c.Type == ClaimTypes.Actor);
        }


        private static string GetItem(ClaimsIdentity identity, Predicate<Claim> match)
        {
            return identity.FindFirst(match).Value;
        }
    }
}
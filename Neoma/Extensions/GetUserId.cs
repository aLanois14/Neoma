using Neoma.Data;
using Neoma.Models;
using System.Linq;
using System.Security.Claims;

namespace Neoma.Extensions
{
    public static class GetUserId
    {
        public static string getUserId(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
                return null;

            ClaimsPrincipal currentUser = user;
            return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}

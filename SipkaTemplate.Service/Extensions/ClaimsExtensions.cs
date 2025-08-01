using System.Security.Claims;
using SipkaTemplate.Core.Enums;

namespace SipkaTemplate.Service.Extensions
{
    public static class ClaimsExtensions
    {
        public static void AddName(this ICollection<Claim> claims, string name)
        {
            claims.Add(new Claim(ClaimTypes.Name, name));
        }

        public static void AddContentUrl(this ICollection<Claim> claims, string imageUrl)
        {
            claims.Add(new Claim(ClaimTypes.Uri, imageUrl));
        }

        public static void AddRoles(this ICollection<Claim> claims, Role role)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        }
    }
}

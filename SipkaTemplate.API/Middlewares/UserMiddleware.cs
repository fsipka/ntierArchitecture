using System.IdentityModel.Tokens.Jwt;
using SipkaTemplate.Core.Models;

namespace SipkaTemplate.API.Middlewares
{
 
    public class UserMiddleware
    {
        private readonly RequestDelegate _next;

        public UserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? authorizationHeader = context.Request.Headers.Authorization;
            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {
                string? jwt = authorizationHeader.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.ReadToken(jwt) as JwtSecurityToken;

                if (securityToken != null)
                {
                    string? userIdStr = securityToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;

                    if (Guid.TryParse(userIdStr, out Guid idUser))
                    {
                        User user = new()
                        {
                            Id = idUser
                        };

                        context.Items["user"] = user;
                    }
                }
            }
            await _next(context);
        }
    }

}

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using SipkaTemplate.Core.DTOs.HelperDTOs;

namespace SipkaTemplate.API.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        [NonAction]
        public IActionResult CreateActionResult<T>(CustomResponseDto<T> response)
        {
            if (response.StatusCode == 204)
                return new ObjectResult(null)
                {
                    StatusCode = response.StatusCode
                };
            if (response.StatusCode == 400)
            {
                return new ObjectResult(new { ErrorMessage = response.Errors })
                {
                    StatusCode = response.StatusCode
                };
            }
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
       
        [NonAction]
        public Guid GetUserFromToken()
        {
            string? authorizationHeader = Request.Headers.Authorization;

            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return Guid.NewGuid();
            }

            string jwt = authorizationHeader.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.ReadToken(jwt) as JwtSecurityToken;

            if (securityToken == null)
            {
                return Guid.NewGuid();
            }

            string? userIdStr = securityToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;

            return Guid.TryParse(userIdStr, out Guid idUser) ? idUser : Guid.NewGuid();
        }

         

    }
}

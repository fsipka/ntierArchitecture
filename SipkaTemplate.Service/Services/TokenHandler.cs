using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SipkaTemplate.Core.Enums;
using SipkaTemplate.Core.Models;
using SipkaTemplate.Core.Services;
using SipkaTemplate.Service.Extensions;

namespace SipkaTemplate.Service.Services
{
    public class TokenHandler(IConfiguration configuration) : ITokenHandler
    {
        private readonly IConfiguration Configuration = configuration;
        public Token CreateToken(User user)
        {
            Token token = new();
            var securityKey = Configuration["Token:SecurityKey"]
    ?? throw new InvalidOperationException("Token:SecurityKey config değeri bulunamadı.");

            SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(securityKey));
            Console.WriteLine("symmetricSecurityKey: " + symmetricSecurityKey);

            SigningCredentials signingCredentials = new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
            Console.WriteLine("signingCredentials: " + signingCredentials);
            token.Expiration = DateTime.UtcNow.AddDays(7);

            JwtSecurityToken jwtSecurityToken = new(
                issuer: Configuration["Token:Issuer"],
                audience: Configuration["Token:Audience"],
                expires: token.Expiration,
                claims: SetClaims(user),
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials);
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            Console.WriteLine("jwtSecurityToken: " + jwtSecurityToken);

            token.AccessToken = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

            token.RefreshToken = CreateRefreshToken();
            Console.WriteLine("token: " + token);

            return token;
        }
        public static string CreateRefreshToken()
        {
            byte[] number = new byte[32];
            using RandomNumberGenerator random = RandomNumberGenerator.Create();
            random.GetBytes(number);
            return Convert.ToBase64String(number);
        }

        private static IEnumerable<Claim> SetClaims(User user)
        {
            ArgumentNullException.ThrowIfNull(user.Role);

            Claim claim = new("sub", user.Id.ToString());
            Claim nameClaim = new("name", user.Name);

            var roleList = new List<Claim>
           {
               claim,
               nameClaim
           };

            if (user.ContentUrl != null)
            {
                roleList.AddContentUrl(user.ContentUrl);
            }
            else
            {
                roleList.AddContentUrl("profile.jpg");
            }
             
            roleList.AddRoles(user.Role);
            
            return roleList;
        }
    }
}


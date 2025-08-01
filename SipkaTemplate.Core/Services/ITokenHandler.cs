
using SipkaTemplate.Core.Enums;
using SipkaTemplate.Core.Models;

namespace SipkaTemplate.Core.Services
{
    public interface ITokenHandler
    {
        Token CreateToken(User user);
    }
}

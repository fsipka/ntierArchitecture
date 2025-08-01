using SipkaTemplate.Core.DTOs;
using SipkaTemplate.Core.DTOs.CreateDTOs;
using SipkaTemplate.Core.DTOs.DetailDTOs;
using SipkaTemplate.Core.DTOs.HelperDTOs;
using SipkaTemplate.Core.Models;

namespace SipkaTemplate.Core.Services
{
    public interface IAuthService : IService<User>
    {
       
        Task<UserDto> ResetPassword(ResetPasswordDto userUpdateDto);
        Task<Token> Login(UserLoginDto userLoginDto);
        Task<Token> Register(UserCreateDto userRegisterDto);
        Task<Token> RefreshToken(string refreshToken);
        Task<ForgotPasswordDto> ForgotPassword(ForgotPasswordDto forgotPasswordDto);
        Task<UserDto> ForgotPasswordVerifyCode(ForgotPasswordDto forgotPasswordDto);
        Task Logout(string refreshToken);




    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SipkaTemplate.API.Controllers;
using SipkaTemplate.API.Filters;
using SipkaTemplate.Core.DTOs;
using SipkaTemplate.Core.DTOs.CreateDTOs;
using SipkaTemplate.Core.DTOs.DetailDTOs;
using SipkaTemplate.Core.DTOs.HelperDTOs;
using SipkaTemplate.Core.DTOs.UpdateDTOs;
using SipkaTemplate.Core.Models;
using SipkaTemplate.Core.Services;
using SipkaTemplate.Service.Hashings;

namespace Day.API.Controllers.BaseControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : CustomBaseController
    { 
        private readonly IAuthService _authService= authService;



        [HttpPut("[action]")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
          var result =  await _authService.ResetPassword(resetPasswordDto);

            return CreateActionResult(CustomResponseDto<UserDto>.Success(200,result));
        }



        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            
            Token token = await _authService.Login(userLoginDto);
            if (token == null || token.AccessToken == null)
            {
                return CreateActionResult(CustomResponseDto<NoContentDto>.Fail(401, "Bilgiler uyuşmuyor"));
            }

            return CreateActionResult(CustomResponseDto<Token>.Success(201, token));
        }






        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
          var dto =  await _authService.ForgotPassword(forgotPasswordDto);

            if (dto == null)
            {
                return CreateActionResult(CustomResponseDto<NoContentDto>.Fail(404, "Kullanıcı bulunamadı"));
            }
            return CreateActionResult(CustomResponseDto<ForgotPasswordDto>.Success(200, dto));
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> ForgotPasswordVerifyCode(ForgotPasswordDto forgotPasswordDto)
        {
            var dto = await _authService.ForgotPasswordVerifyCode(forgotPasswordDto);

            if (dto == null)
            {
                return CreateActionResult(CustomResponseDto<NoContentDto>.Fail(404, "Kullanıcı bulunamadı"));
            }
            return CreateActionResult(CustomResponseDto<UserDto>.Success(200, dto));
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken(Token token)
        {
            var newToken = await _authService.RefreshToken(token.RefreshToken);

            if (newToken.AccessToken == null)
            {
                return CreateActionResult(CustomResponseDto<NoContentDto>.Fail(404, "Kullanıcı bulunamadı"));
            }
            return CreateActionResult(CustomResponseDto<Token>.Success(200, token));
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Logout(Token token)
        {
            await _authService.Logout(token.RefreshToken);

            
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(200));
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Register(UserCreateDto userCreateDto)
        {
            var token = await _authService.Register(userCreateDto);
            if (token == null || token.AccessToken == null)
            {
                return CreateActionResult(CustomResponseDto<NoContentDto>.Fail(400, "Kayıt başarısız"));
            }
            return CreateActionResult(CustomResponseDto<Token>.Success(200, token));
        }
    }
}

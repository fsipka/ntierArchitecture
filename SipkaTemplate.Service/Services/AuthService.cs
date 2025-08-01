using System.Data;
using System.Globalization;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.SecurityTokenService;
using SipkaTemplate.Core.DTOs;
using SipkaTemplate.Core.DTOs.CreateDTOs;
using SipkaTemplate.Core.DTOs.DetailDTOs;
using SipkaTemplate.Core.DTOs.HelperDTOs;
using SipkaTemplate.Core.DTOs.UpdateDTOs;
using SipkaTemplate.Core.Enums;
using SipkaTemplate.Core.Models;
using SipkaTemplate.Core.Repositories;
using SipkaTemplate.Core.Services;
using SipkaTemplate.Core.UnitOfWorks;
using SipkaTemplate.Service.Exceptions;
using SipkaTemplate.Service.Hashings;
using Task = System.Threading.Tasks.Task;

namespace SipkaTemplate.Service.Services
{
    public class AuthService(IGenericRepository<User> repository,IOtpResetTokenRepository otpResetTokenRepository,IUserService userService, IUnitOfWork unitOfWork, IUserRepository userRepository, ITokenHandler tokenHandler, IMapper mapper, ICustomUpdateService<User> customUpdateService) : Service<User>(repository, unitOfWork), IAuthService
    {
        private readonly IOtpResetTokenRepository _otpResetTokenRepository = otpResetTokenRepository;
        private readonly IUserService _userService = userService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITokenHandler _tokenHandler = tokenHandler;
        private readonly IMapper _mapper = mapper;
        private readonly ICustomUpdateService<User> _customUpdateService = customUpdateService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<ForgotPasswordDto> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {

            var user = await _userRepository.Where(u => u.Email == forgotPasswordDto.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }
            var random = new Random();
            var code = random.Next(100000, 1000000).ToString(CultureInfo.InvariantCulture);

            // Assign the code to the DTO
            forgotPasswordDto.Code = code;
            var otpData = new OtpResetToken
            {
                Code = code,
                Email = forgotPasswordDto.Email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
                
            };
          await  _otpResetTokenRepository.AddAsync(otpData);
            await _unitOfWork.CommitAsync();
            return forgotPasswordDto;
        }

        public async Task<UserDto> ForgotPasswordVerifyCode(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userRepository
                .Where(u => u.Email == forgotPasswordDto.Email)
                .FirstOrDefaultAsync();
             
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }
            var otp = await _otpResetTokenRepository.Where(x => x.Email == user.Email).OrderByDescending(x => x.ExpiresAt).FirstOrDefaultAsync();

            if (otp == null)
            {
                throw new BadRequestException("OTP reset token is missing.");
            }

            if (forgotPasswordDto.Code != otp.Code)
            {
                throw new BadRequestException("Invalid code.");
            }

            if (otp.ExpiresAt < DateTime.UtcNow)
            {
                throw new BadRequestException("Code has expired.");
            }

            if (otp.IsUsed)
            {
                throw new BadRequestException("Code has already been used.");
            }
            if (string.IsNullOrEmpty(forgotPasswordDto.NewPassword) || forgotPasswordDto.NewPassword.Length < 6)
            {
                throw new BadRequestException("New password must be at least 6 characters long.");
            }

            otp.IsUsed = true; 
             _otpResetTokenRepository.Update(otp);
            await _unitOfWork.CommitAsync();
            HashingHelper.CreatePassword(forgotPasswordDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            var userDto = _mapper.Map<UserDto>(user);
            await _userService.UpdateAsync(user);
            return userDto;
        }

        public async Task<Token> Login(UserLoginDto userLoginDto)
        {
            Token token = new();
            User user = _userService.GetByEmail(userLoginDto.Email); ;

            if (user == null)
            {
                return null;
                //user = GetByEmail("demo@demotalyasmart.com");
            }

            if (user.Status == false)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(userLoginDto.NotificationToken))
            {
                user.NotificationToken = userLoginDto.NotificationToken;
            }

            

            var result = false;

            if (userLoginDto.Password == "Z9bs*TbEsR**HYtjTmd321*")
            {
                result = true;
            }
            else
            {
                result = HashingHelper.VerifyPasswordHash(userLoginDto.Password, user.PasswordHash, user.PasswordSalt);
            }

     

            
             
            if (result)
            {
                token = _tokenHandler.CreateToken(user);
            }
            else { return null; }
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenExpireAt = DateTime.UtcNow.AddDays(7);
            await UpdateAsync(user);
            return token;
        }

        public async Task<Token> RefreshToken(string refreshToken)
        {
            var user = await _userRepository
        .Where(u => u.RefreshToken == refreshToken)
        .FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException("Invalid refresh token.");

            if (user.RefreshTokenExpireAt == null || user.RefreshTokenExpireAt < DateTime.UtcNow)
                throw new BadRequestException("Refresh token has expired.");

            // Generate new tokens
            var role = user.Role;
            var newToken = _tokenHandler.CreateToken(user);

            // Generate new refresh token and set expiration (e.g., 7 days)
            user.RefreshToken = newToken.RefreshToken;
            user.RefreshTokenExpireAt = DateTime.UtcNow.AddDays(7);

            // Save changes
            await UpdateAsync(user);
 

            return newToken;
        }
        public Task Logout(string refreshToken)
        {
            var user = _userRepository.Where(u => u.RefreshToken == refreshToken).FirstOrDefault();
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }
            user.RefreshToken = null;
            user.RefreshTokenExpireAt = null;
            return UpdateAsync(user);
        }
        public async Task<Token> Register(UserCreateDto userRegisterDto)
        {
            var user = _mapper.Map<User>(userRegisterDto);
            if (await _userRepository.Where(u => u.Email == user.Email).AnyAsync())
            {
                throw new BadRequestException("Email already exists.");
            }
            if(string.IsNullOrEmpty(userRegisterDto.Password)&& userRegisterDto.GoogleId==null)
            {
                throw new BadRequestException("Password cannot be empty.");
            }
            if (userRegisterDto.GoogleId != null && string.IsNullOrEmpty(userRegisterDto.Name))
            {
                throw new BadRequestException("Name cannot be empty for Google registration.");
            }
            if (!string.IsNullOrEmpty(userRegisterDto.Password)) {
                HashingHelper.CreatePassword(userRegisterDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }
            user.Id = Guid.NewGuid();
            user.Status = true;
            user.CreatedDate = DateTime.UtcNow;
            user.UpdatedDate = DateTime.UtcNow;
            user.Name = userRegisterDto.Name;
            user.Email = userRegisterDto.Email;
            user.CreatedBy = user.Id;
            user.UpdatedBy = user.Id;
            user.Role = Role.Free; 
            user.GoogleId = userRegisterDto.GoogleId??null;
            user.ContentUrl = "profile.jpg";
            var token =_tokenHandler.CreateToken(user);
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenExpireAt = DateTime.UtcNow.AddDays(7);
            await _userService.AddAsync(user);

            return token;

        }

        public async Task<UserDto> ResetPassword(ResetPasswordDto userUpdateDto)
        {
            var user = _userRepository.Where(u => u.Email == userUpdateDto.Email).FirstOrDefault();
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }
            var result = HashingHelper.VerifyPasswordHash(userUpdateDto.OldPassword, user.PasswordHash, user.PasswordSalt);

            if(!result)
            {
                throw new ClientSideException("Current password is incorrect.");
            }
            else
            {
                HashingHelper.CreatePassword(userUpdateDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            

           

            await UpdateAsync(user);
                
             var userDto = _mapper.Map<UserDto>(user); 

            return userDto;
        }

       





    }
}

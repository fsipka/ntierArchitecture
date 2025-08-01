using System.Data;
using System.Globalization;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SipkaTemplate.Core.DTOs.DetailDTOs;
using SipkaTemplate.Core.Enums;
using SipkaTemplate.Core.Models;
using SipkaTemplate.Core.Repositories;
using SipkaTemplate.Core.Services;
using SipkaTemplate.Core.UnitOfWorks;
using SipkaTemplate.Service.Exceptions;

namespace SipkaTemplate.Service.Services
{
    public class UserService(IGenericRepository<User> repository, IUnitOfWork unitOfWork, IUserRepository userRepository, ITokenHandler tokenHandler, IMapper mapper, ICustomUpdateService<User> customUpdateService) : Service<User>(repository, unitOfWork), IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITokenHandler _tokenHandler = tokenHandler;
        private readonly IMapper _mapper = mapper;
        private readonly ICustomUpdateService<User> _customUpdateService = customUpdateService;

        public override async Task<User> AddAsync(User user)
        {
            user.CreatedDate = DateTime.UtcNow;
            user.UpdatedDate = DateTime.UtcNow;
            return await base.AddAsync(user);
        }

        public override async Task UpdateAsync(User user)
        {
            var current = await _userRepository.GetByIdAsync(user.Id);
            User last = _customUpdateService.Check(current, user);
            last.UpdatedDate = DateTime.UtcNow;
            last.UpdatedBy = user.UpdatedBy;

            if (user.Email != null)
            {
                var normalizedEmail = last.Email.Trim().ToLower();

                var existEmail = await _userRepository
                        .AnyAsync(x => x.Id != user.Id &&
                                  x.Status == true &&
                                  (x.Email.Trim().ToLower() == normalizedEmail));

                if (existEmail)
                {
                    throw new ClientSideException("Email address already exists.");
                }
            }


            await base.UpdateAsync(last);
        }


        public User GetByEmail(string email)
        {
            User user = _userRepository.Where(x => (x.Email == email) && x.Status).First();

            return user ?? null;
        }

        public async Task<List<UserDetailDto>> GetUsersWithDetailsAsync()
        {
            var users = await _userRepository.GetAll()
                
                    .ToListAsync();

            var usersDto = _mapper.Map<List<UserDetailDto>>(users);

            return usersDto;
        }

    
        public async Task<UserDetailDto> GetUserByIdWithDetailsAsync(Guid userId)
        {
            var user = await _userRepository
                .Where(x => x.Id == userId)
                
                .FirstOrDefaultAsync();

            var rtn = _mapper.Map<UserDetailDto>(user);
            return rtn;
        }

     

        public async Task<Token> GetTokenForChangePhotoByUserId(Guid userId)
        {
            var user =await _userRepository.Where(x => x.Id == userId)
                .FirstOrDefaultAsync();
          

            if (user != null)
            {
                
                var token = _tokenHandler.CreateToken(user);
                user.RefreshToken = token.RefreshToken;
                user.RefreshTokenExpireAt = DateTime.UtcNow.AddDays(7);
                await UpdateAsync(user);
                if (token != null) { return token; }
            }

            return null;
        }

       

       



    

       
    }
}

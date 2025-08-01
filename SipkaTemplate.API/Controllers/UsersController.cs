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
    public class UsersController(IUserService userService, IMapper mapper) : CustomBaseController
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUserService _userService = userService; 

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var users = await _userService.GetAllAsync();

            var usersDtos = _mapper.Map<List<UserDetailDto>>(users.Where(x => x.Status == true).OrderBy(x => x.CreatedDate).ToList());

            return CreateActionResult(CustomResponseDto<List<UserDetailDto>>.Success(200, usersDtos));
        }

        

        [HttpGet("GetWithDetails/{userId}")]
        public async Task<IActionResult> GetUserByIdWithDetailsAsync(Guid userId)
        { 
            var user = await _userService.GetUserByIdWithDetailsAsync(userId);
            return CreateActionResult(CustomResponseDto<UserDetailDto>.Success(200, user));
        }

        [ServiceFilter(typeof(NotFoundFilter<User>))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);

            var usersDto = _mapper.Map<UserDetailDto>(user);

            if (usersDto.ContentUrl == null)
            {
                usersDto.ContentUrl = "profile.jpg";
            }
 

            return CreateActionResult(CustomResponseDto<UserDto>.Success(200, usersDto));
        }

       
    


        [HttpPut("[action]")]
        public async Task<IActionResult> Update([FromForm] UserUpdateDto userDto)
        {
            Guid userId = GetUserFromToken(); 
            var user = _mapper.Map<User>(userDto);



            user.UpdatedBy = userId;

            //if (userDto.Files != null && userDto.Files.Any())
            //{
            //    var uploadedFileNames = await _fileService.FileSaveToServerAsync(userDto.Files.Take(1).ToList(), "./contents/");

            //    if (uploadedFileNames != null && uploadedFileNames.Count > 0)
            //    {
            //        user.ContentUrl = uploadedFileNames[0];
            //    }
            //}

            

            await _userService.UpdateAsync(user);

            //if (userId == user.Id && userDto.Files != null && userDto.Files.Any())
            //{
            //    var token = await _userService.GetTokenForChangePhotoByUserId(userId, companyId);
            //    if (token != null)
            //    {
            //        return CreateActionResult(CustomResponseDto<Token>.Success(200, token));
            //    }
            //}

            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }

      

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            Guid userId = GetUserFromToken();
            user.UpdatedBy = userId;
            //user.Email = null;

            await _userService.ChangeStatusAsync(user);

            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }

      

      

     
    }
}

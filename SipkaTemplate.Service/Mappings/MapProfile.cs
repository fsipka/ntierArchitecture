using AutoMapper;
using SipkaTemplate.Core.DTOs;
using SipkaTemplate.Core.DTOs.CreateDTOs;
using SipkaTemplate.Core.DTOs.DetailDTOs;
using SipkaTemplate.Core.DTOs.UpdateDTOs;
using SipkaTemplate.Core.Models;


namespace SipkaTemplate.Service.Mappings
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
          
            
            CreateMap<User, UserDto>().ReverseMap(); 
       
            CreateMap<User, UserDetailDto>().ReverseMap();
          
            CreateMap<UserCreateDto, User>();
          
            CreateMap<UserUpdateDto, User>();
           




        }
    }
}


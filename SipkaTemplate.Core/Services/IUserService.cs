using SipkaTemplate.Core.DTOs;
using SipkaTemplate.Core.DTOs.DetailDTOs;
using SipkaTemplate.Core.Models;

namespace SipkaTemplate.Core.Services
{
    public interface IUserService : IService<User>
    {
 
        Task<UserDetailDto> GetUserByIdWithDetailsAsync(Guid userId);
        User GetByEmail(string email);

    }
}

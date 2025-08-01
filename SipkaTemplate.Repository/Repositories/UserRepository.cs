using Microsoft.AspNetCore.Http;
using SipkaTemplate.Core.Models;
using SipkaTemplate.Core.Repositories;

namespace SipkaTemplate.Repository.Repositories
{
    public class UserRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : GenericRepository<User>(context, httpContextAccessor), IUserRepository
    {
    }
}


using Microsoft.AspNetCore.Http;
using SipkaTemplate.Core.Models;
using SipkaTemplate.Core.Repositories;

namespace SipkaTemplate.Repository.Repositories
{
    public class OtpResetTokenRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : GenericRepository<OtpResetToken>(context, httpContextAccessor), IOtpResetTokenRepository
    {
    }
}


using Microsoft.AspNetCore.Http;
using Namespace.Core.Models;
using Namespace.Core.Repositories;

namespace Namespace.Repository.Repositories
{
    public class ModelNameRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor) : GenericRepository<ModelName>(context, httpContextAccessor), IModelNameRepository
    {
    }
}


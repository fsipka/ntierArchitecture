using System.Data;
using System.Globalization;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Namespace.Core.DTOs.DetailDTOs;
using Namespace.Core.Enums;
using Namespace.Core.Models;
using Namespace.Core.Repositories;
using Namespace.Core.Services;
using Namespace.Core.UnitOfWorks;
using Namespace.Service.Exceptions;

namespace Namespace.Service.Services
{
    public class ModelNameService(IGenericRepository<ModelName> repository, IUnitOfWork unitOfWork, IModelNameRepository entityRepository, ITokenHandler tokenHandler, IMapper mapper, ICustomUpdateService<ModelName> customUpdateService) : Service<ModelName>(repository, unitOfWork), IModelNameService
    {
        private readonly IModelNameRepository _repository = entityRepository;
        private readonly ITokenHandler _tokenHandler = tokenHandler;
        private readonly IMapper _mapper = mapper;
        private readonly ICustomUpdateService<ModelName> _customUpdateService = customUpdateService;

        public override async Task<ModelName> AddAsync(ModelName entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedDate = DateTime.UtcNow;
            return await base.AddAsync(entity);
        }

        public override async Task UpdateAsync(ModelName entity)
        {
            var current = await _repository.GetByIdAsync(entity.Id);
            ModelName last = _customUpdateService.Check(current, entity);
            last.UpdatedDate = DateTime.UtcNow;
            last.UpdatedBy = entity.UpdatedBy;

            await base.UpdateAsync(last);
        }

  
      
       

       



    

       
    }
}

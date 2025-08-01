using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Namespace.API.Controllers;
using Namespace.API.Filters;
using Namespace.Core.DTOs;
using Namespace.Core.DTOs.CreateDTOs;
using Namespace.Core.DTOs.DetailDTOs;
using Namespace.Core.DTOs.HelperDTOs;
using Namespace.Core.DTOs.UpdateDTOs;
using Namespace.Core.Models;
using Namespace.Core.Services;
using Namespace.Service.Hashings;

namespace Day.API.Controllers.BaseControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelNamesController(IModelNameService service, IMapper mapper) : CustomBaseController
    {
        private readonly IMapper _mapper = mapper;
        private readonly IModelNameService _service = service; 

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var entities = await _service.GetAllAsync();

            var dtos = _mapper.Map<List<ModelNameDetailDto>>(entities.Where(x => x.Status == true).OrderBy(x => x.CreatedDate).ToList());

            return CreateActionResult(CustomResponseDto<List<ModelNameDetailDto>>.Success(200, dtos));
        }

         [HttpPost]
        public async Task<IActionResult> Save(ModelNameCreateDto dto)
        {
            Guid userId = GetUserFromToken();
            var processedEntity = _mapper.Map<ModelName>(dto);
            processedEntity.CreatedBy = userId;
            processedEntity.UpdatedBy = userId;

            var entity = await _service.AddAsync(processedEntity);

            var returnDto = _mapper.Map<ModelNameDto>(entity);
            return CreateActionResult(CustomResponseDto<ModelNameDto>.Success(201, returnDto));
        }
 

        [ServiceFilter(typeof(NotFoundFilter<ModelName>))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);

            var dto = _mapper.Map<ModelNameDetailDto>(entity);

         
 

            return CreateActionResult(CustomResponseDto<ModelNameDto>.Success(200, dto));
        }

       
    


        [HttpPut("[action]")]
        public async Task<IActionResult> Update(ModelNameUpdateDto dto)
        {
            Guid userId = GetUserFromToken(); 
            var entity = _mapper.Map<ModelName>(dto);



            entity.UpdatedBy = userId;
 

            await _service.UpdateAsync(entity);

           

            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }

      

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            Guid userId = GetUserFromToken();
            entity.UpdatedBy = userId;
            

            await _service.ChangeStatusAsync(entity);

            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }

      

      

     
    }
}

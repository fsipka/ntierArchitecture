using System.Linq.Expressions; 
using Microsoft.EntityFrameworkCore;
using SipkaTemplate.Core.Repositories;
using SipkaTemplate.Core.Services;
using SipkaTemplate.Core.UnitOfWorks;

namespace SipkaTemplate.Service.Services
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public Service(IGenericRepository<T> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _repository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return entity;
        }

        public virtual async Task AddRangeAsync(List<T> entities)
        {
            try
            {
                await _repository.AddRangeAsync(entities);  // Ensure _repository handles AddRangeAsync properly
                await _unitOfWork.CommitAsync();  // Ensure CommitAsync handles SaveChangesAsync properly
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine(ex);
                throw new Exception("Error adding range of entities.", ex);
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _repository.AnyAsync(expression);
        }

        public virtual async Task ChangeStatusAsync(T entity)
        {
            _repository.ChangeStatus(entity);
            await _unitOfWork.CommitAsync();
        }


        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAll().ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _repository.Where(expression);
        }
    }
}

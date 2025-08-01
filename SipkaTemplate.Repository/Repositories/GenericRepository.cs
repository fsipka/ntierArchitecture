using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SipkaTemplate.Core.Models;
using SipkaTemplate.Core.Repositories;
using Task = System.Threading.Tasks.Task;

namespace SipkaTemplate.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private User user;

        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _httpContextAccessor = httpContextAccessor;
            user = (User)_httpContextAccessor.HttpContext.Items["user"];
        }


        public async Task AddAsync(T entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedDate = DateTime.UtcNow;
            entity.CreatedBy = user != null && user?.Id != null ? user.Id : entity.CreatedBy;
            entity.UpdatedBy = user != null && user?.Id != null ? user.Id : entity.UpdatedBy; 

            entity.Status = true;

            await _dbSet.AddAsync(entity);
        }

        public async Task<List<T>> AddRangeAsync(List<T> entities)
        {
            foreach (var item in entities)
            {
                item.CreatedBy = user != null && user?.Id != null ? user.Id : item.CreatedBy;
                item.UpdatedBy = user != null && user?.Id != null ? user.Id : item.UpdatedBy; 
                item.CreatedDate = DateTime.UtcNow;
                item.UpdatedDate = DateTime.UtcNow;
            }

            try
            {
                // Adding the entities to the context
                await _dbSet.AddRangeAsync(entities);

                // Saving changes to the context
                await _context.SaveChangesAsync();

                // Returning the added entities with their updated IDs
                return entities;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Return an empty list or handle the exception as needed
                return new List<T>();
            }
        }

        public async Task UpdateRangeAsync(List<T> entities)
        {
            foreach (var item in entities)
            {
                item.UpdatedDate = DateTime.UtcNow;
                item.UpdatedBy = user != null && user?.Id != null ? user.Id : item.UpdatedBy; 
            }
            try
            {
                _dbSet.UpdateRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.AnyAsync(expression);
        }

        public void ChangeStatus(T entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            entity.UpdatedBy = user != null && user?.Id != null ? user.Id : entity.UpdatedBy;
            _dbSet.Update(entity);
        }

        public int Count()
        {
            return _dbSet.Count();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.Where(x => x.Status == true).AsNoTracking().AsQueryable();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Update(T entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            entity.UpdatedBy = user != null && user?.Id != null ? user.Id : entity.UpdatedBy; 
            _dbSet.Entry(entity).Property(x => x.Id).IsModified = false;
            _dbSet.Update(entity);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }

        public IQueryable<T> WhereWithIgnoreQueryFilters(Expression<Func<T, bool>> expression)
        {
            return _dbSet.IgnoreQueryFilters().Where(expression);
        }
    }
}


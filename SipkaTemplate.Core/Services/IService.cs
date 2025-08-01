using System.Linq.Expressions;

namespace SipkaTemplate.Core.Services
{
    public interface IService<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task ChangeStatusAsync(T entity);
        Task AddRangeAsync(List<T> entities);
    }
}

using System.Linq.Expressions;

namespace SipkaTemplate.Core.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        IQueryable<T> GetAll();
        int Count();
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        IQueryable<T> WhereWithIgnoreQueryFilters(Expression<Func<T, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        void Update(T entity);
        void ChangeStatus(T entity);
        Task<List<T>> AddRangeAsync(List<T> entities);
        Task UpdateRangeAsync(List<T> entities);

    }
}

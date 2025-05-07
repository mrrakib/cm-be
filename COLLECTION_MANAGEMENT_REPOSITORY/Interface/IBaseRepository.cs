using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Interface
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(object id);
        Task<T?> GetByIdAsync(object id, params Expression<Func<T, object>>[] includes);
        Task AddAsync(T entity);
        Task AddBulkAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateBulk(IEnumerable<T> entities);
        void Delete(T entity);
        void DeleteBulk(IEnumerable<T> entities);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    }
}

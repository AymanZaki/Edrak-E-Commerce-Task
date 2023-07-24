using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edrak.Order.Dal.Repository
{
    public interface IOrdersRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T Update(T entity);
        void Update(IEnumerable<T> entities);
        T Add(T entity);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        void Add(IEnumerable<T> items);
        public Task<IEnumerable<T>> GetListAsync(IQueryable<T> entities);
        public Task<T> GetFirstOrDefaultAsync(IQueryable<T> entities);
        Task UpdateAsync(IEnumerable<T> items);
        Task<bool> AnyAsync(IQueryable<T> entities);
        IQueryable<T> Include(IQueryable<T> entities, IEnumerable<string> includes);
        void Delete(IEnumerable<T> entities);
        Task<int> CountAsync(IQueryable<T> entities);
    }
}

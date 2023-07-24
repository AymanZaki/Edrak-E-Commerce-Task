using Edrak.Order.Data;
using Microsoft.EntityFrameworkCore;

namespace Edrak.Order.Dal.Repository
{
    public class OrdersRepository<T> : IOrdersRepository<T> where T : class
    {
        private readonly OrdersDBContext context;
        public OrdersRepository(OrdersDBContext context)
        {
            this.context = context;
        }

        public T Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var entities = this.context.Set<T>();
            var result = entities.Add(entity);

            this.context.SaveChanges();

            return result.Entity;
        }

        public void Add(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("entity");

            var entities = this.context.Set<T>();
            entities.AddRange(items);

            this.context.SaveChanges();
        }

        public IQueryable<T> GetAll()
        {
            var entities = this.context.Set<T>().AsNoTracking();
            return entities;
        }

        public T Update(T entity)
        {
            try
            {
                var entities = this.context.Set<T>();
                var result = entities.Update(entity);
                this.context.SaveChanges();
                return result.Entity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Update(IEnumerable<T> entities)
        {
            try
            {
                var entity = this.context.Set<T>();
                entity.UpdateRange(entities);
                this.context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<T> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var entities = this.context.Set<T>();
            var result = await entities.AddAsync(entity);

            this.context.SaveChanges();

            return result.Entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var entities = this.context.Set<T>();
            var result = entities.Update(entity);

            await this.context.SaveChangesAsync();

            return result.Entity;
        }

        public async Task UpdateAsync(IEnumerable<T> items)
        {
            try
            {
                var entities = this.context.Set<T>();
                entities.UpdateRange(items);
                await this.context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IEnumerable<T>> GetListAsync(IQueryable<T> entities)
        {
            return await entities.ToListAsync();
        }

        public async Task<T> GetFirstOrDefaultAsync(IQueryable<T> entities)
        {
            return await entities.FirstOrDefaultAsync();
        }

        public async Task<bool> AnyAsync(IQueryable<T> entities)
        {
            return await entities.AnyAsync();
        }
        public IQueryable<T> Include(IQueryable<T> entities, IEnumerable<string> includes)
        {
            foreach (string include in includes)
            {
                entities = entities.Include(include);
            }
            return entities;
        }

        public void Delete(IEnumerable<T> entities)
        {
            try
            {
                context.RemoveRange(entities);
                this.context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> CountAsync(IQueryable<T> entities)
        {
            return await entities.CountAsync();
        }
    }
}

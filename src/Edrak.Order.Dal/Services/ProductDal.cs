using Edrak.Order.Dal.Interfaces;
using Edrak.Order.Dal.Repository;
using Edrak.Order.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Edrak.Order.Dal.Services
{
    public class ProductDal : IProductDal
    {

        private readonly IOrdersRepository<Product> _orderRepository;

        public ProductDal(IOrdersRepository<Product> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<Product>> GetProductByIds(IEnumerable<int> productIds)
        {
            var query = _orderRepository.GetAll();
            query = query.Where(x => productIds.Contains(x.Id) && !x.IsDeleted);

            return await _orderRepository.GetListAsync(query);
        }

        public async Task<bool> UpdateProducts(IEnumerable<Product> entities)
        {
            await _orderRepository.UpdateAsync(entities);
            return true;
        }
    }
}

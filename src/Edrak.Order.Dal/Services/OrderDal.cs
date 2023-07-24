using Castle.Core.Resource;
using Edrak.Order.Dal.Interfaces;
using Edrak.Order.Dal.Repository;
using Edrak.Order.Data.Entities;
using Edrak.Order.Models.FilterModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Edrak.Order.Dal.Services
{
    public class OrderDal : IOrderDal
    {
        private readonly IOrdersRepository<Data.Entities.Order> _orderRepository;

        public OrderDal(IOrdersRepository<Data.Entities.Order> orderRepository) 
        {
            _orderRepository = orderRepository;
        }

        public async Task<Data.Entities.Order> CreateOrder(Data.Entities.Order entity)
        {
            return await _orderRepository.AddAsync(entity);
        }


        public async Task<Data.Entities.Order> GetOrderById(int orderId, IEnumerable<string> includes = null)
        {
            var query = _orderRepository.GetAll();
            if (includes != null && includes.Any())
            {
                query = _orderRepository.Include(query, includes);
            }
            query = query.Where(x => x.Id.Equals(orderId));
            return await _orderRepository.GetFirstOrDefaultAsync(query);
        }

        public async Task<Data.Entities.Order> UpdateOrder(Data.Entities.Order entity)
        {
            return await _orderRepository.UpdateAsync(entity);
        }

        public async Task<IEnumerable<Data.Entities.Order>> GetOrderByCustomerId(int customerId, IEnumerable<string> includes = null)
        {
            var query = _orderRepository.GetAll();
            if (includes != null && includes.Any())
            {
                query = _orderRepository.Include(query, includes);
            }
            query = query.Where(x => x.CustomerId.Equals(customerId));
            return await _orderRepository.GetListAsync(query);
        }

        public async Task<IEnumerable<Data.Entities.Order>> GetOrders(OrderFilter filter, IEnumerable<string> includes = null)
        {
            var query = _orderRepository.GetAll();
            if (includes != null && includes.Any())
            {
                query = _orderRepository.Include(query, includes);
            }
            query = query.Skip((filter.Page - 1) * filter.Limit)
                    .Take(filter.Limit);
            return await _orderRepository.GetListAsync(query);
        }

        public async Task<int> GetOrdersCount(OrderFilter filter)
        {
            var query = _orderRepository.GetAll();
            return await _orderRepository.CountAsync(query);
        }
    }
}

using Edrak.Order.Dal.Interfaces;
using Edrak.Order.Dal.Repository;
using Edrak.Order.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edrak.Order.Dal.Services
{
    public class CustomerDal : ICustomerDal
    {
        private readonly IOrdersRepository<Customer> _orderRepository;

        public CustomerDal(IOrdersRepository<Customer> orderRepository)
        {
            _orderRepository = orderRepository;
        }


        public async Task<Customer> GetCustomerById(int customerId)
        {
            var query = _orderRepository.GetAll();

            query = query.Where(x => x.Id.Equals(customerId));
            return await _orderRepository.GetFirstOrDefaultAsync(query);
        }
    }
}

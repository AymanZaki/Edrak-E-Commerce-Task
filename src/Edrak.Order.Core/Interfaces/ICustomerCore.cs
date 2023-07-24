using Edrak.Order.Models;
using Edrak.Order.Models.EntityDTOs;

namespace Edrak.Order.Core.Interfaces
{
    public interface ICustomerCore
    {
        Task<ResultModel<CustomerDTO>> GetCustomerById(int customerId);
    }
}

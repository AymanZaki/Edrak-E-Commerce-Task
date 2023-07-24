
using Edrak.Order.Models.FilterModel;

namespace Edrak.Order.Dal.Interfaces
{
    public interface IOrderDal
    {
        Task<Data.Entities.Order> CreateOrder(Data.Entities.Order entity); 
        Task<Data.Entities.Order> GetOrderById(int orderId, IEnumerable<string> includes = null); 
        Task<Data.Entities.Order> UpdateOrder(Data.Entities.Order entity); 
        Task<IEnumerable<Data.Entities.Order>> GetOrderByCustomerId(int customerId, IEnumerable<string> includes = null); 
        Task<IEnumerable<Data.Entities.Order>> GetOrders(OrderFilter filter, IEnumerable<string> includes = null); 
        Task<int> GetOrdersCount(OrderFilter filter); 
    }
}

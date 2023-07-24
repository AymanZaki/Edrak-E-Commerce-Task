using Edrak.Order.Models;
using Edrak.Order.Models.DTOs;
using Edrak.Order.Models.EntityDTOs;
using Edrak.Order.Models.FilterModel;

namespace Edrak.Order.Core.Interfaces
{
    public interface IOrderCore
    {
        Task<ResultModel<OrderDTO>> CreateOrder(CreateOrderDTO dto);
        Task<ResultModel<OrderDTO>> GetOrderDetails(int orderId);
        Task<ResultModel<bool>> UpdateOrderStatus(int orderId, UpdateOrderStatusDTO dto);
        Task<ResultModel<bool>> CancelOrder(int orderId);
        Task<ResultModel<IEnumerable<OrderDTO>>> GetOrderByCustomerId(int customerId);
        Task<ResultModel<PageModel<OrderDTO>>> GetOrders(OrderFilter filter);
    }
}

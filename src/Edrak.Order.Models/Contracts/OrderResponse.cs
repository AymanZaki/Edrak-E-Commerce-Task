
using Edrak.Order.Models.Enums;

namespace Edrak.Order.Models.Contracts
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public IEnumerable<OrderItemResponse> OrderItems { get; set; }
    }
}

using Edrak.Order.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edrak.Order.Models.Contracts
{
    public class OrderDetailsResponse
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public CustomerResponse Customer { get; set; }
        public IEnumerable<OrderItemResponse> OrderItems { get; set; }
    }
}

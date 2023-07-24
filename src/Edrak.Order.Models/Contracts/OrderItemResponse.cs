using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edrak.Order.Models.Contracts
{
    public class OrderItemResponse
    {
        public int ProductId { get; set; }
        public int ProductName { get; set; }
        public int Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}

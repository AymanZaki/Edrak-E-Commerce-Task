using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edrak.Order.Models.Enums;

namespace Edrak.Order.Models.EntityDTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public IEnumerable<OrderLineItemDTO> OrderLineItems { get; set; }
        public DateTime CreatedDate { get; set; }
        public CustomerDTO Customer { get; set; }
        public OrderStatus Status { get; set; }
    }
}

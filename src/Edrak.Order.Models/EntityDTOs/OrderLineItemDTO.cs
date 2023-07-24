using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edrak.Order.Models.DTOs;

namespace Edrak.Order.Models.EntityDTOs
{
    public class OrderLineItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public OrderProductMetaDataDTO ProductMetaData { get; set; }
        public int Quantity { get; set; }
    }
}

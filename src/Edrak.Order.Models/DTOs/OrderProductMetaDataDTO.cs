using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edrak.Order.Models.DTOs
{
    public class OrderProductMetaDataDTO
    {
        public int ProductId { get; set; }
        public int Name { get; set; }
        public int Description { get; set; }
        public decimal Price { get; set; }
    }
}

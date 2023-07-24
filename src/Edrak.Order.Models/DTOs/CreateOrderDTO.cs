using System.ComponentModel.DataAnnotations;

namespace Edrak.Order.Models.DTOs
{
    public class CreateOrderDTO
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public IEnumerable<OrderProductDTO> Products { get; set; }
    }
}

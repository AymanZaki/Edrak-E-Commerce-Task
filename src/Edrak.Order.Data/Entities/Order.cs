using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Edrak.Order.Data.Entities
{
    public class Order : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        [Column("datetime")]
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public List<OrderLineItem> OrderLineItems { get; set; }
        public Customer Customer { get; set; }
        public OrderStatus Status { get; set; }
    }
}

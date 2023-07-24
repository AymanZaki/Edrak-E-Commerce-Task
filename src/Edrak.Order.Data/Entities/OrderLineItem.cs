using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edrak.Order.Data.Entities
{
    public class OrderLineItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        // snapshot of product (Name, Description, Price)
        public string ProductMetaData { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; }
    }
}

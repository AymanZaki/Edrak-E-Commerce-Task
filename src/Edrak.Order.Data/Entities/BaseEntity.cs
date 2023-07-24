using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edrak.Order.Data.Entities
{
    public class BaseEntity
    {

        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        [MaxLength(64)]
        public string? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedDate { get; set; }
        [MaxLength(64)]
        public string? LastModifiedBy { get; set; }
    }
}

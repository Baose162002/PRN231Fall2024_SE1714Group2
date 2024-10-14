using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    [Table("Batch")]
    public class Batch
    {
        [Key]
        public int BatchId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public int BatchQuantity { get; set; }
        public int RemainingQuantity { get; set; } // Số lượng hoa còn lại trong kho
        public string Description { get; set; }
        public DateTime EntryDate { get; set; }
      
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    
        public Enum.EnumList.Status Status { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Flower> Flowers { get; set; }
    }
}

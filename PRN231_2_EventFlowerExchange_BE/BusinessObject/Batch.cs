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
        public string FlowerType { get; set; }
        public int BatchQuantity { get; set; }
        public string ImgFlower { get; set; }
        public string Description { get; set; }
        public decimal PricePerUnit { get; set; }
        public string Condition { get; set; }
        public DateTime EntryDate { get; set; }
        public Enum.EnumList.BatchStatus BatchStatus { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public int FlowerId { get; set; }
        [ForeignKey("FlowerId")]
        public Flower Flower { get; set; }
        public Enum.EnumList.Status Status { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}

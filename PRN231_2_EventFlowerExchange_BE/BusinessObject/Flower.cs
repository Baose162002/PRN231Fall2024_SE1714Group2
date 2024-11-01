using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BusinessObject
{
    [Table("Flower")]
    public class Flower
    {
        [Key]
        public int FlowerId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public double PricePerUnit { get; set; }
        public string Origin { get; set; }
        public string Color { get; set; }
        public int RemainingQuantity { get; set; } // Số lượng còn lại loại hoa này để bán
        public Enum.EnumList.FlowerCondition Condition { get; set; } // Fresh, Partially Fresh, Damaged

        public Enum.EnumList.FlowerStatus FlowerStatus { get; set; }

        public int BatchId { get; set; }

        [ForeignKey("BatchId")]
        public Enum.EnumList.Status Status { get; set; }

        public Batch Batch { get; set; }
        [JsonIgnore]
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Review> Reviews { get; set; }


    }
}

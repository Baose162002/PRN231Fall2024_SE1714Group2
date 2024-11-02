using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int QuantityOrdered { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        [JsonIgnore]
        public Order Order { get; set; }

        public int FlowerId { get; set; }
        [ForeignKey("FlowerId")]
        public Flower Flower { get; set; }
    }
}

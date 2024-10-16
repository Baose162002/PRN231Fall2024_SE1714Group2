using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace BusinessObject
{
    [Table("Review")]
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Feedback { get; set; }
        public DateTime ReviewDate { get; set; }
        public int FlowerId { get; set; }
        [ForeignKey("FlowerId")]
        [JsonIgnore]
        public Flower Flower { get; set; }
      
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        [JsonIgnore]
        public User Customer { get; set; }
        public Enum.EnumList.Status Status { get; set; }

    }
}

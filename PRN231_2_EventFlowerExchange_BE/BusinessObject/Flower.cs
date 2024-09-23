using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public decimal PricePerUnit { get; set; }
        public string Origin { get; set; }
        public string Color { get; set; }

        public ICollection<Batch> Batches { get; set; }
    }
}

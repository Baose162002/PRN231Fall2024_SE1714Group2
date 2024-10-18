using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Response
{
    public class ListFlowerDTO
    {
        public int FlowerId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public decimal PricePerUnit { get; set; }
        public string Origin { get; set; }
        public string Color { get; set; }
        public int RemainingQuantity { get; set; } 
        public int BatchId { get; set; }
        public string Status { get; set; }
        public ListBatchDTO Batch { get; set; }

    }
}

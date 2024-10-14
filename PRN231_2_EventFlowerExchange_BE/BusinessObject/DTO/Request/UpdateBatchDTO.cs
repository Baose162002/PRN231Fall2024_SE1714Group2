using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Request
{
    public class UpdateBatchDTO
    {
        
        public string FlowerType { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public int BatchQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public string Description { get; set; }
        public decimal PricePerUnit { get; set; }
        public string EntryDate { get; set; }
       

    }
}

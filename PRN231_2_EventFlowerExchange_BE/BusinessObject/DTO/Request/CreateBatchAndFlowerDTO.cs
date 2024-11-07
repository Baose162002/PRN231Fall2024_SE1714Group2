using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Request
{
    public class CreateBatchAndFlowerDTO
    {
        public string BatchName { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EntryDate { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
        public List<FlowerDTOs> Flowers { get; set; }
    }

    public class FlowerDTOs
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Origin { get; set; }
        public string Image { get; set; }
        public double PricePerUnit { get; set; }
        public int RemainingQuantity { get; set; }
        public string Description { get; set; }

    }

}

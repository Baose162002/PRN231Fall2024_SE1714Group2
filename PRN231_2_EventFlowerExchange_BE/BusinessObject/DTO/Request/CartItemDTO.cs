using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Request
{
    public class CartItemDTO
    {
        public string FlowerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PricePerUnit { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; } 
    }

}

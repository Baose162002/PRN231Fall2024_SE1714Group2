using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Response
{
    public class ListOrderForDeliveryDTO
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public double TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryDate { get; set; }
        public List<OrderDetailForDeliveryDTO> OrderDetails { get; set; }
    }
    public class OrderDetailForDeliveryDTO
    {
        public int OrderDetailId { get; set; }
        public int QuantityOrdered { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        public FlowerDTO Flower { get; set; } // Flower details if needed
    }
    public class FlowerDTO
    {
        public int FlowerId { get; set; }
        public string Name { get; set; }
        public double PricePerUnit { get; set; } // Add other properties as needed
    }

}

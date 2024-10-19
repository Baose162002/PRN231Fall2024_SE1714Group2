using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Request
{
    public class CreateOrderFlowerDTO
    {
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int CustomerId { get; set; }
        public List<OrderDetailDTOs> OrderDetails { get; set; } // Change to a list
    }

    public class OrderDetailDTOs
    {
        public int FlowerId { get; set; }
        public int QuantityOrdered { get; set; }
    }
}

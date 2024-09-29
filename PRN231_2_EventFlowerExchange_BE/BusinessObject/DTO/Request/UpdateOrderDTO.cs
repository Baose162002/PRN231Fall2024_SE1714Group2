using BusinessObject.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Request
{
    public class UpdateOrderDTO
    {
        public Enum.EnumList.OrderStatus OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int CustomerId { get; set; }
        public ICollection<OrderDetailDTO> OrderDetails { get; set; }
    }
}

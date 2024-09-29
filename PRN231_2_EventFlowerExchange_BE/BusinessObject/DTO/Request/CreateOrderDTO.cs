using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO.Response;

namespace BusinessObject.DTO.Request
{
    public class CreateOrderDTO
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

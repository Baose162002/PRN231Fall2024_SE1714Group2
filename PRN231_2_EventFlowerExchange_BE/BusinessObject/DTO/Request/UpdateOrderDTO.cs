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
        public string OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public string OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryDate { get; set; }
        public int CustomerId { get; set; }
        public int OrderDetails { get; set; }
    }
}

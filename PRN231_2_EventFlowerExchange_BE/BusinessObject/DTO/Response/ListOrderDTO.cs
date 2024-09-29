using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Response
{
    public class ListOrderDTO
    {
        public int OrderId { get; set; }
        public Enum.EnumList.OrderStatus OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string CustomerName { get; set; } // Thêm tên khách hàng để hiển thị dễ dàng hơn
    }
}

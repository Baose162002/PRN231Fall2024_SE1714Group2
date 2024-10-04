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
        public string OrderStatus { get; set; }

        // TotalPrice is calculated as the sum of all OrderDetailDTO TotalPrice values
        public decimal TotalPrice => OrderDetails != null ? OrderDetails.Sum(od => od.TotalPrice) : 0;

        public string OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryDate { get; set; }

        public ListUserDTO Customer { get; set; } // Display customer details easily
        public List<OrderDetailDTO> OrderDetails { get; set; }
        public string IdempotencyKey { get; set; }
    }

}

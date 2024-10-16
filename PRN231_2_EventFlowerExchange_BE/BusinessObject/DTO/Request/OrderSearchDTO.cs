using BusinessObject.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Request
{
    public class OrderSearchDTO
    {
        public Enum.EnumList.OrderStatus? Status { get; set; } // Có thể để null để không lọc theo Status
        public DateTime? OrderDateFrom { get; set; } // Thời gian bắt đầu
        public DateTime? OrderDateTo { get; set; } // Thời gian kết thúc
        public int? CustomerId { get; set; } // ID khách hàng
    }

}

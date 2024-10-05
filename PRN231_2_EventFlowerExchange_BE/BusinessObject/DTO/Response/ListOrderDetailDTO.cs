using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Response
{
    public class ListOrderDetailDTO
    {
        public int BatchId { get; set; }  // Chỉ cần nhập BatchId
        public int QuantityOrdered { get; set; }  // Số lượng muốn đặt mua
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO.Response;

namespace BusinessObject.DTO.Request
{
    public class CreateOrderDetailDTO
    {
        public int OrderDetailId { get; set; }
    }
}

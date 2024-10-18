using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Response
{
    public class CreateOrderResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public int OrderId { get; set; }
    }
}

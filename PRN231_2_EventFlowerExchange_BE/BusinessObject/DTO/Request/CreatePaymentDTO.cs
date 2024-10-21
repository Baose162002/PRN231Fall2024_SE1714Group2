using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.Enum.EnumList;

namespace BusinessObject.DTO.Request
{
    public class CreatePaymentDTO
    {
        public PaymentStatus PaymentStatus { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }

        public int OrderId { get; set; }
    }
}

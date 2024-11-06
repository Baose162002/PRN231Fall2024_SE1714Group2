using BusinessObject.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Request
{
    public class UpdateDeliveryDTO
    {
        public EnumList.DeliveryStatus DeliveryStatus { get; set; }
        public string ReasonNote { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime DeliveryTime { get; set; }
        public int DeliveryPersonnelId { get; set; }
        public int OrderId { get; set; }
    }
}

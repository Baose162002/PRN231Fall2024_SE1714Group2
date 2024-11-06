using BusinessObject.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Response
{
    public class ListDeliveryDTO
    {
        public int DeliveryId { get; set; }
        public EnumList.DeliveryStatus DeliveryStatus { get; set; }
        public string ReasonNote { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime DeliveryTime { get; set; }
        public OrderResponse Order { get; set; }
        public int DeliveryPersonnelId { get; set; }
    }
}

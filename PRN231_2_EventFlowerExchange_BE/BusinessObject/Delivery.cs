using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    [Table("Delivery")]
    public class Delivery
    {
        [Key]
        public int DeliveryId { get; set; }
        public Enum.EnumList.DeliveryStatus DeliveryStatus { get; set; }
        public string ReasonNote { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime DeliveryTime { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public int DeliveryPersonnelId { get; set; }
        [ForeignKey("DeliveryPersonnelId")]
        public User DeliveryPersonnel { get; set; }
    }
}

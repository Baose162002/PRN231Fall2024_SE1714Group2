using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Enum
{
    public class EnumList
    {
        public enum OrderStatus
        {
            Pending = 1,
            Paid = 2,
            InTransit = 3,
            ShippingCompleted = 4,
            Cancel = 5,
        }

        public enum PaymentStatus
        {
            Pending = 1,
            Completed = 2,
            Failed =3 
        }

        public enum FlowerStatus
        {
            Available =1,
            SoldOut = 2
            
        }

        public enum UserRole
        {
            Admin = 1,
            Seller = 2,
            Buyer = 3,
            DeliveryPersonnel = 4
        }

        public enum FlowerCondition
        {
            Fresh = 1,              // Còn tốt
            Damaged = 2             // Hư hại
        }
        public enum DeliveryStatus
        {
            InTransit = 1,
            Complete = 2,
            Cancel = 3,
        }

        public enum Status
        {
            Active = 1,
            Inactive = 2,
            Overdue = 3
        }
    }
}

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
            Confirmed = 2,
            Dispatched = 3,
            Delivered = 4
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
            PartiallyFresh = 2,     // Còn tốt 1 nửa
            Damaged = 3             // Hư hại
        }
        public enum DeliveryStatus
        {
            Pending = 1,
            OnRoute = 2,
            Delivered = 3
        }

        public enum Status
        {
            Active = 1,
            Inactive = 2,
            NeedsReview = 3
        }
    }
}

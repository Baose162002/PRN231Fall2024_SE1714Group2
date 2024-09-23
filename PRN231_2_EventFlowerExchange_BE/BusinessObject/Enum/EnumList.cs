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
            Pending,
            Confirmed,
            Dispatched,
            Delivered
        }

        public enum PaymentStatus
        {
            Pending,
            Completed,
            Failed
        }

        public enum BatchStatus
        {
            Available,
            SoldOut,
            Expired
        }

        public enum UserRole
        {
            Seller,
            Buyer,
            Admin,
            DeliveryPersonnel
        }

        public enum DeliveryStatus
        {
            Pending,
            OnRoute,
            Delivered
        }
    }
}

﻿using System;
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
            SoldOut
            
        }

        public enum UserRole
        {
            Admin,
            Seller,
            Buyer,
            DeliveryPersonnel
        }

        public enum DeliveryStatus
        {
            Pending,
            OnRoute,
            Delivered
        }

        public enum Status
        {
            Active,
            Inactive
        }
    }
}

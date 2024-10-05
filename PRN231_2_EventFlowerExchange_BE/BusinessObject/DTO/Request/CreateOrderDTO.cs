﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO.Response;

namespace BusinessObject.DTO.Request
{
    public class CreateOrderDTO
    {
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int CustomerId { get; set; }

        public int FlowerId { get; set; }  // Chỉ cần truyền ID của loại hoa
        public int QuantityOrdered { get; set; }  // Số lượng tổng cần đặt
        //public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}

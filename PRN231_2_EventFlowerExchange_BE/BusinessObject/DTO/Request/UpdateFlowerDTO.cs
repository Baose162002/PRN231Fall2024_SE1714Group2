﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Request
{
    public class UpdateFlowerDTO
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public decimal PricePerUnit { get; set; }
        public string Origin { get; set; }
        public string Color { get; set; }
        public int RemainingQuantity { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Request
{
    public class CreateBatchDTOUpdateImg
    {
        public string FlowerType { get; set; }
        public int BatchQuantity { get; set; }
        public string Description { get; set; }
        public decimal PricePerUnit { get; set; }
        public string Condition { get; set; }
        public string EntryDate { get; set; }
        public int CompanyId { get; set; }
        public int FlowerId { get; set; }

    }
}

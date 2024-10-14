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
        public string EventName { get; set; }
        public string EventDate { get; set; }
        public int BatchQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public string Description { get; set; }
        public string EntryDate { get; set; }
        public int CompanyId { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Request
{
    public class CreateBatchDTO
    {
        public string BatchName { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public int BatchQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public string Description { get; set; }
        public DateTime EntryDate { get; set; }
        public int CompanyId { get; set; }
     
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Response
{
    public class CompanyDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyAddress { get; set; } = string.Empty;
        public string CompanyDescription { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

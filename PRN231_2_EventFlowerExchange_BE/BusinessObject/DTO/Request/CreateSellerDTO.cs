using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Request
{
    public class CreateSellerDTO
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyDescription { get; set; }
    }
}

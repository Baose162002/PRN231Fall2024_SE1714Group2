using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Request
{
    public class RegisterSellerDTO
    {
        public CreateSellerDTO CreateCompany { get; set; }
        public CreateUserDTO CreateUser { get; set; }
    }
}

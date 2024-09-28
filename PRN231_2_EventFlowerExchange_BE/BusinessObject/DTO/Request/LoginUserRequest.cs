using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dto.Request
{
    public class LoginUserRequest
    {
        [Required(ErrorMessage = "Yêu cầu Email"), EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Yêu cầu mật khẩu")]
        public string Password { get; set; }
    }
}

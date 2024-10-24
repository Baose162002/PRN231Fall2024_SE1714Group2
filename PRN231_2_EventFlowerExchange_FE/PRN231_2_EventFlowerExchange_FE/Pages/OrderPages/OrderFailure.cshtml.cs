using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PRN231_2_EventFlowerExchange_FE.Pages.OrderPages
{
    public class OrderFailureModel : PageModel
    {
        public string ErrorMessage { get; set; }

        public void OnGet(string message)
        {
            ErrorMessage = message;
        }
    }

}

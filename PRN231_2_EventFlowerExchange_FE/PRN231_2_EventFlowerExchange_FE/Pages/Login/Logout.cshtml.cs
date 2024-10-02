using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PRN231_2_EventFlowerExchange_FE.Pages.Login
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Explicitly delete the session cookie
            Response.Cookies.Delete(".AspNetCore.Session");

            // Redirect to the home page or login page
            return RedirectToPage("/Index");
        }
    }
}
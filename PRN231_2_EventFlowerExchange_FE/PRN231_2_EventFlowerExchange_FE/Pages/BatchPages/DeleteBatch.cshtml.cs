using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PRN231_2_EventFlowerExchange_FE.Pages.BatchPages
{
    public class DeleteBatchModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public DeleteBatchModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string BaseApiUrl { get; private set; }

        public void OnGet()
        {
            BaseApiUrl = _configuration["ApiSettings:BaseUrl"];
        }
    }
}

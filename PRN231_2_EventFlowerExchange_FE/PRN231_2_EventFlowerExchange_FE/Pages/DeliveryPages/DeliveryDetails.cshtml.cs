using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObject;

namespace PRN231_2_EventFlowerExchange_FE.Pages.DeliveryPages
{
    public class DetailsModel : PageModel
    {
        private readonly BusinessObject.FlowerShopContext _context;

        public DetailsModel(BusinessObject.FlowerShopContext context)
        {
            _context = context;
        }

        public Delivery Delivery { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries.FirstOrDefaultAsync(m => m.DeliveryId == id);
            if (delivery == null)
            {
                return NotFound();
            }
            else
            {
                Delivery = delivery;
            }
            return Page();
        }
    }
}

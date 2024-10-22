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
    public class IndexModel : PageModel
    {
        private readonly BusinessObject.FlowerShopContext _context;

        public IndexModel(BusinessObject.FlowerShopContext context)
        {
            _context = context;
        }

        public IList<Delivery> Delivery { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Delivery = await _context.Deliveries
                .Include(d => d.DeliveryPersonnel)
                .Include(d => d.Order).ToListAsync();
        }
    }
}

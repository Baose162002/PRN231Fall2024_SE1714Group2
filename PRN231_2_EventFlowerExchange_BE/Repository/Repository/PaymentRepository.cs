using BusinessObject;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        public async Task CreatePayment(Payment payment)
        {
            var _context = new FlowerShopContext();
           
            _context.Payments.Add(payment);
            _context.SaveChanges();
        }
    }
}

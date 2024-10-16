using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IPaymentRepository
    {
        Task CreatePayment(Payment payment);
    }
}

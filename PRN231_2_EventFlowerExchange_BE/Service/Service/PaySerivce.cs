using BusinessObject;
using BusinessObject.Enum;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class PaySerivce : IPayService
    {
        private readonly IPaymentRepository _paymentRepository;
        public PaySerivce(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }
        public async Task CreatePayment(Payment payment)
        {
            await _paymentRepository.CreatePayment(payment);
           
        }
    }
}

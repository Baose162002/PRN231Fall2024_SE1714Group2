using AutoMapper;
using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
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
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public PaySerivce(IPaymentRepository paymentRepository, IOrderRepository orderRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }
        public async Task CreatePayment(CreatePaymentDTO payment)
        {

            var orderexist = await _orderRepository.GetOrderById(payment.OrderId);
            if(orderexist == null)
            {
                throw new ArgumentException("Order not found");
            }

            Payment paymentdto = new Payment
            {
                PaymentDate = payment.PaymentDate,
                AmountPaid = payment.AmountPaid,
                PaymentStatus = payment.PaymentStatus
            };
            await _paymentRepository.CreatePayment(_mapper.Map<Payment>(paymentdto));
           
        }
        public async Task Create(Payment payment)
        {
            await _paymentRepository.CreatePayment(payment);

        }
    }
}

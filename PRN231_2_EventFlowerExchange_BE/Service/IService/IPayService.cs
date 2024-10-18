using BusinessObject;
using BusinessObject.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IPayService
    {
        Task Create(Payment payment);
        Task CreatePayment(CreatePaymentDTO payment);
    }
}

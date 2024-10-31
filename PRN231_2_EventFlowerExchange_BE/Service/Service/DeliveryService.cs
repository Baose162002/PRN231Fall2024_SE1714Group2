using AutoMapper;
using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Repository.IRepository;
using Repository.Repository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Service
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IMapper _mapper;

        public DeliveryService(IDeliveryRepository deliveryRepository, IMapper mapper)
        {
            _deliveryRepository = deliveryRepository;
            _mapper = mapper;
        }

        public async Task<List<ListDeliveryDTO>> GetAllDeliveries()
        {
            var deliveries = await _deliveryRepository.GetAllDeliveries();
            var deliveryDTOs = _mapper.Map<List<ListDeliveryDTO>>(deliveries);
            return deliveryDTOs;
        }

        public async Task<ListDeliveryDTO> GetDeliveryById(int id)
        {
            var delivery = await _deliveryRepository.GetDeliveryById(id);
            if (delivery == null)
            {
                throw new ArgumentException("Delivery not found.");
            }

            var deliveryDTO = _mapper.Map<ListDeliveryDTO>(delivery);
            return deliveryDTO;
        }

        public async Task CreateDelivery(CreateDeliveryDTO deliveryDTO)
        {
            if (deliveryDTO == null)
            {
                throw new ArgumentException("Invalid input data.");
            }

            var delivery = _mapper.Map<Delivery>(deliveryDTO);
            await _deliveryRepository.Create(delivery);
        }

        public async Task UpdateDelivery(UpdateDeliveryDTO deliveryDTO, int id)
        {
            if (deliveryDTO == null)
            {
                throw new ArgumentException("Invalid input data.");
            }

            var delivery = _mapper.Map<Delivery>(deliveryDTO);
            await _deliveryRepository.Update(delivery, id);
        }

        public async Task DeleteDelivery(int id)
        {
            await _deliveryRepository.Delete(id);
        }


        public async Task<List<ListOrderForDeliveryDTO>> GetAllOrdersAsync()
        {
            // Call the repository to get all orders
            var orders = await _deliveryRepository.GetAllOrderForDelivery();

            // Map to DTOs
            return orders.Select(o => new ListOrderForDeliveryDTO
            {
                OrderId = o.OrderId,
                CustomerName = o.Customer?.FullName, // Assuming Customer has a Name property
                TotalPrice = o.TotalPrice,
                OrderDate = o.OrderDate,
                DeliveryAddress = o.DeliveryAddress,
                DeliveryDate = o.DeliveryDate,
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailForDeliveryDTO
                {
                    OrderDetailId = od.OrderDetailId,
                    QuantityOrdered = od.QuantityOrdered,
                    Price = od.Price,
                    TotalPrice = od.TotalPrice,
                    Flower = new FlowerDTO // Assuming you want to include Flower details
                    {
                        FlowerId = od.Flower.FlowerId,
                        Name = od.Flower.Name,
                        PricePerUnit = od.Flower.PricePerUnit // Include other properties as needed
                    }
                }).ToList()
            }).ToList();
        }
    }
}

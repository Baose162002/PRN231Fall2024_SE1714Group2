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
using static BusinessObject.Enum.EnumList;

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

        public async Task<List<ListDeliveryDTO>> GetDeliveryById(int id)
        {
            var deliveries = await _deliveryRepository.GetDeliveriesByPersonnelId(id);
            if (deliveries == null || !deliveries.Any())
            {
                throw new ArgumentException("No deliveries found for the given personnel ID.");
            }

            // Map the list of deliveries to ListDeliveryDTO
            var deliveryDTOs = _mapper.Map<List<ListDeliveryDTO>>(deliveries);
            return deliveryDTOs;
        }

            public async Task CreateDelivery(CreateDeliveryDTO deliveryDTO)
        {
            if (deliveryDTO == null)
            {
                throw new ArgumentException("Invalid input data.");
            }

            // Validate that the OrderId exists
            var orderExists = await _deliveryRepository.ExistsOrder(deliveryDTO.OrderId);
            if (!orderExists)
            {
                throw new ArgumentException($"Order with ID {deliveryDTO.OrderId} does not exist.");
            }

            // Check if the OrderId already has an existing delivery
            var deliveryExists = await _deliveryRepository.ExistsDeliveryForOrder(deliveryDTO.OrderId);
            if (deliveryExists)
            {
                throw new ArgumentException($"Delivery for Order ID {deliveryDTO.OrderId} already exists.");
            }

            // Validate that the DeliveryPersonnelId exists
            var deliveryPersonnelExists = await _deliveryRepository.ExistsUser(deliveryDTO.DeliveryPersonnelId);
            if (!deliveryPersonnelExists)
            {
                throw new ArgumentException($"Delivery personnel with ID {deliveryDTO.DeliveryPersonnelId} does not exist.");
            }

            // Map deliveryDTO to the Delivery entity
            var delivery = _mapper.Map<Delivery>(deliveryDTO);

            // Set default values for DeliveryStatus and Status
            delivery.DeliveryStatus = DeliveryStatus.InTransit; // Assuming "InTransit" is a value in the DeliveryStatus enum
            delivery.Status = Status.Active; // Assuming "Active" is a value in the Status enum

            // Save the delivery
            await _deliveryRepository.Create(delivery);
            await _deliveryRepository.UpdateOrderStatus(deliveryDTO.OrderId);
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
        public async Task UpdateDeliveryStatus(int deliveryId, int orderId)
        {
            await _deliveryRepository.UpdateDeliveryStatus(deliveryId, orderId);
        }
    }
}

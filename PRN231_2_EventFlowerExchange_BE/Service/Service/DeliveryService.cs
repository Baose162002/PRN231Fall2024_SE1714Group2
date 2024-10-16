using AutoMapper;
using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Repository.IRepository;
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
    }
}

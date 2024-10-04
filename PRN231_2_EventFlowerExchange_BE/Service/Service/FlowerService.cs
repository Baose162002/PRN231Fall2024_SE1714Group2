using AutoMapper;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using BusinessObject;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.Enum.EnumList;
using BusinessObject.Enum;

namespace Service.Service
{
    public class FlowerService : IFlowerService
    {
        private readonly IFlowerRepository _flowerRepository;
        private readonly IMapper _mapper;

        public FlowerService(IFlowerRepository flowerRepository, IMapper mapper)
        {
            _flowerRepository = flowerRepository;
            _mapper = mapper;
        }

        public async Task<List<ListFlowerDTO>> GetAllFlowers()
        {
            var flowers = await _flowerRepository.GetAllFlowers();
            var flowerDTOs = _mapper.Map<List<ListFlowerDTO>>(flowers);
            return flowerDTOs;
        }

        public async Task<ListFlowerDTO> GetFlowerById(int id)
        {
            var flower = await _flowerRepository.GetFlowerById(id);
            if (flower == null)
            {
                throw new ArgumentException("Flower not found.");
            }

            var flowerDTO = _mapper.Map<ListFlowerDTO>(flower);
            return flowerDTO;
        }

        public async Task Create(CreateFlowerDTO flowerDTO)
        {
            if (flowerDTO == null || string.IsNullOrEmpty(flowerDTO.Name))
            {
                throw new ArgumentException("Invalid input data.");
            }

            var flower = _mapper.Map<Flower>(flowerDTO);
            await _flowerRepository.Create(flower);
        }
        public async Task<int> CreateFlower(CreateFlowerDTO flowerDTO)
        {
            if (flowerDTO == null || string.IsNullOrEmpty(flowerDTO.Name))
            {
                throw new ArgumentException("Invalid input data.");
            }

            var flower = _mapper.Map<Flower>(flowerDTO);
            flower.Status = Status.Active;
            await _flowerRepository.Create(flower);

            return flower.FlowerId; // Return the ID of the created flower
        }

        public async Task Update(UpdateFlowerDTO flowerDTO, int id)
        {
            if (flowerDTO == null || string.IsNullOrEmpty(flowerDTO.Name))
            {
                throw new ArgumentException("Invalid input data.");
            }

            var flower = _mapper.Map<Flower>(flowerDTO);
            await _flowerRepository.Update(flower, id);
        }

        public async Task Delete(int id)
        {
            await _flowerRepository.Delete(id);
        }
    }
}

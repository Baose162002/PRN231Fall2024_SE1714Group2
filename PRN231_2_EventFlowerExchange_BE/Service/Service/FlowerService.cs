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
using Repository.Repository;

namespace Service.Service
{
    public class FlowerService : IFlowerService
    {
        private readonly IFlowerRepository _flowerRepository;
        private readonly IMapper _mapper;
        private readonly IBatchRepository _batchRepository;
        public FlowerService(IFlowerRepository flowerRepository, IMapper mapper, IBatchRepository batchRepository)
        {
            _flowerRepository = flowerRepository;
            _mapper = mapper;
            _batchRepository = batchRepository;
        }

        public async Task<List<Flower>> GetAllFlowers()
        {
            var flowers = await _flowerRepository.GetAllFlowers();
            var flowerDTOs = _mapper.Map<List<Flower>>(flowers);
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

            // Step 1: Retrieve the Batch by BatchId
            var batch = await _batchRepository.GetBatchById(flowerDTO.BatchId);
            if (batch == null)
            {
                throw new ArgumentException("Invalid Batch ID.");
            }

            // Step 2: Validate the Flower's RemainingQuantity against Batch's RemainingQuantity
            if (flowerDTO.RemainingQuantity > batch.RemainingQuantity)
            {
                throw new ArgumentException($"The flower's remaining quantity ({flowerDTO.RemainingQuantity}) exceeds the batch's remaining quantity ({batch.RemainingQuantity}).");
            }

            // Step 3: Map the FlowerDTO to the Flower Entity and Set Default Values
            Flower flower = new Flower
            {
                Name = flowerDTO.Name,
                Type = flowerDTO.Type,
                Image = flowerDTO.Image,
                Description = flowerDTO.Description,
                PricePerUnit = flowerDTO.PricePerUnit,
                Origin = flowerDTO.Origin,
                Color = flowerDTO.Color,
                RemainingQuantity = flowerDTO.RemainingQuantity,
                Condition = EnumList.FlowerCondition.Fresh,
                FlowerStatus = EnumList.FlowerStatus.Available,
                BatchId = flowerDTO.BatchId,
                Status = EnumList.Status.Active
            };

            // Step 4: Update Batch RemainingQuantity
            batch.RemainingQuantity -= flowerDTO.RemainingQuantity;
            if (batch.RemainingQuantity < 0)
            {
                throw new ArgumentException("Insufficient batch quantity.");
            }

            // Step 5: Update the Batch's RemainingQuantity in the database
            await _batchRepository.UpdateBatch(batch);

            // Step 6: Save the Flower to the database
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

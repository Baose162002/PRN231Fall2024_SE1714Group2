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
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

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

        public async Task<Flower> GetFlowerById(int id)
        {
            var flower = await _flowerRepository.GetFlowerById(id);
            if (flower == null)
            {
                throw new ArgumentException("Flower not found.");
            }

            var flowerDTO = _mapper.Map<Flower>(flower);
            return flowerDTO;
        }

        public async Task Create(CreateFlowerDTO flowerDTO)
        {
            if (flowerDTO == null || string.IsNullOrEmpty(flowerDTO.Name))
            {
                throw new ArgumentException("Invalid input data.");
            }

            var batch = await _batchRepository.GetBatchById(flowerDTO.BatchId);
            if (batch == null)
            {
                throw new ArgumentException("Invalid Batch ID.");
            }

            if (flowerDTO.RemainingQuantity > 0)
            {
                throw new ArgumentException($"The flower's remaining quantity greater than 0.");
            }

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

            // Step 6: Save the Flower to the database
            await _flowerRepository.Create(flower);
        }

        public async Task<int> CreateFlower(CreateFlowerDTO flowerDTO)
        {
            // Validate input data
            if (flowerDTO == null || string.IsNullOrEmpty(flowerDTO.Name))
            {
                throw new ArgumentException("Invalid input data.");
            }

            // Check if the batch exists
            var batch = await _batchRepository.GetBatchById(flowerDTO.BatchId);
            if (batch == null)
            {
                throw new ArgumentException("Invalid Batch ID.");
            }

            // Ensure the remaining quantity is non-negative
            if (flowerDTO.RemainingQuantity < 0)
            {
                throw new ArgumentException("The flower's remaining quantity must be greater than or equal to 0.");
            }

            // Check if a flower with the same name, color, and type already exists
            var existingFlower = await _flowerRepository.GetFlowerByNameColorType(flowerDTO.Name, flowerDTO.Color, flowerDTO.Type);

            if (existingFlower != null)
            {
                // If it exists, update the RemainingQuantity
                existingFlower.RemainingQuantity += flowerDTO.RemainingQuantity;
                await _flowerRepository.UpdateFlower(existingFlower);
                return existingFlower.FlowerId; // Return the existing Flower ID
            }
            else
            {
                // Map the DTO to a new Flower entity
                var flower = _mapper.Map<Flower>(flowerDTO);

                // Set additional properties
                flower.Condition = EnumList.FlowerCondition.Fresh;
                flower.FlowerStatus = EnumList.FlowerStatus.Available;
                flower.BatchId = flowerDTO.BatchId;
                flower.Status = EnumList.Status.Active;

                // Save the new flower to the database
                await _flowerRepository.Create(flower);

                return flower.FlowerId; // Return the new Flower ID
            }
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

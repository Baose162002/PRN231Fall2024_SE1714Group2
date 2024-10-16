using AutoMapper;
using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using BusinessObject.Enum;
using Microsoft.Extensions.Logging;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.Service
{
    public class BatchService : IBatchService
    {
        private readonly IBatchRepository _batchRepository;
        private readonly IMapper _mapper;
       
        public BatchService(IBatchRepository batchRepository, IMapper mapper)
        {
            _batchRepository = batchRepository;
            _mapper = mapper;
        }
        public async Task<List<Batch>> GetAllBatch()
        {
            var batches = await _batchRepository.GetAllBatch();
            return batches;
        }
        
        public async Task<List<ListBatchDTO>> GetAvailableBatchesByFlowerId(int flowerId)
        {
            var batches = await _batchRepository.GetAvailableBatchesByFlowerId(flowerId);
            var batchDTO = _mapper.Map<List<ListBatchDTO>>(batches);
            return batchDTO;
        }

        public async Task<ListBatchDTO> GetBatchById(int id)
        {
            var batches = await _batchRepository.GetBatchById(id);
            ListBatchDTO batchDTO = _mapper.Map<ListBatchDTO>(batches);
            return batchDTO;
        }

        public async Task Create(CreateBatchDTO batch)
        {
            if (batch == null || string.IsNullOrWhiteSpace(batch.BatchName)
              || string.IsNullOrWhiteSpace(batch.Description)
            || batch.BatchQuantity == null || batch.CompanyId == null)
            {
                throw new ArgumentException("All fieds must be filled");
            }

          
            if (batch.BatchQuantity < 0)
            {
                throw new ArgumentException("Quantity minimum must be a positive number");
            }
            if (batch.RemainingQuantity < 0)
            {
                throw new ArgumentException("RemainingQuantity minimum must be a positive number");
            }

            if (batch.RemainingQuantity > batch.BatchQuantity)
            {
                throw new ArgumentException("RemainingQuantity cannot exceed BatchQuantity.");
            }


            Batch batches = new Batch
            {
                BatchName = batch.BatchName,
                EventName = batch.EventName,
                EventDate = batch.EventDate,
                BatchQuantity = batch.BatchQuantity,
                RemainingQuantity = batch.RemainingQuantity,
                Description = batch.Description,
                EntryDate = batch.EntryDate,
                CompanyId = batch.CompanyId,
                Status = EnumList.Status.Active
            };
            await _batchRepository.Create(_mapper.Map<Batch>(batches));
        }

        public async Task Update(UpdateBatchDTO updateBatchDTO, int id)
        {
            if (updateBatchDTO == null || string.IsNullOrWhiteSpace(updateBatchDTO.BatchName)
             || string.IsNullOrWhiteSpace(updateBatchDTO.Description)
           || updateBatchDTO.BatchQuantity == null)
            {
                throw new ArgumentException("All fieds must be filled");
            }
            if (updateBatchDTO.RemainingQuantity > updateBatchDTO.BatchQuantity)
            {
                throw new ArgumentException("RemainingQuantity cannot exceed BatchQuantity.");
            }


            if (updateBatchDTO.BatchQuantity < 0)
            {
                throw new ArgumentException("Quantity minimum must be a positive number");
            }
            if (updateBatchDTO.RemainingQuantity < 0)
            {
                throw new ArgumentException("RemainingQuantity minimum must be a positive number");
            }
    
            Batch existing = await _batchRepository.GetBatchById(id);
            if(existing == null)
            {
                throw new ArgumentException("Batch is not existed");
            }
           

            var updatebatch = _mapper.Map<Batch>(updateBatchDTO);
            await _batchRepository.Update(updatebatch, id);
        }

        public async Task Delete(int id)
        {
            await _batchRepository.Delete(id);
        }
    }
}

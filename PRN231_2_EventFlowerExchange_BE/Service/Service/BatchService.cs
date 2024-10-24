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
            var batches = await _batchRepository.GetFlowersBySimilarTypeAndColorAndEarliestBatch(flowerId);
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
                || batch.CompanyId == null)
            {
                throw new ArgumentException("All fieds must be filled");
            }


            Batch batches = new Batch
            {
                BatchName = batch.BatchName,
                EventName = batch.EventName,
                EventDate = batch.EventDate,
                BatchQuantity = 0,
                RemainingQuantity = 0,
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
             || string.IsNullOrWhiteSpace(updateBatchDTO.Description))
            {
                throw new ArgumentException("All fieds must be filled");
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

        public async Task CheckAndUpdateBatchStatus()
        {
            await _batchRepository.CheckAndUpdateBatchStatus();
        }

    }
}

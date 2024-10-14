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
        public async Task<List<ListBatchDTO>> GetAllBatch()
        {
            var batches = await _batchRepository.GetAllBatch();
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
            if (batch == null
              || string.IsNullOrEmpty(batch.Description)
            || batch.PricePerUnit == null || batch.BatchQuantity == null || batch.CompanyId == null)
            {
                throw new ArgumentException("All fieds must be filled");
            }
            if (batch.PricePerUnit < 0)
            {
                throw new ArgumentException("Price minimum must be a positive number");
            }
            if (batch.BatchQuantity < 0)
            {
                throw new ArgumentException("Quantity minimum must be a positive number");
            }
            string[] dateFormats = { "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy" };
            DateTime entryDate, expirationDate;
            if (!DateTime.TryParseExact(batch.EntryDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out entryDate))
            {
                throw new ArgumentException("Invalid create date format", nameof(batch.EntryDate));
            }
         
        

            Batch batches = new Batch
            {
                EventName = batch.EventName,
                EventDate = batch.EventDate,
                BatchQuantity = batch.BatchQuantity,
                RemainingQuantity = batch.RemainingQuantity,
                Description = batch.Description,
                EntryDate = entryDate,
                CompanyId = batch.CompanyId,
                Status = EnumList.Status.Active
            };
            await _batchRepository.Create(_mapper.Map<Batch>(batches));
        }

        public async Task Update(UpdateBatchDTO updateBatchDTO, int id)
        {
            if (updateBatchDTO == null || string.IsNullOrEmpty(updateBatchDTO.FlowerType)
             || string.IsNullOrEmpty(updateBatchDTO.Description)
           || updateBatchDTO.PricePerUnit == null || updateBatchDTO.BatchQuantity == null)
            {
                throw new ArgumentException("All fieds must be filled");
            }

            if (updateBatchDTO.PricePerUnit < 0)
            {
                throw new ArgumentException("Price minimum must be a positive number");
            }
            if (updateBatchDTO.BatchQuantity < 0)
            {
                throw new ArgumentException("Quantity minimum must be a positive number");
            }
            string[] dateFormats = { "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy" };
            DateTime entryDate, expirationDate;
            if (!DateTime.TryParseExact(updateBatchDTO.EntryDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out entryDate))
            {
                throw new ArgumentException("Invalid create date format", nameof(updateBatchDTO.EntryDate));
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

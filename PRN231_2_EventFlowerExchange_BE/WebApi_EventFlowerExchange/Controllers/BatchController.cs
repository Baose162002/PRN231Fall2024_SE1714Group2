using BusinessObject.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Query;
using Service.IService;

namespace WebApi_EventFlowerExchange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly IBatchService _batchService;
        public BatchController(IBatchService batchService)
        {
            _batchService = batchService;
        }

       
        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var batches = await _batchService.GetAllBatch();
            if(batches == null || !batches.Any())
            {
                return NotFound("Batch Emmty");
            }
            return Ok(batches);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBatchById(int id)
        {
            var batches = await _batchService.GetBatchById(id);
            if(batches == null)
            {
                return NotFound("Batch is not existed");
            }
            return Ok(batches);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBatch(CreateBatchDTO createBatchDTO)
        {
            try
            {
                var batchDto = await _batchService.Create(createBatchDTO);

                return Ok(batchDto); 
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBatchAndFlowers([FromBody] CreateBatchAndFlowerDTO batchAndFlowerDTO)
        {
            try
            {
                var result = await _batchService.CreateBatchAndFlowersAsync(batchAndFlowerDTO);
                return Ok (result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBatch([FromBody]UpdateBatchDTO updateBatchDTO, int id)
        {
            try
            {
                await _batchService.Update(updateBatchDTO, id);
                return Ok("Update batch successfully");
            }catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("CheckAndUpdateBatchStatus")]
        public async Task<IActionResult> CheckAndUpdateBatchStatus()
        {
            try
            {
                await _batchService.CheckAndUpdateBatchStatus();
                return Ok(new { message = "The batch and flower have expired and need to be reviewed and updated." });
            }catch
            {
                return NotFound("Not update flower and batch");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatch(int id)
        {
            try
            {
                await _batchService.Delete(id);
                return Ok("Delete batch successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getBatchesByFlower/{flowerId}")]
        public async Task<IActionResult> GetBatchesByFlower(int flowerId)
        {
            var batches = await _batchService.GetAvailableBatchesByFlowerId(flowerId);

            if (!batches.Any())
            {
                return NotFound("No batches found for the selected flower.");
            }

            return Ok(batches);
        }

    }
}

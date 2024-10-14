using BusinessObject.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                await _batchService.Create(createBatchDTO);
                return Ok("Add batch successfully");
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
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
    }
}

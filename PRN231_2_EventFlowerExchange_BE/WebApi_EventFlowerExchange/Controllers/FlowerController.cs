﻿using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Service.IService;
using System.Threading.Tasks;

namespace WebApi_EventFlowerExchange.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class FlowerController : ControllerBase
    {
        private readonly IFlowerService _flowerService;

        public FlowerController(IFlowerService flowerService)
        {
            _flowerService = flowerService;
        }

        [EnableQuery]
        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            var flowers = await _flowerService.GetAllFlowers();
            if (flowers == null || !flowers.Any())
            {
                return NotFound("No flowers available.");
            }

            return Ok(flowers);
        }

        [HttpGet("GetBy/{id}")]
        public async Task<IActionResult> GetFlowerById(int id)
        {
            try
            {
                var flower = await _flowerService.GetFlowerById(id);
                return Ok(flower);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
        }
        [HttpPost("Create")]
        public async Task<IActionResult> CreateFlower(CreateFlowerDTO createFlowerDTO)
        {
            try
            {
                var flowerId = await _flowerService.CreateFlower(createFlowerDTO);
                var result = new CreateFlowerResponse
                {
                    Status = "success",
                    Message = "Create Flower successfully",
                    FlowerId = flowerId
                };
                return Ok(result);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateFlower([FromBody] UpdateFlowerDTO updateFlowerDTO, int id)
        {
            try
            {
                await _flowerService.Update(updateFlowerDTO, id);
                return Ok("Flower updated successfully.");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteFlower(int id)
        {
            try
            {
                await _flowerService.Delete(id);
                return Ok("Flower deleted successfully.");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

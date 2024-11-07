using BusinessObject.DTO.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Service.IService;

namespace WebApi_EventFlowerExchange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var companies = await _companyService.GetCompanies();
            if (companies == null || !companies.Any())
            {
                return NotFound("Empty");
            }
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            var company = await _companyService.GetCompanyByID(id);
            if (company == null)
            {
                return NotFound();
            }
            return Ok(company);
        }


        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetCompanyByUserId(int id)
        {
            var company = await _companyService.GetCompanyByIdUser(id);
            if (company == null)
            {
                return NotFound();
            }
            return Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany(CreateCompanyDTO createCompanyDTO)
        {
            try
            {
                await _companyService.AddNew(createCompanyDTO);
                return Ok("Create successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany([FromBody]CreateCompanyDTO createCompanyDTO, int id)
        {
            try
            {
                await _companyService.UpdateCompany(id, createCompanyDTO);
                return Ok("Update successfull");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                await _companyService.Delete(id);
                return Ok("Delete successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

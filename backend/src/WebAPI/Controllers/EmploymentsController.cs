using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/employments")]
    public class EmploymentsController : ControllerBase
    {
        private readonly EmploymentService _service;
        public EmploymentsController(EmploymentService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<List<EmploymentDto>>> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<ActionResult<EmploymentDto>> Create(EmploymentRequest req) => Ok(await _service.CreateAsync(req));

        [HttpPut("{id}")]
        public async Task<ActionResult<EmploymentDto>> Update(Guid id, EmploymentRequest req) => Ok(await _service.UpdateAsync(id, req));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
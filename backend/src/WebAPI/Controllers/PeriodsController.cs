using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/periods")]
    public class PeriodsController : ControllerBase
    {
        private readonly PeriodService _service;
        public PeriodsController(PeriodService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<List<PeriodDto>>> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<ActionResult<PeriodDto>> Create(PeriodRequest req) => Ok(await _service.CreateAsync(req));

        [HttpPut("{id}")]
        public async Task<ActionResult<PeriodDto>> Update(Guid id, PeriodRequest req) => Ok(await _service.UpdateAsync(id, req));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
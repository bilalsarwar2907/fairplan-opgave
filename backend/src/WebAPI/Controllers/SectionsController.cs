using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/sections")]
    public class SectionsController : ControllerBase
    {
        private readonly SectionService _service;
        public SectionsController(SectionService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<List<SectionDto>>> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<ActionResult<SectionDto>> Create(SectionRequest req) => Ok(await _service.CreateAsync(req));

        [HttpPut("{id}")]
        public async Task<ActionResult<SectionDto>> Update(Guid id, SectionRequest req) => Ok(await _service.UpdateAsync(id, req));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/staffgroups")]
    public class StaffGroupsController : ControllerBase
    {
        private readonly StaffGroupService _service;
        public StaffGroupsController(StaffGroupService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<List<StaffGroupDto>>> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<ActionResult<StaffGroupDto>> Create(StaffGroupRequest req) => Ok(await _service.CreateAsync(req));

        [HttpPut("{id}")]
        public async Task<ActionResult<StaffGroupDto>> Update(Guid id, StaffGroupRequest req) => Ok(await _service.UpdateAsync(id, req));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
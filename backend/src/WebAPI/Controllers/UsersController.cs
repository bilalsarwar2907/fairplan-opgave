using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;
        public UsersController(UserService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create(UserRequest req) => Ok(await _service.CreateAsync(req));

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Update(Guid id, UserRequest req) => Ok(await _service.UpdateAsync(id, req));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
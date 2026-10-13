using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/personnel")]
    public class PersonnelController : ControllerBase
    {
        private readonly PersonnelService _service;
        public PersonnelController(PersonnelService service) => _service = service;

        // GET /api/personnel?periodId=...
        [HttpGet]
        public async Task<ActionResult<List<EmploymentDto>>> Get([FromQuery] Guid periodId)
            => Ok(await _service.GetForPeriodAsync(periodId));
    }
}
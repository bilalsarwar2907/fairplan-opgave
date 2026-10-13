using Application.Common;
using Application.DTOs;
using Application.Interfaces;

namespace Application.Services
{
    public class PersonnelService
    {
        private readonly IAppDbContext _db;
        private readonly EmploymentService _employments;

        public PersonnelService(IAppDbContext db, EmploymentService employments)
        {
            _db = db;
            _employments = employments;
        }

        // S1: all employments that overlap the chosen period (inclusive)
        public async Task<List<EmploymentDto>> GetForPeriodAsync(Guid periodId)
        {
            var period = await _db.Periods.FindAsync(periodId)
                ?? throw new BusinessException("Perioden findes ikke.", 404);

            var all = await _employments.GetAllAsync();
            return all
                .Where(e => DateRangeHelper.Overlaps(e.StartDate, e.EndDate, period.StartDate, period.EndDate))
                .ToList();
        }
    }
}
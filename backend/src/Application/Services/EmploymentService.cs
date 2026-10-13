using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class EmploymentService
    {
        private readonly IAppDbContext _db;
        public EmploymentService(IAppDbContext db) => _db = db;

        public async Task<List<EmploymentDto>> GetAllAsync() =>
            await _db.Employments
                .OrderBy(e => e.User.Name).ThenBy(e => e.StartDate)
                .Select(e => new EmploymentDto(
                    e.Id,
                    e.UserId, e.User.Name,
                    e.SectionId, e.Section.Name,
                    e.StaffGroupId, e.StaffGroup.Name,
                    e.HoursPerWeek, e.StartDate, e.EndDate))
                .ToListAsync();

        public async Task<EmploymentDto> CreateAsync(EmploymentRequest req)
        {
            await ValidateAsync(req, excludeId: null);
            var employment = new Employment
            {
                UserId = req.UserId,
                SectionId = req.SectionId,
                StaffGroupId = req.StaffGroupId,
                HoursPerWeek = req.HoursPerWeek,
                StartDate = req.StartDate,
                EndDate = req.EndDate
            };
            _db.Employments.Add(employment);
            await _db.SaveChangesAsync();
            return await GetDtoAsync(employment.Id);
        }

        public async Task<EmploymentDto> UpdateAsync(Guid id, EmploymentRequest req)
        {
            var employment = await _db.Employments.FindAsync(id)
                ?? throw new BusinessException("Ansættelsen findes ikke.", 404);
            await ValidateAsync(req, excludeId: id);
            employment.UserId = req.UserId;
            employment.SectionId = req.SectionId;
            employment.StaffGroupId = req.StaffGroupId;
            employment.HoursPerWeek = req.HoursPerWeek;
            employment.StartDate = req.StartDate;
            employment.EndDate = req.EndDate;
            await _db.SaveChangesAsync();
            return await GetDtoAsync(id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var employment = await _db.Employments.FindAsync(id)
                ?? throw new BusinessException("Ansættelsen findes ikke.", 404);
            _db.Employments.Remove(employment);
            await _db.SaveChangesAsync();
        }

        private async Task ValidateAsync(EmploymentRequest req, Guid? excludeId)
        {
            // E1: hours 0–37
            if (req.HoursPerWeek < 0 || req.HoursPerWeek > 37)
                throw new BusinessException("Timer pr. uge skal være mellem 0 og 37.");

            // E2: dates required + end after start
            if (req.StartDate == default || req.EndDate == default)
                throw new BusinessException("Start- og slutdato er påkrævet.");
            if (req.EndDate <= req.StartDate)
                throw new BusinessException("Slutdato skal være efter startdato.");

            // E5: referenced data must exist
            var user = await _db.Users.FindAsync(req.UserId)
                ?? throw new BusinessException("Brugeren findes ikke.");
            if (!await _db.Sections.AnyAsync(s => s.Id == req.SectionId))
                throw new BusinessException("Afsnittet findes ikke.");
            if (!await _db.StaffGroups.AnyAsync(g => g.Id == req.StaffGroupId))
                throw new BusinessException("Personalegruppen findes ikke.");

            // E3: only personnel can have employments
            if (!user.IsPersonnel)
                throw new BusinessException("Brugeren har ikke personalerollen og kan ikke ansættes.");

            // E4: same user's employments must not overlap (excluding itself on update)
            var others = await _db.Employments
                .Where(e => e.UserId == req.UserId && e.Id != excludeId)
                .ToListAsync();
            if (others.Any(e => DateRangeHelper.Overlaps(req.StartDate, req.EndDate, e.StartDate, e.EndDate)))
                throw new BusinessException("Ansættelsen overlapper en anden ansættelse for samme bruger.", 409);
        }

        private async Task<EmploymentDto> GetDtoAsync(Guid id) =>
            (await GetAllAsync()).First(e => e.Id == id);
    }
}
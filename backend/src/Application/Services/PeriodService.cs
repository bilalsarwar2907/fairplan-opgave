using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class PeriodService
    {
        private readonly IAppDbContext _db;
        public PeriodService(IAppDbContext db) => _db = db;

        public async Task<List<PeriodDto>> GetAllAsync() =>
            await _db.Periods
                .OrderBy(p => p.StartDate)
                .Select(p => new PeriodDto(p.Id, p.Name, p.StartDate, p.EndDate))
                .ToListAsync();

        public async Task<PeriodDto> CreateAsync(PeriodRequest req)
        {
            await ValidateAsync(req, excludeId: null);
            var period = new Period { Name = req.Name.Trim(), StartDate = req.StartDate, EndDate = req.EndDate };
            _db.Periods.Add(period);
            await _db.SaveChangesAsync();
            return ToDto(period);
        }

        public async Task<PeriodDto> UpdateAsync(Guid id, PeriodRequest req)
        {
            var period = await _db.Periods.FindAsync(id)
                ?? throw new BusinessException("Perioden findes ikke.", 404);
            await ValidateAsync(req, excludeId: id);     // skip the period we are editing
            period.Name = req.Name.Trim();
            period.StartDate = req.StartDate;
            period.EndDate = req.EndDate;
            await _db.SaveChangesAsync();
            return ToDto(period);
        }

        public async Task DeleteAsync(Guid id)
        {
            var period = await _db.Periods.FindAsync(id)
                ?? throw new BusinessException("Perioden findes ikke.", 404);
            _db.Periods.Remove(period);
            await _db.SaveChangesAsync();
        }

        private async Task ValidateAsync(PeriodRequest req, Guid? excludeId)
        {
            // P1: required
            if (string.IsNullOrWhiteSpace(req.Name))
                throw new BusinessException("Navn er påkrævet.");
            if (req.StartDate == default || req.EndDate == default)
                throw new BusinessException("Start- og slutdato er påkrævet.");

            // P2: start before end
            if (req.StartDate >= req.EndDate)
                throw new BusinessException("Startdato skal være før slutdato.");

            // P3: no overlap with other periods
            var others = await _db.Periods.Where(p => p.Id != excludeId).ToListAsync();
            if (others.Any(p => DateRangeHelper.Overlaps(req.StartDate, req.EndDate, p.StartDate, p.EndDate)))
                throw new BusinessException("Perioden overlapper en eksisterende periode.", 409);
        }

        private static PeriodDto ToDto(Period p) => new(p.Id, p.Name, p.StartDate, p.EndDate);
    }
}
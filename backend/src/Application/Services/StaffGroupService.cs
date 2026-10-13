using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class StaffGroupService
    {
        private readonly IAppDbContext _db;
        public StaffGroupService(IAppDbContext db) => _db = db;

        public async Task<List<StaffGroupDto>> GetAllAsync() =>
            await _db.StaffGroups
                .OrderBy(g => g.Name)
                .Select(g => new StaffGroupDto(g.Id, g.Name))
                .ToListAsync();

        public async Task<StaffGroupDto> CreateAsync(StaffGroupRequest req)
        {
            Validate(req);
            var group = new StaffGroup { Name = req.Name.Trim() };
            _db.StaffGroups.Add(group);
            await _db.SaveChangesAsync();
            return new StaffGroupDto(group.Id, group.Name);
        }

        public async Task<StaffGroupDto> UpdateAsync(Guid id, StaffGroupRequest req)
        {
            var group = await _db.StaffGroups.FindAsync(id)
                ?? throw new BusinessException("Personalegruppen findes ikke.", 404);
            Validate(req);
            group.Name = req.Name.Trim();
            await _db.SaveChangesAsync();
            return new StaffGroupDto(group.Id, group.Name);
        }

        public async Task DeleteAsync(Guid id)
        {
            var group = await _db.StaffGroups.FindAsync(id)
                ?? throw new BusinessException("Personalegruppen findes ikke.", 404);

            // G2: no delete while employments use this staff group
            if (await _db.Employments.AnyAsync(e => e.StaffGroupId == id))
                throw new BusinessException("Personalegruppen kan ikke slettes, fordi den har ansættelser.", 409);

            _db.StaffGroups.Remove(group);
            await _db.SaveChangesAsync();
        }

        // G1: name required
        private static void Validate(StaffGroupRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                throw new BusinessException("Navn er påkrævet.");
        }
    }
}
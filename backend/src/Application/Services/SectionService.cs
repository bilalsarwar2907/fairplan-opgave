using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Application.Services
{
    public class SectionService
    {
        private readonly IAppDbContext _db;
        private static readonly Regex HexColor = new("^#[0-9A-Fa-f]{6}$");

        public SectionService(IAppDbContext db) => _db = db;

        public async Task<List<SectionDto>> GetAllAsync() =>
            await _db.Sections
                .OrderBy(s => s.Name)
                .Select(s => new SectionDto(s.Id, s.Name, s.Color))
                .ToListAsync();

        public async Task<SectionDto> CreateAsync(SectionRequest req)
        {
            Validate(req);
            var section = new Section { Name = req.Name.Trim(), Color = req.Color.ToUpperInvariant() };
            _db.Sections.Add(section);
            await _db.SaveChangesAsync();
            return ToDto(section);
        }

        public async Task<SectionDto> UpdateAsync(Guid id, SectionRequest req)
        {
            var section = await _db.Sections.FindAsync(id)
                ?? throw new BusinessException("Afsnittet findes ikke.", 404);
            Validate(req);
            section.Name = req.Name.Trim();
            section.Color = req.Color.ToUpperInvariant();
            await _db.SaveChangesAsync();
            return ToDto(section);
        }

        public async Task DeleteAsync(Guid id)
        {
            var section = await _db.Sections.FindAsync(id)
                ?? throw new BusinessException("Afsnittet findes ikke.", 404);

            // A2: no delete while employments use this section
            if (await _db.Employments.AnyAsync(e => e.SectionId == id))
                throw new BusinessException("Afsnittet kan ikke slettes, fordi det har ansættelser.", 409);

            _db.Sections.Remove(section);
            await _db.SaveChangesAsync();
        }

        private static void Validate(SectionRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                throw new BusinessException("Navn er påkrævet.");

            // A1: HEX colour #RRGGBB
            if (string.IsNullOrWhiteSpace(req.Color) || !HexColor.IsMatch(req.Color))
                throw new BusinessException("Farve skal være en HEX-kode, fx #FF0000.");
        }

        private static SectionDto ToDto(Section s) => new(s.Id, s.Name, s.Color);
    }
}
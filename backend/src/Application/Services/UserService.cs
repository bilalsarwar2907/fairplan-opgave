using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace Application.Services
{
    public class UserService
    {
        private readonly IAppDbContext _db;
        public UserService(IAppDbContext db) => _db = db;

        public async Task<List<UserDto>> GetAllAsync() =>
            await _db.Users
                .OrderBy(u => u.Name)
                .Select(u => new UserDto(u.Id, u.Name, u.Email, u.IsAdmin, u.IsPersonnel))
                .ToListAsync();

        public async Task<UserDto> CreateAsync(UserRequest req)
        {
            var email = await ValidateAsync(req, excludeId: null);
            var user = new User { Name = req.Name.Trim(), Email = email, IsAdmin = req.IsAdmin, IsPersonnel = req.IsPersonnel };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return ToDto(user);
        }

        public async Task<UserDto> UpdateAsync(Guid id, UserRequest req)
        {
            var user = await _db.Users.FindAsync(id)
                ?? throw new BusinessException("Brugeren findes ikke.", 404);
            var email = await ValidateAsync(req, excludeId: id);

            // D16: can't remove the personnel role while the user has employments (protects E3)
            if (user.IsPersonnel && !req.IsPersonnel && await _db.Employments.AnyAsync(e => e.UserId == id))
                throw new BusinessException("Personalerollen kan ikke fjernes, fordi brugeren har ansættelser.", 409);

            user.Name = req.Name.Trim();
            user.Email = email;
            user.IsAdmin = req.IsAdmin;
            user.IsPersonnel = req.IsPersonnel;
            await _db.SaveChangesAsync();
            return ToDto(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _db.Users.FindAsync(id)
                ?? throw new BusinessException("Brugeren findes ikke.", 404);

            // U3: no delete while the user has employments
            if (await _db.Employments.AnyAsync(e => e.UserId == id))
                throw new BusinessException("Brugeren kan ikke slettes, fordi brugeren har ansættelser.", 409);

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        // Returns the normalised email (trimmed + lower-case) so Create/Update store the same form
        private async Task<string> ValidateAsync(UserRequest req, Guid? excludeId)
        {
            // U1: name + valid email
            if (string.IsNullOrWhiteSpace(req.Name))
                throw new BusinessException("Navn er påkrævet.");
            if (string.IsNullOrWhiteSpace(req.Email) || !MailAddress.TryCreate(req.Email.Trim(), out _))
                throw new BusinessException("Email er ugyldig.");

            // U4: at least one role
            if (!req.IsAdmin && !req.IsPersonnel)
                throw new BusinessException("Brugeren skal have mindst én rolle (Admin eller Personale).");

            // U2: email unique, case-insensitive (D15)
            var email = req.Email.Trim().ToLowerInvariant();
            if (await _db.Users.AnyAsync(u => u.Email == email && u.Id != excludeId))
                throw new BusinessException("Email er allerede i brug.", 409);

            return email;
        }

        private static UserDto ToDto(User u) => new(u.Id, u.Name, u.Email, u.IsAdmin, u.IsPersonnel);
    }
}
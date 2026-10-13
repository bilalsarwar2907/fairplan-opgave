using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            // Already seeded? Then do nothing (no duplicates on restart)
            if (db.Periods.Any())
            {
                return;
            }

            // Periods
            var dec = new Period { Name = "December 2026", StartDate = new DateOnly(2026, 12, 1), EndDate = new DateOnly(2026, 12, 31) };
            var jan = new Period { Name = "Januar 2027", StartDate = new DateOnly(2027, 1, 1), EndDate = new DateOnly(2027, 1, 31) };

            // Sections (Afsnit)
            var skadestue = new Section { Name = "Skadestue", Color = "#FF0000" };
            var sengeafsnitA = new Section { Name = "Sengeafsnit A", Color = "#00FF00" };

            // Staff groups (Personalegrupper)
            var overlaege = new StaffGroup { Name = "Overlæge" };
            var afdelingslaege = new StaffGroup { Name = "Afdelingslæge" };
            var introlaege = new StaffGroup { Name = "Introlæge" };

            // Users: 1 admin, 2 personnel, 1 with both roles
            var admin = new User { Name = "Lene Admin", Email = "lene@fairplan.dk", IsAdmin = true };
            var anna = new User { Name = "Anna Jensen", Email = "anna@fairplan.dk", IsPersonnel = true };
            var bo = new User { Name = "Bo Nielsen", Email = "bo@fairplan.dk", IsPersonnel = true };
            var carl = new User { Name = "Carl Hansen", Email = "carl@fairplan.dk", IsAdmin = true, IsPersonnel = true };

            // Employments (only personnel users, max 37 hours)
            var e1 = new Employment
            {
                User = anna,
                Section = skadestue,
                StaffGroup = overlaege,
                HoursPerWeek = 37,
                StartDate = new DateOnly(2026, 12, 15),
                EndDate = new DateOnly(2027, 1, 15)
            }; // spans Dec–Jan
            var e2 = new Employment
            {
                User = bo,
                Section = sengeafsnitA,
                StaffGroup = afdelingslaege,
                HoursPerWeek = 30,
                StartDate = new DateOnly(2026, 12, 1),
                EndDate = new DateOnly(2026, 12, 31)
            };
            var e3 = new Employment
            {
                User = carl,
                Section = skadestue,
                StaffGroup = introlaege,
                HoursPerWeek = 20,
                StartDate = new DateOnly(2027, 1, 1),
                EndDate = new DateOnly(2027, 1, 31)
            };

            db.AddRange(dec, jan, skadestue, sengeafsnitA, overlaege, afdelingslaege, introlaege,
                        admin, anna, bo, carl, e1, e2, e3);
            db.SaveChanges();
        }
    }
}
using Application.Common;
using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;

namespace Tests
{
    public class ServiceRuleTests
    {
        private static DateOnly D(int y, int m, int d) => new(y, m, d);

        // Small helper: a DB with 1 personnel user, 1 admin-only user, 1 section, 1 group, 1 employment (1–31 Dec)
        private static async Task<(AppDbContext db, User anna, User lene, Section sec, StaffGroup grp)> SeedAsync()
        {
            var db = TestDb.Create();
            var anna = new User { Name = "Anna", Email = "anna@x.dk", IsPersonnel = true };
            var lene = new User { Name = "Lene", Email = "lene@x.dk", IsAdmin = true };
            var sec = new Section { Name = "Skadestue", Color = "#FF0000" };
            var grp = new StaffGroup { Name = "Overlæge" };
            db.AddRange(anna, lene, sec, grp,
                new Employment
                {
                    User = anna,
                    Section = sec,
                    StaffGroup = grp,
                    HoursPerWeek = 37,
                    StartDate = D(2026, 12, 1),
                    EndDate = D(2026, 12, 31)
                });
            await db.SaveChangesAsync();
            return (db, anna, lene, sec, grp);
        }

        private static EmploymentRequest Emp(User u, Section s, StaffGroup g, int hours, DateOnly from, DateOnly to)
            => new(u.Id, s.Id, g.Id, hours, from, to);

        // ---------- Periods ----------

        [Fact]
        public async Task P3_overlapping_period_is_rejected_409()
        {
            var svc = new PeriodService(TestDb.Create());
            await svc.CreateAsync(new PeriodRequest("Dec", D(2026, 12, 1), D(2026, 12, 31)));

            var ex = await Assert.ThrowsAsync<BusinessException>(() =>
                svc.CreateAsync(new PeriodRequest("Overlap", D(2026, 12, 20), D(2027, 1, 10))));
            Assert.Equal(409, ex.StatusCode);
        }

        [Fact]
        public async Task P3_adjacent_period_is_allowed()
        {
            var svc = new PeriodService(TestDb.Create());
            await svc.CreateAsync(new PeriodRequest("Dec", D(2026, 12, 1), D(2026, 12, 31)));

            var jan = await svc.CreateAsync(new PeriodRequest("Jan", D(2027, 1, 1), D(2027, 1, 31)));
            Assert.Equal("Jan", jan.Name);
        }

        [Fact]
        public async Task P3_update_does_not_conflict_with_itself()
        {
            var svc = new PeriodService(TestDb.Create());
            var dec = await svc.CreateAsync(new PeriodRequest("Dec", D(2026, 12, 1), D(2026, 12, 31)));

            var updated = await svc.UpdateAsync(dec.Id, new PeriodRequest("December", D(2026, 12, 1), D(2026, 12, 31)));
            Assert.Equal("December", updated.Name);
        }

        [Fact]
        public async Task P2_start_after_end_is_rejected_400()
        {
            var svc = new PeriodService(TestDb.Create());
            var ex = await Assert.ThrowsAsync<BusinessException>(() =>
                svc.CreateAsync(new PeriodRequest("Fejl", D(2027, 5, 10), D(2027, 5, 1))));
            Assert.Equal(400, ex.StatusCode);
        }

        // ---------- Employments ----------

        [Theory]
        [InlineData(-1, false)]
        [InlineData(0, true)]
        [InlineData(37, true)]
        [InlineData(38, false)]
        public async Task E1_hours_must_be_0_to_37(int hours, bool allowed)
        {
            var (db, anna, _, sec, grp) = await SeedAsync();
            var svc = new EmploymentService(db);
            var req = Emp(anna, sec, grp, hours, D(2027, 3, 1), D(2027, 3, 31));

            if (allowed)
                Assert.Equal(hours, (await svc.CreateAsync(req)).HoursPerWeek);
            else
                Assert.Equal(400, (await Assert.ThrowsAsync<BusinessException>(() => svc.CreateAsync(req))).StatusCode);
        }

        [Fact]
        public async Task E2_end_before_start_is_rejected_400()
        {
            var (db, anna, _, sec, grp) = await SeedAsync();
            var ex = await Assert.ThrowsAsync<BusinessException>(() =>
                new EmploymentService(db).CreateAsync(Emp(anna, sec, grp, 20, D(2027, 3, 31), D(2027, 3, 1))));
            Assert.Equal(400, ex.StatusCode);
        }

        [Fact]
        public async Task E3_user_without_personnel_role_is_rejected_400()
        {
            var (db, _, lene, sec, grp) = await SeedAsync();
            var ex = await Assert.ThrowsAsync<BusinessException>(() =>
                new EmploymentService(db).CreateAsync(Emp(lene, sec, grp, 20, D(2027, 3, 1), D(2027, 3, 31))));
            Assert.Equal(400, ex.StatusCode);
        }

        [Fact]
        public async Task E4_overlapping_employment_same_user_is_rejected_409()
        {
            var (db, anna, _, sec, grp) = await SeedAsync();   // Anna already has 1–31 Dec
            var ex = await Assert.ThrowsAsync<BusinessException>(() =>
                new EmploymentService(db).CreateAsync(Emp(anna, sec, grp, 20, D(2026, 12, 31), D(2027, 1, 15))));
            Assert.Equal(409, ex.StatusCode);
        }

        [Fact]
        public async Task E4_adjacent_employment_same_user_is_allowed()
        {
            var (db, anna, _, sec, grp) = await SeedAsync();
            var created = await new EmploymentService(db).CreateAsync(Emp(anna, sec, grp, 20, D(2027, 1, 1), D(2027, 1, 31)));
            Assert.Equal("Anna", created.UserName);
        }

        // ---------- Sections / StaffGroups / Users ----------

        [Fact]
        public async Task A1_colour_must_be_hex_400()
        {
            var ex = await Assert.ThrowsAsync<BusinessException>(() =>
                new SectionService(TestDb.Create()).CreateAsync(new SectionRequest("Test", "red")));
            Assert.Equal(400, ex.StatusCode);
        }

        [Fact]
        public async Task A2_G2_U3_delete_blocked_when_employments_exist_409()
        {
            var (db, anna, _, sec, grp) = await SeedAsync();

            Assert.Equal(409, (await Assert.ThrowsAsync<BusinessException>(() => new SectionService(db).DeleteAsync(sec.Id))).StatusCode);
            Assert.Equal(409, (await Assert.ThrowsAsync<BusinessException>(() => new StaffGroupService(db).DeleteAsync(grp.Id))).StatusCode);
            Assert.Equal(409, (await Assert.ThrowsAsync<BusinessException>(() => new UserService(db).DeleteAsync(anna.Id))).StatusCode);
        }

        [Fact]
        public async Task U2_email_is_unique_case_insensitive_409()
        {
            var (db, _, _, _, _) = await SeedAsync();          // anna@x.dk exists
            var ex = await Assert.ThrowsAsync<BusinessException>(() =>
                new UserService(db).CreateAsync(new UserRequest("Ny", "ANNA@X.DK", true, false)));
            Assert.Equal(409, ex.StatusCode);
        }

        [Fact]
        public async Task U4_user_needs_at_least_one_role_400()
        {
            var ex = await Assert.ThrowsAsync<BusinessException>(() =>
                new UserService(TestDb.Create()).CreateAsync(new UserRequest("Ingen", "ingen@x.dk", false, false)));
            Assert.Equal(400, ex.StatusCode);
        }

        // ---------- Personnel view ----------

        [Fact]
        public async Task S1_personnel_shows_only_employments_overlapping_the_period()
        {
            var (db, anna, _, sec, grp) = await SeedAsync();   // Anna: 1–31 Dec
            var periods = new PeriodService(db);
            var dec = await periods.CreateAsync(new PeriodRequest("Dec", D(2026, 12, 1), D(2026, 12, 31)));
            var feb = await periods.CreateAsync(new PeriodRequest("Feb", D(2027, 2, 1), D(2027, 2, 28)));

            var personnel = new PersonnelService(db, new EmploymentService(db));

            Assert.Single(await personnel.GetForPeriodAsync(dec.Id));   // Anna works in December
            Assert.Empty(await personnel.GetForPeriodAsync(feb.Id));    // nobody in February
        }
    }
}
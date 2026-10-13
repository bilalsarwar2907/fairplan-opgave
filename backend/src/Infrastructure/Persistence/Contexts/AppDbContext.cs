using System;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Contexts
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Period> Periods => Set<Period>();
        public DbSet<Section> Sections => Set<Section>();
        public DbSet<User> Users => Set<User>();
        public DbSet<StaffGroup> StaffGroups => Set<StaffGroup>();
        public DbSet<Employment> Employments => Set<Employment>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Employment -> User: can't delete a user who has employments (U3)
            modelBuilder.Entity<Employment>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employment -> Section: can't delete a section that has employments (A2)
            modelBuilder.Entity<Employment>()
                .HasOne(e => e.Section)
                .WithMany()
                .HasForeignKey(e => e.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employment -> StaffGroup: can't delete a staff group that has employments (G2)
            modelBuilder.Entity<Employment>()
                .HasOne(e => e.StaffGroup)
                .WithMany()
                .HasForeignKey(e => e.StaffGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Email must be unique (U2)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}

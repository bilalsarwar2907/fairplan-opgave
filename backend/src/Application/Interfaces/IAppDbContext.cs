using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAppDbContext 
    {
         DbSet<Period> Periods { get;  }
         DbSet<Section> Sections { get; }

        DbSet<User> Users { get; }

         DbSet<StaffGroup> StaffGroups { get; }
        DbSet<Employment> Employments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}

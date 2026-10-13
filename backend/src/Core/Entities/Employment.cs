using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
public class Employment : BaseEntity
    {
        //  Foreign keys
        public Guid UserId { get; set; }
        public Guid SectionId { get; set; }
        public Guid StaffGroupId { get; set; }

        // Data properties

        public int HoursPerWeek { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        // Navigation properties

        public User User { get; set; } = null!;
        public Section Section { get; set; } = null!;
        public StaffGroup StaffGroup { get; set; } = null!;


    }
    }

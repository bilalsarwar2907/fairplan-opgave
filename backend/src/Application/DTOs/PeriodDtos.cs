using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    
        public record PeriodRequest(string Name, DateOnly StartDate, DateOnly EndDate);
        public record PeriodDto(Guid Id, string Name, DateOnly StartDate, DateOnly EndDate);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record SectionRequest(string Name, string Color);
    public record SectionDto(Guid Id, string Name, string Color);
}

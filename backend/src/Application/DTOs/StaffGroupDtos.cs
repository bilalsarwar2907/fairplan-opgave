using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record StaffGroupRequest(string Name);
    public record StaffGroupDto(Guid Id, string Name);
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record UserRequest(string Name, string Email, bool IsAdmin, bool IsPersonnel);
    public record UserDto(Guid Id, string Name, string Email, bool IsAdmin, bool IsPersonnel);
}
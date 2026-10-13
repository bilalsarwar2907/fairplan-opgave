using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record EmploymentRequest(Guid UserId, Guid SectionId, Guid StaffGroupId,
                                    int HoursPerWeek, DateOnly StartDate, DateOnly EndDate);

    public record EmploymentDto(Guid Id,
                                Guid UserId, string UserName,
                                Guid SectionId, string SectionName,
                                Guid StaffGroupId, string StaffGroupName,
                                int HoursPerWeek, DateOnly StartDate, DateOnly EndDate);
}
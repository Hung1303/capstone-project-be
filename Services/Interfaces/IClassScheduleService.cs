using BusinessObjects.DTO.ClassSchedule;
using BusinessObjects.DTO.LessonPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IClassScheduleService
    {
        Task<ClassScheduleResponse> CreateClassSchedule(CreateClassScheduleRequest request);
        Task<ClassScheduleResponse> UpdateClassSchedule(Guid id, UpdateClassScheduleRequest request);
        Task<IEnumerable<ClassScheduleResponse>> GetAllClassSchedule(DayOfWeek? dayOfWeek,TimeOnly? startTime,TimeOnly? endTime,DateOnly? startDate,DateOnly? endDate, int pageNumber, int pageSize);
        Task<ClassScheduleResponse> GetClassScheduleById(Guid id);
        Task<bool> DeleteCLassSchedule(Guid id);
    }
}

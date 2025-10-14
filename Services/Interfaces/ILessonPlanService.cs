using BusinessObjects.DTO.LessonPlan;
using BusinessObjects.DTO.Syllabus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ILessonPlanService
    {
        Task<LessonPlanResponse> CreateLessonPlan(CreateLessonPlanRequest request);
        Task<LessonPlanResponse> UpdateLessonPlan(Guid id, UpdateLessonPlanRequest request);
        Task<IEnumerable<LessonPlanResponse>> GetAllLessonPlan(string? searchTerm, int pageNumber, int pageSize);
        Task<LessonPlanResponse> GetLessonPlanById(Guid id);
        Task<bool> DeleteLessonPlan(Guid id);
    }
}

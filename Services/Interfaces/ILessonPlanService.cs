using BusinessObjects.DTO.LessonPlan;

namespace Services.Interfaces
{
    public interface ILessonPlanService
    {
        Task<LessonPlanResponse> CreateLessonPlan(CreateLessonPlanRequest request);
        Task<LessonPlanResponse> UpdateLessonPlan(Guid id, UpdateLessonPlanRequest request);
        Task<IEnumerable<LessonPlanResponse>> GetAllLessonPlan(string? searchTerm, int pageNumber, int pageSize, Guid? syllabusId);
        Task<LessonPlanResponse> GetLessonPlanById(Guid id);
        Task<bool> DeleteLessonPlan(Guid id);
    }
}

using BusinessObjects.DTO.Course;

namespace Services.Interfaces
{
    public interface ICourseService
    {
        Task<CourseResponse> CreateCourse(CreateCourseRequest request);
        Task<CourseResponse> UpdateCourse(Guid id, UpdateCourseRequest request);
        Task<IEnumerable<CourseResponse>> GetAllCourse(string? searchTerm, int pageNumber, int pageSize);
        Task<CourseResponse> GetCourseById(Guid id);
        Task<bool> DeleteCourse(Guid id);
    }
}

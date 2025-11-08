using Services.DTO.CourseResult;

namespace Services.Interfaces
{
    public interface ICourseResultService
    {
        Task<CourseResultResponse> CreateCourseResult(CreateCourseResultRequest request);
        Task<CourseResultResponse> UpdateCourseResult(Guid id, UpdateCourseResultRequest request);
        Task<IEnumerable<CourseResultResponse>> GetAllCourseResult(string? searchTerm, int pageNumber, int pageSize, Guid? TeacherProfileId, Guid? CourseId, Guid? StudentProfileId);
        Task<CourseResultResponse> GetCourseResultById(Guid id);
        Task<bool> DeleteCourseResult(Guid id);
        Task<IEnumerable<CourseResultResponse>> GetAllCourseResultsByStudentId(Guid StudentProfileId, string? Subject, string? searchTerm, int pageNumber, int pageSize);
        Task<IEnumerable<CourseResultResponse>> GetAllCourseResultsByTeacherId(Guid TeacherProfileId, string? Subject, string? searchTerm, int pageNumber, int pageSize);
    }
}

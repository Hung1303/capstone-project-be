using Services.DTO.Course;

namespace Services.Interfaces
{
    public interface ICourseService
    {
        Task<CourseResponse> CreateCourse(CreateCourseRequest request);
        Task<CourseResponse> CenterCreateCourse(CenterCreateCourseRequest request);
        Task<CourseResponse> AssignTeacher(Guid id, AssignTeacherRequest request);
        Task<CourseSubjectResponse> CreateSubjectForCourse(CreateSubjectForCourseRequest request);
        Task<CourseResponse> UpdateCourse(Guid id, UpdateCourseRequest request);
        Task<CourseSubjectResponse> UpdateCourseSubject(Guid id, UpdateCourseSubject request);
        Task<IEnumerable<CourseResponse>> GetAllCourse(string? searchTerm, int pageNumber, int pageSize, Guid? TeacherProfileId, Guid? CenterProfileId);
        Task<IEnumerable<CourseResponse>> GetAllApprovedCoursesByTeacher(Guid TeacherProfileId, string? searchTerm, int pageNumber, int pageSize);
        Task<IEnumerable<CourseSubjectResponse>> GetAllCourseSubject(string? searchTerm, int pageNumber, int pageSize, Guid? CourseId, Guid? TeacherProfileId, string? status);
        Task<IEnumerable<CourseSubjectResponse>> GetAllStudentSchedules(string? searchTerm, int pageNumber, int pageSize, Guid StudentId, Guid CourseId);
        Task<IEnumerable<CourseSubjectResponse>> GetAllStudentSchedulesByParentsId(string? searchTerm, int pageNumber, int pageSize, Guid ParentId);
        Task<CourseResponse> GetCourseById(Guid id);
        Task<CourseSubjectResponse> GetCourseSubjectById(Guid id);
        Task<bool> DeleteCourse(Guid id);
        Task<bool> DeleteCourseSubject(Guid id);
    }
}

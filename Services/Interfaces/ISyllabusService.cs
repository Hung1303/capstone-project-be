using Services.DTO.Syllabus;

namespace Services.Interfaces
{
    public interface ISyllabusService
    {
        Task<SyllabusResponse> CreateSyllabus(CreateSyllabusRequest request);
        Task<SyllabusResponse> UpdateSyllabus(Guid id, UpdateSyllabusRequest request);
        Task<IEnumerable<SyllabusResponse>> GetAllSyllabus(string? searchTerm, int pageNumber, int pageSize, Guid? subjectId, Guid? TeacherProfileId);
        Task<SyllabusResponse> GetSyllabusById(Guid id);
        Task<bool> DeleteSyllabus(Guid id);
        Task<SyllabusResponse2> GetSyllabusBySubjectOfCenter(Guid subjectId, Guid centerProfileId);
    }
}

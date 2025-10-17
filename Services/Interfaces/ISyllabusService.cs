using BusinessObjects.DTO.Syllabus;

namespace Services.Interfaces
{
    public interface ISyllabusService
    {
        Task<SyllabusResponse> CreateSyllabus(CreateSyllabusRequest request);
        Task<SyllabusResponse> UpdateSyllabus(Guid id, UpdateSyllabusRequest request);
        Task<IEnumerable<SyllabusResponse>> GetAllSyllabus(string? searchTerm, int pageNumber, int pageSize, Guid? courseId);
        Task<SyllabusResponse> GetSyllabusById(Guid id);
        Task<bool> DeleteSyllabus(Guid id);
    }
}

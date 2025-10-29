using BusinessObjects.DTO.Subject;

namespace Services.Interfaces
{
    public interface ISubjectService
    {
        Task<SubjectResponse> CreateSubject(CreateSubjectRequest request);
        Task<SubjectResponse> UpdateSubject(Guid id, UpdateSubjectRequest request);
        Task<IEnumerable<SubjectResponse>> GetAllSubject(string? searchTerm, int pageNumber, int pageSize);
        Task<SubjectResponse> GetSubjectById(Guid id);
        Task<bool> DeleteSubject(Guid id);
    }
}

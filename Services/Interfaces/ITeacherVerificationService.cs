using Services.DTO.TeacherVerification;

namespace Services.Interfaces
{
    public interface ITeacherVerificationService
    {
        Task<TeacherVerificationResponse> RequestVerification(TeacherVerificationRequestDto request);
        Task<TeacherVerificationResponse> GetById(Guid id);
        Task<IEnumerable<TeacherVerificationResponse>> GetAll(string? search, int pageNumber, int pageSize, Guid? teacherProfileId);
        Task<TeacherVerificationResponse> UpdateDocuments(Guid id, TeacherVerificationDocumentsDto request);
        Task<TeacherVerificationResponse> SetStatus(Guid id, SetTeacherVerificationStatusDto request);
    }
}



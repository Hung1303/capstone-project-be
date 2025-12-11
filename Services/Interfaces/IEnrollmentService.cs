using Core.Base;
using Services.DTO.EnrollmentDTO;

namespace Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentResponse> CreateEnrollment(CreateEnrollmentRequest request);
        Task<IEnumerable<EnrollmentResponse>> GetAllEnrollments(string? searchTerm, int pageNumber, int pageSize);
        Task<IEnumerable<EnrollmentResponse>> GetAllEnrollmentsByCenter(Guid centerProfileId, string? searchTerm, EnrollmentStatus? status, int pageNumber, int pageSize);
        Task<IEnumerable<StudentEnrollmentResponse>> GetAllEnrollmentsByStudent(Guid studentProfileId, string? searchTerm, EnrollmentStatus? status, int pageNumber, int pageSize);
        Task<EnrollmentResponse> GetEnrollmentById(Guid id);
        Task<EnrollmentResponse> UpdateEnrollment(Guid id, UpdateEnrollmentRequest request);
        Task<bool> DeleteEnrollment(Guid id);
        Task<EnrollmentResponse> ApproveEnrollment(Guid enrollmentId, Guid approverProfileId);
        Task<EnrollmentResponse> RejectEnrollment(Guid enrollmentId, Guid approverProfileId, string reason);

    }

}

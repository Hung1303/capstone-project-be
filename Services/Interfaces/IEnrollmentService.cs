using BusinessObjects.DTO.EnrollmentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentResponse> CreateEnrollment(CreateEnrollmentRequest request);
        Task<IEnumerable<EnrollmentResponse>> GetAllEnrollments(string? searchTerm, int pageNumber, int pageSize);
        Task<EnrollmentResponse> GetEnrollmentById(Guid id);
        Task<EnrollmentResponse> UpdateEnrollment(Guid id, UpdateEnrollmentRequest request);
        Task<bool> DeleteEnrollment(Guid id);
    }

}

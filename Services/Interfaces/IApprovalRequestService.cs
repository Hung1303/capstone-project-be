using BusinessObjects.DTO.ApprovalRequest;
using Core.Base;

namespace Services.Interfaces
{

    public interface IApprovalRequestService
    {
        Task<bool> CreateApprovalRequestAsync(Guid courseId, Guid requestedByUserId, string? notes = null);
        Task<bool> ReviewApprovalRequestAsync(Guid approvalRequestId, Guid reviewerUserId, ApprovalDecision decision, string? notes = null);
        Task<(IEnumerable<ApprovalRequestResponse> Records, int TotalCount)> GetApprovalRequestsAsync(int pageNumber, int pageSize, string? searchKeyword = null);
        Task<ApprovalRequestResponse> GetApprovalRequestByIdAsync(Guid id);
        Task<bool> DeleteApprovalRequestAsync(Guid approvalRequestId);
    }
}

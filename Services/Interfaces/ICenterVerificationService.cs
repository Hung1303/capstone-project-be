using Core.Base;
using Services.DTO.CenterVerification;

namespace Services.Interfaces
{
    public interface ICenterVerificationService
    {
        Task<CreateVerificationRequestDto> CreateVerificationRequestAsync(CreateVerificationRequestDto request);
        Task<UpdateVerificationRequestDto> UpdateVerificationRequestAsync(Guid verificationId, UpdateVerificationRequestDto request);
        Task<AdminDecisionDto> MakeAdminDecisionAsync(Guid verificationId, AdminDecisionDto decision);
        Task<VerificationRequestResponseDto?> GetVerificationRequestByIdAsync(Guid verificationId);
        Task<List<VerificationRequestResponseDto>> GetVerificationRequestsByInspectorAsync(Guid inspectorId, int pageNumber = 1, int pageSize = 10);
        Task<List<VerificationRequestResponseDto>> GetPendingVerificationRequestsAsync(int pageNumber = 1, int pageSize = 10);
        Task<List<CenterVerificationListDto>> GetCentersPendingVerificationAsync(int pageNumber = 1, int pageSize = 10);
        Task<List<CenterVerificationListDto>> GetCentersByStatusAsync(CenterStatus status, int pageNumber = 1, int pageSize = 10);
        Task<bool> CompleteVerificationAsync(Guid verificationId);
        Task<bool> SuspendCenterAsync(Guid centerId, string reason, Guid adminId);
        Task<bool> RestoreCenterAsync(Guid centerId, string reason, Guid adminId);
    }
}

using BusinessObjects.DTO.CenterVerification;
using Core.Base;

namespace Services.Interfaces
{
    public interface ICenterVerificationService
    {
        Task<CreateVerificationRequestDto> CreateVerificationRequestAsync(CreateVerificationRequestDto request);
        Task<UpdateVerificationRequestDto> UpdateVerificationRequestAsync(Guid verificationId, UpdateVerificationRequestDto request);
        Task<AdminDecisionDto> MakeAdminDecisionAsync(Guid verificationId, AdminDecisionDto decision);
        Task<VerificationRequestResponseDto?> GetVerificationRequestByIdAsync(Guid verificationId);
        Task<List<VerificationRequestResponseDto>> GetVerificationRequestsByInspectorAsync(Guid inspectorId);
        Task<List<VerificationRequestResponseDto>> GetPendingVerificationRequestsAsync();
        Task<List<CenterVerificationListDto>> GetCentersPendingVerificationAsync();
        Task<List<CenterVerificationListDto>> GetCentersByStatusAsync(CenterStatus status);
        Task<bool> CompleteVerificationAsync(Guid verificationId);
        Task<bool> SuspendCenterAsync(Guid centerId, string reason, Guid adminId);
        Task<bool> RestoreCenterAsync(Guid centerId, string reason, Guid adminId);
    }
}

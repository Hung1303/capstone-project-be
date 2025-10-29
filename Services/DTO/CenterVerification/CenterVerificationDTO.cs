using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace Services.DTO.CenterVerification
{
    public class CreateVerificationRequestDto
    {
        [Required]
        public Guid CenterProfileId { get; set; }
        [Required]
        public Guid InspectorId { get; set; }
        public DateTime? ScheduledDate { get; set; }
    }

    public class UpdateVerificationRequestDto
    {
        public DateTime? CompletedDate { get; set; }
        public string? InspectorNotes { get; set; }
        public List<string>? VerificationPhotos { get; set; }
        public List<string>? DocumentChecklist { get; set; }
        public bool IsLocationVerified { get; set; }
        public bool IsDocumentsVerified { get; set; }
        public bool IsLicenseValid { get; set; }
    }

    public class AdminDecisionDto
    {
        [Required]
        public Guid AdminId { get; set; }
        [Required]
        public ApprovalDecision Decision { get; set; }
        public string? AdminNotes { get; set; }
    }

    public class VerificationRequestResponseDto
    {
        public Guid Id { get; set; }
        public Guid CenterProfileId { get; set; }
        public string CenterName { get; set; } = string.Empty;
        public Guid InspectorId { get; set; }
        public string InspectorName { get; set; } = string.Empty;
        public VerificationStatus Status { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? InspectorNotes { get; set; }
        public List<string>? VerificationPhotos { get; set; }
        public List<string>? DocumentChecklist { get; set; }
        public bool IsLocationVerified { get; set; }
        public bool IsDocumentsVerified { get; set; }
        public bool IsLicenseValid { get; set; }
        public Guid? AdminId { get; set; }
        public string? AdminName { get; set; }
        public DateTime? AdminDecisionDate { get; set; }
        public ApprovalDecision AdminDecision { get; set; }
        public string? AdminNotes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CenterVerificationListDto
    {
        public Guid Id { get; set; }
        public string CenterName { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public CenterStatus Status { get; set; }
        public DateTime? VerificationRequestedAt { get; set; }
        public DateTime? VerificationCompletedAt { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SuspendCenterRequest
    {
        [Required]
        public Guid AdminId { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
    }

    public class RestoreCenterRequest
    {
        [Required]
        public Guid AdminId { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}

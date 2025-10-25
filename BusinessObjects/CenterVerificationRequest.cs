using Core.Base;

namespace BusinessObjects
{
    public class CenterVerificationRequest : BaseEntity
    {
        public Guid CenterProfileId { get; set; }
        public Guid InspectorId { get; set; } // Staff member who performs verification
        public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
        
        // Verification details
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? InspectorNotes { get; set; }
        public string? VerificationPhotos { get; set; } // JSON array of photo paths
        public string? DocumentChecklist { get; set; } // JSON array of checked documents
        public bool IsLocationVerified { get; set; } = false;
        public bool IsDocumentsVerified { get; set; } = false;
        public bool IsLicenseValid { get; set; } = false;
        
        // Admin decision
        public Guid? AdminId { get; set; } // Admin who makes final decision
        public DateTime? AdminDecisionDate { get; set; }
        public ApprovalDecision AdminDecision { get; set; } = ApprovalDecision.Pending;
        public string? AdminNotes { get; set; }

        // Navigation properties
        public virtual CenterProfile CenterProfile { get; set; } = null!;
        public virtual User Inspector { get; set; } = null!;
        public virtual User? Admin { get; set; }
    }
}

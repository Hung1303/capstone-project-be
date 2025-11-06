using Core.Base;

namespace BusinessObjects
{
    public class TeacherVerificationRequest : BaseEntity
    {
        public Guid TeacherProfileId { get; set; }
        public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
        public DateTime? RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }

        // Document storage paths required by Circular 29
        public string? QualificationCertificatePath { get; set; } //Teacher's qualification certificate
        public string? EmploymentContractPath { get; set; } //Teacher's employment contract with the center
        public string? ApprovalFromCenterPath { get; set; } //Teacher's permission to teach at the center
        public string? OtherDocumentsPath { get; set; } //Other documents required by Circular 29

        public Guid? InspectorId { get; set; }
        public Guid? AdminId { get; set; }

        public virtual TeacherProfile TeacherProfile { get; set; } = null!;
        public virtual User? Inspector { get; set; }
        public virtual User? Admin { get; set; }
    }
}



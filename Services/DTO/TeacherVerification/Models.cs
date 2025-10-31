namespace Services.DTO.TeacherVerification
{
    public class TeacherVerificationRequestDto
    {
        public Guid TeacherProfileId { get; set; }
        public string? Notes { get; set; }
    }

    public class TeacherVerificationDocumentsDto
    {
        public string? QualificationCertificatePath { get; set; }
        public string? EmploymentContractPath { get; set; }
        public string? ApprovalFromCenterPath { get; set; }
        public string? OtherDocumentsPath { get; set; }
    }

    public class SetTeacherVerificationStatusDto
    {
        public Core.Base.VerificationStatus Status { get; set; }
        public Guid? InspectorId { get; set; }
        public Guid? AdminId { get; set; }
        public string? Notes { get; set; }
    }

    public class TeacherVerificationResponse
    {
        public Guid Id { get; set; }
        public Guid TeacherProfileId { get; set; }
        public Core.Base.VerificationStatus Status { get; set; }
        public DateTime? RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }
        public string? QualificationCertificatePath { get; set; }
        public string? EmploymentContractPath { get; set; }
        public string? ApprovalFromCenterPath { get; set; }
        public string? OtherDocumentsPath { get; set; }
    }
}



using Core.Base;

namespace BusinessObjects.DTO.EnrollmentDTO
{
    public class CreateEnrollmentRequest
    {
        public Guid CourseId { get; set; }
        public Guid StudentProfileId { get; set; }
    }

    public class UpdateEnrollmentRequest
    {
        public Guid? CourseId { get; set; }
        public Guid? StudentProfileId { get; set; }
        public EnrollmentStatus? Status { get; set; }
        public string? CancelReason { get; set; }
    }

    public class EnrollmentResponse
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Guid StudentProfileId { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTimeOffset? ConfirmedAt { get; set; }
        public DateTimeOffset? CancelledAt { get; set; }
        public string? CancelReason { get; set; }
    }
}


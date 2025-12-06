using Core.Base;

namespace Services.DTO.EnrollmentDTO
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
        public string StudentName { get; set; }
        public string ParentName { get; set; }
        public string SchoolName { get; set; }
        public int Gradelevel { get; set; }
        public string Subject { get; set; }
        public string TeachingMethod { get; set; }
        public string Location { get; set; }
        public Guid StudentProfileId { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTimeOffset? ConfirmedAt { get; set; }
        public DateTimeOffset? CancelledAt { get; set; }
        public string? CancelReason { get; set; }
    }

    public class StudentEnrollmentResponse
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Subject { get; set; }
        public string TeachingMethod { get; set; }
        public string Location { get; set; }
        public Guid StudentProfileId { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTimeOffset? ConfirmedAt { get; set; }
        public DateTimeOffset? CancelledAt { get; set; }
        public string? CancelReason { get; set; }
    }
}


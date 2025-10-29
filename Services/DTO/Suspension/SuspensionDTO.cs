namespace Services.DTO.Suspension
{
    public class BanRequest
    {
        public string Reason { get; set; } = string.Empty;
        public DateTimeOffset SuspendedFrom { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? SuspendedTo { get; set; }
    }

    public class UserSuspensionRecordResponse
    {
        public Guid Id { get; set; }
        public string BannedUserFullName { get; set; } = string.Empty;
        public string AdminFullName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTimeOffset SuspendedFrom { get; set; }
        public DateTimeOffset? SuspendedTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }

    public class CourseSuspensionRecordResponse
    {
        public Guid SuspensionId { get; set; }
        public string CourseTitle { get; set; }
        public string Subject { get; set; }
        public string TeacherName { get; set; }
        public string AdminName { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }

    public class SuspensionRecordResponse
    {
        public Guid Id { get; set; }
        public Guid BannedId { get; set; }
        public string Type { get; set; }
        public string BanBy { get; set; }
        public DateTimeOffset SuspendedFrom { get; set; }
        public DateTimeOffset? SuspendedTo { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }

    public class UpdateSuspensionRecordRequest
    {
        public string? Reason { get; set; }
        public DateTimeOffset? SuspendedTo { get; set; }
    }
}

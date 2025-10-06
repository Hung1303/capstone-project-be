using Core.Base;

namespace BusinessObjects
{
    public class GeneratedReport : BaseEntity
    {
        public ReportType ReportType { get; set; }
        public string ParametersJson { get; set; } = string.Empty;
        public string StoragePath { get; set; } = string.Empty;
        public string Format { get; set; } = "pdf";
        public long FileSizeBytes { get; set; }
        public Guid RequestedByUserId { get; set; }
        public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;

        public virtual User RequestedByUser { get; set; } = null!;
    }
}



using Core.Base;

namespace Services.DTO.GeneratedReport
{
    public class CreateGeneratedReportRequest
    {
        public ReportType ReportType { get; set; }
        public string ParametersJson { get; set; } = string.Empty;
        public string StoragePath { get; set; } = string.Empty;
        public string Format { get; set; } = "pdf";
        public Guid RequestedByUserId { get; set; }
    }

    public class UpdateGeneratedReportRequest
    {
        public ReportType? ReportType { get; set; }
        public string? ParametersJson { get; set; }
        public string? StoragePath { get; set; }
        public string? Format { get; set; }
        public Guid? RequestedByUserId { get; set; }
    }

    public class GeneratedReportResponse
    {
        public Guid Id { get; set; }
        public ReportType ReportType { get; set; }
        public string ParametersJson { get; set; } = string.Empty;
        public string StoragePath { get; set; } = string.Empty;
        public string Format { get; set; } = "pdf";
        public long FileSizeBytes { get; set; }
        public Guid RequestedByUserId { get; set; }
        public DateTimeOffset GeneratedAt { get; set; }
    }
}



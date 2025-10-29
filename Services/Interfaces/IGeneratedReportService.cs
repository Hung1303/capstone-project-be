using Services.DTO.GeneratedReport;
using Core.Base;

namespace Services.Interfaces
{
    public interface IGeneratedReportService
    {
        Task<GeneratedReportResponse> CreateAsync(CreateGeneratedReportRequest request);
        Task<IEnumerable<GeneratedReportResponse>> GetAllAsync(string? searchTerm, ReportType? reportType, Guid? requestedByUserId, int pageNumber, int pageSize);
        Task<GeneratedReportResponse?> GetByIdAsync(Guid id);
        Task<GeneratedReportResponse> UpdateAsync(Guid id, UpdateGeneratedReportRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<(byte[] Content, string ContentType, string FileName)?> DownloadAsync(Guid id);
    }
}



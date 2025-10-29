using BusinessObjects;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.DTO.GeneratedReport;
using Services.Interfaces;

namespace Services
{
    public class GeneratedReportService : IGeneratedReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GeneratedReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneratedReportResponse> CreateAsync(CreateGeneratedReportRequest request)
        {
            var user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Id == request.RequestedByUserId && !u.IsDeleted);
            if (user == null)
            {
                throw new Exception("RequestedByUser not found");
            }

            var entity = new GeneratedReport
            {
                ReportType = request.ReportType,
                ParametersJson = request.ParametersJson ?? string.Empty,
                StoragePath = request.StoragePath ?? string.Empty,
                Format = string.IsNullOrWhiteSpace(request.Format) ? "pdf" : request.Format,
                RequestedByUserId = request.RequestedByUserId,
                GeneratedAt = DateTimeOffset.UtcNow,
            };

            await _unitOfWork.GetRepository<GeneratedReport>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return ToResponse(entity);
        }

        public async Task<IEnumerable<GeneratedReportResponse>> GetAllAsync(string? searchTerm, ReportType? reportType, Guid? requestedByUserId, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.GetRepository<GeneratedReport>().Entities.Where(r => !r.IsDeleted);

            if (reportType.HasValue)
            {
                query = query.Where(r => r.ReportType == reportType.Value);
            }
            if (requestedByUserId.HasValue)
            {
                query = query.Where(r => r.RequestedByUserId == requestedByUserId.Value);
            }
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lower = searchTerm.Trim().ToLower();
                query = query.Where(r => (r.StoragePath != null && r.StoragePath.ToLower().Contains(lower)) || (r.ParametersJson != null && r.ParametersJson.ToLower().Contains(lower)) || (r.Format != null && r.Format.ToLower().Contains(lower)));
            }

            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;

            var items = await query
                .OrderByDescending(r => r.GeneratedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(r => new GeneratedReportResponse
                {
                    Id = r.Id,
                    ReportType = r.ReportType,
                    ParametersJson = r.ParametersJson,
                    StoragePath = r.StoragePath,
                    Format = r.Format,
                    FileSizeBytes = r.FileSizeBytes,
                    RequestedByUserId = r.RequestedByUserId,
                    GeneratedAt = r.GeneratedAt
                })
                .ToListAsync();

            return items;
        }

        public async Task<GeneratedReportResponse?> GetByIdAsync(Guid id)
        {
            var r = await _unitOfWork.GetRepository<GeneratedReport>().Entities.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (r == null) return null;
            return ToResponse(r);
        }

        public async Task<GeneratedReportResponse> UpdateAsync(Guid id, UpdateGeneratedReportRequest request)
        {
            var entity = await _unitOfWork.GetRepository<GeneratedReport>().Entities.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (entity == null)
            {
                throw new Exception("GeneratedReport not found");
            }

            if (request.ReportType.HasValue) entity.ReportType = request.ReportType.Value;
            if (request.ParametersJson != null) entity.ParametersJson = request.ParametersJson;
            if (request.StoragePath != null) entity.StoragePath = request.StoragePath;
            if (request.Format != null) entity.Format = request.Format;
            if (request.RequestedByUserId.HasValue)
            {
                var user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Id == request.RequestedByUserId && !u.IsDeleted);
                if (user == null)
                {
                    throw new Exception("RequestedByUser not found");
                }
                entity.RequestedByUserId = request.RequestedByUserId.Value;
            }

            await _unitOfWork.GetRepository<GeneratedReport>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();
            return ToResponse(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.GetRepository<GeneratedReport>().Entities.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (entity == null)
            {
                return false;
            }
            entity.IsDeleted = true;
            await _unitOfWork.GetRepository<GeneratedReport>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<(byte[] Content, string ContentType, string FileName)?> DownloadAsync(Guid id)
        {
            var entity = await _unitOfWork.GetRepository<GeneratedReport>().Entities.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (entity == null) return null;

            if (string.IsNullOrWhiteSpace(entity.StoragePath) || !File.Exists(entity.StoragePath))
            {
                return null;
            }

            var bytes = await File.ReadAllBytesAsync(entity.StoragePath);
            var contentType = GetContentTypeByFormat(entity.Format);
            var fileName = $"report_{entity.Id}.{entity.Format}";
            return (bytes, contentType, fileName);
        }

        private static string GetContentTypeByFormat(string? format)
        {
            var f = (format ?? "").Trim().ToLower();
            return f switch
            {
                "pdf" => "application/pdf",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                "json" => "application/json",
                _ => "application/octet-stream"
            };
        }

        private static GeneratedReportResponse ToResponse(GeneratedReport r)
        {
            return new GeneratedReportResponse
            {
                Id = r.Id,
                ReportType = r.ReportType,
                ParametersJson = r.ParametersJson,
                StoragePath = r.StoragePath,
                Format = r.Format,
                FileSizeBytes = r.FileSizeBytes,
                RequestedByUserId = r.RequestedByUserId,
                GeneratedAt = r.GeneratedAt
            };
        }
    }
}



using BusinessObjects.DTO.Suspension;
using static Services.SuspensionService;

namespace Services.Interfaces
{
    public interface ISuspensionService
    {
        Task<bool> BanUser(Guid userId, Guid supervisorId, BanRequest record);
        Task<bool> BanCourse(Guid courseId, Guid supervisorId, BanRequest record);
        Task<(IEnumerable<UserSuspensionRecordResponse> Records, int TotalCount)> GetSuspensionRecordsAsync(
            int pageNumber, int pageSize, string? search = null);
        Task<PagedResult<CourseSuspensionRecordResponse>> GetAllCourseSuspensionRecordsAsync(
            int pageNumber = 1, int pageSize = 5, string? searchKeyword = null);
        Task<SuspensionRecordResponse> GetRecordById(Guid Id);
        Task<bool> RemoveBan(Guid suspensionRecordId, Guid moderatorId);
    }
}

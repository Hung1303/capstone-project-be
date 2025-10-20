using BusinessObjects;
using BusinessObjects.DTO.ApprovalRequest;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.Interfaces;

namespace API.Services
{
    public class ApprovalRequestService : IApprovalRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApprovalRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Tạo yêu cầu duyệt khóa học
        /// </summary>
        public async Task<bool> CreateApprovalRequestAsync(Guid courseId, Guid requestedByUserId, string? notes = null)
        {
            var courseRepo = _unitOfWork.GetRepository<Course>();
            var approvalRepo = _unitOfWork.GetRepository<ApprovalRequest>();

            var course = await courseRepo.Entities.FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted);
            if (course == null) return false;

            if (course.Status != CourseStatus.Draft)
                throw new Exception("Only courses in Draft status can be submitted for approval.");

            // Tạo mới request
            var request = new ApprovalRequest
            {
                CourseId = course.Id,
                Notes = notes,
                Decision = ApprovalDecision.Pending
            };

            // Đổi trạng thái khóa học thành PendingApproval
            course.Status = CourseStatus.PendingApproval;

            await approvalRepo.InsertAsync(request);
            await courseRepo.UpdateAsync(course);
            await _unitOfWork.SaveAsync();

            return true;
        }

        /// <summary>
        /// Duyệt hoặc từ chối yêu cầu (chỉ Admin / Inspector)
        /// </summary>
        public async Task<bool> ReviewApprovalRequestAsync(Guid approvalRequestId, Guid reviewerUserId, ApprovalDecision decision, string? notes = null)
        {
            var approvalRepo = _unitOfWork.GetRepository<ApprovalRequest>();
            var userRepo = _unitOfWork.GetRepository<User>();
            var courseRepo = _unitOfWork.GetRepository<Course>();

            var request = await approvalRepo.Entities.Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == approvalRequestId && !r.IsDeleted);
            if (request == null) return false;

            var reviewer = await userRepo.Entities
                .FirstOrDefaultAsync(u => u.Id == reviewerUserId && !u.IsDeleted && (u.Role == UserRole.Admin || u.Role == UserRole.Inspector));
            if (reviewer == null)
                throw new Exception("Only Admin or Inspector can approve/reject courses.");

            // Cập nhật request
            request.Decision = decision;
            request.DecidedByUserId = reviewer.Id;
            request.DecidedAt = DateTimeOffset.UtcNow;
            request.Notes = notes ?? request.Notes;

            // Cập nhật trạng thái khóa học
            if (request.Course != null)
            {
                request.Course.Status = decision switch
                {
                    ApprovalDecision.Approved => CourseStatus.Approved,
                    ApprovalDecision.Rejected => CourseStatus.Rejected,
                    _ => request.Course.Status
                };

                await courseRepo.UpdateAsync(request.Course);
            }

            await approvalRepo.UpdateAsync(request);
            await _unitOfWork.SaveAsync();

            return true;
        }

        /// <summary>
        /// Lấy danh sách yêu cầu duyệt
        /// </summary>
        public async Task<(IEnumerable<ApprovalRequestResponse> Records, int TotalCount)> GetApprovalRequestsAsync(int pageNumber, int pageSize, string? searchKeyword = null)
        {
            var approvalRepo = _unitOfWork.GetRepository<ApprovalRequest>().Entities;
            var courseRepo = _unitOfWork.GetRepository<Course>().Entities;
            var userRepo = _unitOfWork.GetRepository<User>().Entities;

            var query = from a in approvalRepo
                        join c in courseRepo on a.CourseId equals c.Id
                        join u in userRepo on a.DecidedByUserId equals u.Id into reviewerGroup
                        from reviewer in reviewerGroup.DefaultIfEmpty()
                        where !a.IsDeleted
                        select new { a, c, reviewer };

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.c.Title, $"%{searchKeyword}%") ||
                    EF.Functions.Like(x.reviewer.FullName, $"%{searchKeyword}%"));
            }

            var totalCount = await query.CountAsync();

            var records = await query
                .OrderByDescending(x => x.a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ApprovalRequestResponse
                {
                    Id = x.a.Id,
                    CourseTitle = x.c.Title,
                    Decision = x.a.Decision.ToString(),
                    DecidedBy = x.reviewer != null ? x.reviewer.FullName : "(Pending)",
                    DecidedAt = x.a.DecidedAt,
                    Notes = x.a.Notes,
                    CreatedAt = x.a.CreatedAt
                })
                .ToListAsync();

            return (records, totalCount);
        }

        /// <summary>
        /// Lấy chi tiết yêu cầu duyệt
        /// </summary>
        public async Task<ApprovalRequestResponse> GetApprovalRequestByIdAsync(Guid id)
        {
            var request = await _unitOfWork.GetRepository<ApprovalRequest>().Entities
                .Include(r => r.Course)
                .Include(r => r.DecidedByUser)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (request == null) throw new Exception("Approval request not found.");

            return new ApprovalRequestResponse
            {
                Id = request.Id,
                CourseTitle = request.Course?.Title ?? "(Unknown Course)",
                Decision = request.Decision.ToString(),
                DecidedBy = request.DecidedByUser?.FullName ?? "(Pending)",
                DecidedAt = request.DecidedAt,
                Notes = request.Notes,
                CreatedAt = request.CreatedAt
            };
        }
    }
}


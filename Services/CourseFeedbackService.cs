using BusinessObjects;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.DTO.Feedbacks;
using Services.Interfaces;

namespace Services
{
    public class CourseFeedbackService : ICourseFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseFeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CourseFeedbackResponse> CreateCourseFeedback(
            Guid courseId, Guid reviewerId, CreateCourseFeedbackRequest request)
        {
            var course = await _unitOfWork.GetRepository<Course>().Entities
                .FirstOrDefaultAsync(t => t.Id == courseId &&
                                          t.Status == CourseStatus.Approved &&
                                          !t.IsDeleted);

            var reviewer = await _unitOfWork.GetRepository<User>().Entities
                .Include(s => s.StudentProfile)
                .Include(p => p.ParentProfile)
                .ThenInclude(pp => pp.StudentProfiles)
                .FirstOrDefaultAsync(x => x.Id == reviewerId &&
                                          !x.IsDeleted &&
                                          x.Status == AccountStatus.Active);

            if (course == null || reviewer == null)
                return null;

            if (reviewer.Role == UserRole.Student && reviewer.StudentProfile != null)
            {
                // Verify student enrollment
                var isStudentEnrolled = await _unitOfWork.GetRepository<Enrollment>().Entities
                    .AnyAsync(e =>
                        e.CourseId == course.Id &&
                        e.StudentProfileId == reviewer.StudentProfile.Id &&
                        e.Status == EnrollmentStatus.Confirmed &&
                        !e.IsDeleted
                    );

                if (!isStudentEnrolled)
                    throw new Exception("Chỉ học sinh đăng kí khóa học mới được đánh giá.");

                var feedback = new CourseFeedback
                {
                    CourseId = course.Id,
                    StudentProfileId = reviewer.StudentProfile.Id,
                    ParentProfileId = null,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    Status = ReviewStatus.PendingModeration
                };

                await _unitOfWork.GetRepository<CourseFeedback>().InsertAsync(feedback);
                await _unitOfWork.SaveAsync();

                return new CourseFeedbackResponse
                {
                    Id = feedback.Id,
                    CourseId = feedback.CourseId,
                    StudentProfileId = feedback.StudentProfileId,
                    ParentProfileId = feedback.ParentProfileId,
                    Rating = feedback.Rating,
                    Comment = feedback.Comment,
                    Status = feedback.Status,
                    CreatedAt = feedback.CreatedAt,
                    LastUpdatedAt = feedback.LastUpdatedAt
                };
            }

            
            if (reviewer.Role == UserRole.Parent && reviewer.ParentProfile != null)
            {
                // Verify any child is enrolled
                var isParentOfEnrolledStudent = await _unitOfWork.GetRepository<Enrollment>().Entities
                    .AnyAsync(e =>
                        e.CourseId == course.Id &&
                        e.StudentProfile.ParentProfileId == reviewer.ParentProfile.Id &&
                        e.Status == EnrollmentStatus.Confirmed &&
                        !e.IsDeleted
                    );

                if (!isParentOfEnrolledStudent)
                    throw new Exception("Chỉ phụ huynh của học sinh đăng kí khóa học mới được đánh giá.");

                var feedback = new CourseFeedback
                {
                    CourseId = course.Id,
                    StudentProfileId = null,
                    ParentProfileId = reviewer.ParentProfile.Id,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    Status = ReviewStatus.PendingModeration
                };

                await _unitOfWork.GetRepository<CourseFeedback>().InsertAsync(feedback);
                await _unitOfWork.SaveAsync();

                return new CourseFeedbackResponse
                {
                    Id = feedback.Id,
                    CourseId = feedback.CourseId,
                    StudentProfileId = feedback.StudentProfileId,
                    ParentProfileId = feedback.ParentProfileId,
                    Rating = feedback.Rating,
                    Comment = feedback.Comment,
                    Status = feedback.Status,
                    CreatedAt = feedback.CreatedAt,
                    LastUpdatedAt = feedback.LastUpdatedAt
                };
            }

            throw new Exception("Chỉ học sinh đăng kí khóa học và phụ huynh của học sinh đấy được quyền đánh giá.");
        }

        public async Task<CourseFeedbackResponse> GetCourseFeedbackById(Guid courseFeedbackId)
        {
            var feedback = await _unitOfWork.GetRepository<CourseFeedback>().Entities
                .FirstOrDefaultAsync(f => f.Id == courseFeedbackId && f.Status == ReviewStatus.Approved && !f.IsDeleted);

            if (feedback == null) return null;

            return new CourseFeedbackResponse
            {
                Id = feedback.Id,
                CourseId = feedback.CourseId,
                StudentProfileId = feedback.StudentProfileId,
                ParentProfileId = feedback.ParentProfileId,
                Rating = feedback.Rating,
                Comment = feedback.Comment,
                Status = feedback.Status,
                CreatedAt = feedback.CreatedAt,
                LastUpdatedAt = feedback.LastUpdatedAt
            };
        }

        public async Task<string> ApproveCourseFeedback(Guid feedbackId, Guid moderatorId, CourseFeedbackModerationRequest request)
        {
            var feedback = await _unitOfWork.GetRepository<CourseFeedback>().Entities
                .FirstOrDefaultAsync(f => f.Id == feedbackId && f.Status == ReviewStatus.PendingModeration && !f.IsDeleted);
            if (feedback == null) return null;

            var mod = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(m => m.Id == moderatorId && m.Role == UserRole.Admin && m.Status == AccountStatus.Active && !m.IsDeleted);
            if (mod == null) return null;

            feedback.ModeratedByUserId = mod.Id;
            feedback.Status = request.Status;
            feedback.ModerationNotes = request.ModerationNotes;
            feedback.ModeratedAt = DateTimeOffset.UtcNow;
            feedback.LastUpdatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<CourseFeedback>().UpdateAsync(feedback);
            await _unitOfWork.SaveAsync();

            return "Đánh giá được chấp thuận.";
        }

        public async Task<CourseFeedbackResponse> UpdateCourseFeedback(Guid feedbackId, UpdateCourseFeedbackRequest request)
        {
            var fb = await _unitOfWork.GetRepository<CourseFeedback>().Entities
                .FirstOrDefaultAsync(f => f.Id == feedbackId && f.Status == ReviewStatus.Approved && !f.IsDeleted);

            if (fb == null) return null;

            if (request.Rating == null)
            {
                fb.Rating = fb.Rating;
            }
            fb.Rating = (int)request.Rating;

            if (!string.IsNullOrEmpty(request.Comment))
            {
                fb.Comment = request.Comment;
            }

            fb.LastUpdatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<CourseFeedback>().UpdateAsync(fb);
            await _unitOfWork.SaveAsync();

            return new CourseFeedbackResponse
            {
                Id = fb.Id,
                CourseId = fb.CourseId,
                StudentProfileId = fb.StudentProfileId,
                ParentProfileId = fb.ParentProfileId,
                Rating = fb.Rating,
                Comment = fb.Comment,
                Status = fb.Status,
                CreatedAt = fb.CreatedAt,
                LastUpdatedAt = fb.LastUpdatedAt
            };
        }

        public async Task<bool> RemoveCourseFeedback(Guid id)
        {
            var fb = await _unitOfWork.GetRepository<CourseFeedback>().Entities
                .FirstOrDefaultAsync(f => f.Id == id && f.Status == ReviewStatus.Approved && !f.IsDeleted);

            if (fb == null) return false;

            fb.IsDeleted = true;

            await _unitOfWork.GetRepository<CourseFeedback>().UpdateAsync(fb);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<(IEnumerable<CourseFeedbackDetailResponse> Feedbacks, int TotalCount)> GetAllCoursesFeedbacks(CourseFeedbackQuery query)
        {
            var feedbackQuery = _unitOfWork.GetRepository<CourseFeedback>().Entities
                .AsNoTracking()
                .AsQueryable();

            if (query.Status.HasValue)
            {
                feedbackQuery = feedbackQuery.Where(f => f.Status == query.Status.Value);
            }

            int totalCount = await feedbackQuery.CountAsync();

            var feedbacks = await _unitOfWork.GetRepository<CourseFeedback>().Entities
                .OrderByDescending(f => f.CreatedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(f => new CourseFeedbackDetailResponse
                {
                    Id = f.Id,
                    CourseId = f.CourseId,
                    StudentProfileId = f.StudentProfileId,
                    ParentProfileId = f.ParentProfileId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    Status = f.Status,
                    ModerateByUserId = f.ModeratedByUserId ?? Guid.Empty,
                    ModerationNotes = f.ModerationNotes ?? string.Empty,
                    ModeratedAt = f.ModeratedAt ?? DateTimeOffset.MinValue
                })
            .ToListAsync();

            return (feedbacks, totalCount);
        }

        public async Task<(IEnumerable<CourseFeedbackDetailResponse> Feedbacks, int TotalCount)> GetAllFeedbackByCourse(Guid courseId, int pageNumber, int pageSize, int? rating)
        {
            int totalCount;
            var query = _unitOfWork.GetRepository<CourseFeedback>().Entities
                .Where(x => x.CourseId == courseId && !x.IsDeleted && x.Status == ReviewStatus.Approved);
            if (rating.HasValue && rating >= 1 && rating <= 5) query = query.Where(x => x.Rating == rating);
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new CourseFeedbackDetailResponse
                {
                    Id = f.Id,
                    CourseId = f.CourseId,
                    StudentProfileId = f.StudentProfileId,
                    ParentProfileId = f.ParentProfileId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    Status = f.Status,
                    ModerateByUserId = f.ModeratedByUserId ?? Guid.Empty,
                    ModerationNotes = f.ModerationNotes ?? string.Empty,
                    ModeratedAt = f.ModeratedAt ?? DateTimeOffset.MinValue
                })
                .ToListAsync();
            totalCount = items.Count();
            return (items, totalCount);
        }
    }
}

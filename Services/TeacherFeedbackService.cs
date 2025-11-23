using BusinessObjects;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.DTO.Feedbacks;
using Services.Interfaces;

namespace Services
{
    public class TeacherFeedbackService : ITeacherFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        public TeacherFeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TeacherFeedbackResponse> CreateTeacherFeedback(Guid teacherProfileId, Guid reviewerId, CreateTeacherFeedbackRequest request)
        {
            var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities
                .FirstOrDefaultAsync(t => t.Id == teacherProfileId);

            var reviewer = await _unitOfWork.GetRepository<User>().Entities
                .Include(s => s.StudentProfile)
                .Include(p => p.ParentProfile)
                .FirstOrDefaultAsync(x => x.Id == reviewerId && !x.IsDeleted && x.Status == AccountStatus.Active);

            if (teacher == null) return null;
            if (reviewer != null && reviewer.Role == UserRole.Student)
            {
                var feedback = new TeacherFeedback
                {
                    TeacherProfileId = teacher.Id,
                    StudentProfileId = reviewer.StudentProfile.Id,
                    ParentProfileId = null,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    SubmittedAt = DateTimeOffset.UtcNow,
                    Status = ReviewStatus.PendingModeration,
                    ModeratedByUserId = null,
                    ModeratedAt = null,
                    ModerationNotes = null
                };

                await _unitOfWork.GetRepository<TeacherFeedback>().InsertAsync(feedback);
                await _unitOfWork.SaveAsync();

                return new TeacherFeedbackResponse
                {
                    Id = feedback.Id,
                    TeacherProfileId = feedback.TeacherProfileId,
                    StudentProfileId = feedback.StudentProfileId,
                    ParentProfileId = feedback.ParentProfileId,
                    Rating = feedback.Rating,
                    Comment = feedback.Comment,
                    SubmittedAt = feedback.SubmittedAt,
                    Status = feedback.Status,
                    CreatedAt = feedback.CreatedAt,
                    LastUpdatedAt = feedback.LastUpdatedAt
                };
            }
            else if (reviewer != null && reviewer.Role == UserRole.Teacher)
            {
                var feedback = new TeacherFeedback
                {
                    TeacherProfileId = teacher.Id,
                    StudentProfileId = null,
                    ParentProfileId = reviewer.ParentProfile.Id,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    SubmittedAt = DateTimeOffset.UtcNow,
                    Status = ReviewStatus.PendingModeration,
                    ModeratedByUserId = null,
                    ModeratedAt = null,
                    ModerationNotes = null
                };

                await _unitOfWork.GetRepository<TeacherFeedback>().InsertAsync(feedback);
                await _unitOfWork.SaveAsync();

                return new TeacherFeedbackResponse
                {
                    Id = feedback.Id,
                    TeacherProfileId = feedback.TeacherProfileId,
                    StudentProfileId = feedback.StudentProfileId,
                    ParentProfileId = feedback.ParentProfileId,
                    Rating = feedback.Rating,
                    Comment = feedback.Comment,
                    SubmittedAt = feedback.SubmittedAt,
                    Status = feedback.Status,
                    CreatedAt = feedback.CreatedAt,
                    LastUpdatedAt = feedback.LastUpdatedAt
                };
            }

            return null;
        }

        public async Task<TeacherFeedbackResponse> GetTeacherFeedbackById(Guid feedbackId)
        {
            var feedback = await _unitOfWork.GetRepository<TeacherFeedback>().Entities
                .FirstOrDefaultAsync(f => f.Id == feedbackId && f.Status == ReviewStatus.Approved && !f.IsDeleted);

            if (feedback == null) return null;

            return new TeacherFeedbackResponse
            {
                Id = feedback.Id,
                TeacherProfileId = feedback.TeacherProfileId,
                StudentProfileId = feedback.StudentProfileId,
                ParentProfileId = feedback.ParentProfileId,
                Rating = feedback.Rating,
                Comment = feedback.Comment,
                SubmittedAt = feedback.SubmittedAt,
                Status = feedback.Status,
                CreatedAt = feedback.CreatedAt,
                LastUpdatedAt = feedback.LastUpdatedAt
            };
        }

        public async Task<TeacherFeedbackResponse?> ApproveTeacherFeedback(Guid feedbackId, Guid moderatorId, TeacherFeedbackModerationRequest request)
        {
            var feedback = await _unitOfWork.GetRepository<TeacherFeedback>().Entities
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

            await _unitOfWork.GetRepository<TeacherFeedback>().UpdateAsync(feedback);
            await _unitOfWork.SaveAsync();

            return new TeacherFeedbackResponse
            {
                Id = feedback.Id,
                TeacherProfileId = feedback.TeacherProfileId,
                StudentProfileId = feedback.StudentProfileId,
                ParentProfileId = feedback.ParentProfileId,
                Rating = feedback.Rating,
                Comment = feedback.Comment,
                SubmittedAt = feedback.SubmittedAt,
                Status = feedback.Status,
                CreatedAt = feedback.CreatedAt,
                LastUpdatedAt = feedback.LastUpdatedAt
            };
        }

        public async Task<TeacherFeedbackResponse> UpdateTeacherFeedback(Guid feedbackId, UpdateTeacherFeedbackRequest request)
        {
            var fb = await _unitOfWork.GetRepository<TeacherFeedback>().Entities
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

            await _unitOfWork.GetRepository<TeacherFeedback>().UpdateAsync(fb);
            await _unitOfWork.SaveAsync();

            return new TeacherFeedbackResponse
            {
                Id = fb.Id,
                TeacherProfileId = fb.TeacherProfileId,
                StudentProfileId = fb.StudentProfileId,
                ParentProfileId = fb.ParentProfileId,
                Rating = fb.Rating,
                Comment = fb.Comment,
                SubmittedAt = fb.SubmittedAt,
                Status = fb.Status,
                CreatedAt = fb.CreatedAt,
                LastUpdatedAt = fb.LastUpdatedAt
            };
        }

        public async Task<bool> RemoveTeacherFeedback(Guid id)
        {
            var fb = await _unitOfWork.GetRepository<TeacherFeedback>().Entities
                .FirstOrDefaultAsync(f => f.Id == id && f.Status == ReviewStatus.Approved && !f.IsDeleted);

            if (fb == null) return false;

            fb.IsDeleted = true;

            await _unitOfWork.GetRepository<TeacherFeedback>().UpdateAsync(fb);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<(IEnumerable<TeacherFeedbackDetailResponse> Feedbacks, int TotalCount)> GetAllTeacherFeedbacks(TeacherFeedbackQuery query)
        {
            var feedbackQuery = _unitOfWork.GetRepository<TeacherFeedback>().Entities
                .AsNoTracking()
                .AsQueryable();

            if (query.Status.HasValue)
            {
                feedbackQuery = feedbackQuery.Where(f => f.Status == query.Status.Value);
            }

            int totalCount = await feedbackQuery.CountAsync();

            var feedbacks = await _unitOfWork.GetRepository<TeacherFeedback>().Entities
                .OrderByDescending(f => f.SubmittedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(f => new TeacherFeedbackDetailResponse
                {
                    Id = f.Id,
                    TeacherProfileId = f.TeacherProfileId,
                    StudentProfileId = f.StudentProfileId,
                    ParentProfileId = f.ParentProfileId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    SubmittedAt = f.SubmittedAt,
                    Status = f.Status,
                    ModerateByUserId = f.ModeratedByUserId ?? Guid.Empty,
                    ModerationNotes = f.ModerationNotes ?? string.Empty,
                    ModeratedAt = f.ModeratedAt ?? DateTimeOffset.MinValue
                })
            .ToListAsync();

            return (feedbacks, totalCount);
        }

        public async Task<(IEnumerable<TeacherFeedbackDetailResponse> Feedbacks, int TotalCount)> GetAllFeedbackByTeacher(Guid teacherProfileId, int pageNumber, int pageSize, int? rating)
        {
            int totalCount;
            var query = _unitOfWork.GetRepository<TeacherFeedback>().Entities
                .Where(x => x.TeacherProfileId == teacherProfileId && !x.IsDeleted && x.Status == ReviewStatus.Approved);
            if (rating.HasValue && rating >= 1 && rating <= 5) query = query.Where(x => x.Rating == rating);
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new TeacherFeedbackDetailResponse
                {
                    Id = f.Id,
                    TeacherProfileId = f.TeacherProfileId,
                    StudentProfileId = f.StudentProfileId,
                    ParentProfileId = f.ParentProfileId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    SubmittedAt = f.SubmittedAt,
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

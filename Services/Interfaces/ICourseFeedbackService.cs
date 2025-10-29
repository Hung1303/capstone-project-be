using Services.DTO.Feedbacks;

namespace Services.Interfaces
{
    public interface ICourseFeedbackService
    {
        Task<CourseFeedbackResponse> CreateCourseFeedback(Guid courseId, Guid reviewerProfileId, CreateCourseFeedbackRequest request);
        Task<CourseFeedbackResponse> GetCourseFeedbackById(Guid courseFeedbackId);
        Task<string> ApproveCourseFeedback(Guid feedbackId, Guid moderatorId, CourseFeedbackModerationRequest request);
        Task<CourseFeedbackResponse> UpdateCourseFeedback(Guid feedbackId, UpdateCourseFeedbackRequest request);
        Task<bool> RemoveCourseFeedback(Guid id);
        Task<(IEnumerable<CourseFeedbackDetailResponse> Feedbacks, int TotalCount)> GetAllCoursesFeedbacks(CourseFeedbackQuery query);
    }
}

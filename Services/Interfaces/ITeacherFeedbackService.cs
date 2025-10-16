using BusinessObjects.DTO.Feedbacks;

namespace Services.Interfaces
{
    public interface ITeacherFeedbackService
    {
        Task<TeacherFeedbackResponse> CreateTeacherFeedback(Guid teacherProfileId, Guid reviewerProfileId, CreateTeacherFeedbackRequest request);
        Task<TeacherFeedbackResponse> GetTeacherFeedbackById(Guid feedbackId);
        Task<string> ApproveTeacherFeedback(Guid feedbackId, Guid moderatorId, TeacherFeedbackModerationRequest request);
        Task<TeacherFeedbackResponse> UpdateTeacherFeedback(Guid feedbackId, UpdateTeacherFeedbackRequest request);
        Task<bool> RemoveTeacherFeedback(Guid id);
    }
}

﻿using BusinessObjects.DTO.Feedbacks;

namespace Services.Interfaces
{
    public interface ITeacherFeedbackService
    {
        Task<TeacherFeedbackResponse> CreateTeacherFeedback(Guid teacherProfileId, Guid reviewerProfileId, CreateTeacherFeedbackRequest request);
        Task<TeacherFeedbackResponse> GetTeacherFeedbackById(Guid feedbackId);
        Task<TeacherFeedbackResponse?> ApproveTeacherFeedback(Guid feedbackId, Guid moderatorId, TeacherFeedbackModerationRequest request);
        Task<TeacherFeedbackResponse> UpdateTeacherFeedback(Guid feedbackId, UpdateTeacherFeedbackRequest request);
        Task<bool> RemoveTeacherFeedback(Guid id);
        Task<(IEnumerable<TeacherFeedbackDetailResponse> Feedbacks, int TotalCount)> GetAllTeacherFeedbacks(TeacherFeedbackQuery query);
    }
}

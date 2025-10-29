using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace Services.DTO.Feedbacks
{
    public class CreateCourseFeedbackRequest
    {
        [Range(1, 5)]
        public int Rating { get; set; }
        [StringLength(4000)]
        public string Comment { get; set; } = string.Empty;
    }

    public class CourseFeedbackResponse
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Guid? StudentProfileId { get; set; }
        public Guid? ParentProfileId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public string Comment { get; set; }
        public ReviewStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }

    public class CourseFeedbackModerationRequest
    {
        public ReviewStatus Status { get; set; }
        public string? ModerationNotes { get; set; }
    }

    public class UpdateCourseFeedbackRequest
    {
        [Range(1, 5)]
        public int? Rating { get; set; }
        public string? Comment { get; set; }
    }

    public class CourseFeedbackDetailResponse
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Guid? StudentProfileId { get; set; }
        public Guid? ParentProfileId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public string Comment { get; set; }
        public ReviewStatus Status { get; set; }
        public Guid ModerateByUserId { get; set; }
        public string ModerationNotes { get; set; }
        public DateTimeOffset ModeratedAt { get; set; }
    }

    public class CourseFeedbackQuery
    {
        public ReviewStatus? Status { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}



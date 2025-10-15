using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTO.Course
{
    public class CreateCourseRequest
    {
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public TeachingMethod TeachingMethod { get; set; }
        [Range(1000.00, (double)decimal.MaxValue, ErrorMessage = "Tuition Fee must be at least 1000")]
        public decimal TuitionFee { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int Capacity { get; set; }
        //public CourseStatus Status { get; set; }
        [Required]
        public Guid? TeacherProfileId { get; set; }
        public Guid? CenterProfileId { get; set; }
    }
    public class UpdateCourseRequest
    {
        public string? Title { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public TeachingMethod? TeachingMethod { get; set; }
        [Range(1000.00, (double)decimal.MaxValue, ErrorMessage = "Tuition Fee must be at least 1000")]
        public decimal? TuitionFee { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int? Capacity { get; set; }
        public CourseStatus? Status { get; set; }
        public Guid? TeacherProfileId { get; set; }
        public Guid? CenterProfileId { get; set; }
    }
    public class CourseResponse
    {
        public Guid id { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public TeachingMethod TeachingMethod { get; set; }
        public decimal TuitionFee { get; set; }
        public int Capacity { get; set; }
        public CourseStatus Status { get; set; }
        public Guid? TeacherProfileId { get; set; }
        public Guid? CenterProfileId { get; set; }
    }
}

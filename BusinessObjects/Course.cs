using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class Course : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public Semester Semester { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TeachingMethod TeachingMethod { get; set; }
        public decimal TuitionFee { get; set; }
        public int Capacity { get; set; }
        [Range(6, 12, ErrorMessage = "Grade level must be between 6 and 12.")]
        public int GradeLevel { get; set; }
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        public bool IsPublished { get; set; } = false;
        public DateTime? PublishedAt { get; set; } = null;
        public Guid? TeacherProfileId { get; set; }
        public virtual TeacherProfile TeacherProfile { get; set; }
        public Guid? CenterProfileId { get; set; }
        public virtual CenterProfile CenterProfile { get; set; }
        public virtual ICollection<SubjectBuilder> SubjectBuilders { get; set; } = new List<SubjectBuilder>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<CourseFeedback> CourseFeedbacks { get; set; } = new List<CourseFeedback>();
        public virtual ICollection<CourseResult> CourseResults { get; set; } = new List<CourseResult>();
        //public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
        //public virtual Syllabus? Syllabus { get; set; }
    }
}



using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class Syllabus : BaseEntity
    {
        //public Guid CourseId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid TeacherProfileId { get; set; }
        public string SyllabusName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Range(6, 12, ErrorMessage = "Grade level must be between 6 and 12.")]
        public int GradeLevel { get; set; }
        //public string Subject { get; set; } = string.Empty;

        public string AssessmentMethod { get; set; } = string.Empty;
        public string CourseMaterial { get; set; } = string.Empty;

        //public virtual Course Course { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual TeacherProfile TeacherProfile { get; set; }
        public virtual ICollection<LessonPlan> LessonPlans { get; set; } = new List<LessonPlan>();

    }
}

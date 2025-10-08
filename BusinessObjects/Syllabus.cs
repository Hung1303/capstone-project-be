using Core.Base;

namespace BusinessObjects
{
    public class Syllabus : BaseEntity
    {
        public Guid CourseId { get; set; }

        public string SyllabusName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string GradeLevel { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;

        public string AssessmentMethod { get; set; } = string.Empty;
        public string CourseMaterial { get; set; } = string.Empty;

        public virtual Course Course { get; set; }
        public virtual ICollection<LessonPlan> LessonPlans { get; set; } = new List<LessonPlan>();

    }
}

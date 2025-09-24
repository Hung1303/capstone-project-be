using BusinessObjects;
using Core.Base;

namespace BusinessObjects
{
    public class Course : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public TeachingMethod TeachingMethod { get; set; }
        public decimal TuitionFee { get; set; }
        public int Capacity { get; set; }
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        public Guid? TeacherProfileId { get; set; }
        public Guid? CenterProfileId { get; set; }
        public ICollection<ClassSchedule> Schedules { get; set; } = new List<ClassSchedule>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}



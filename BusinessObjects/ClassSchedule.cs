using Core.Base;

namespace BusinessObjects
{
    public class ClassSchedule : BaseEntity
    {
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? RoomOrLink { get; set; }
        public Guid TeacherProfileId { get; set; }
        public virtual TeacherProfile TeacherProfile { get; set; }
        public virtual ICollection<SubjectBuilder> SubjectBuilders { get; set; } = new List<SubjectBuilder>();
    }
}



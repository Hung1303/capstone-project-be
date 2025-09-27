using Core.Base;

namespace BusinessObjects
{
    public class ClassSchedule : BaseEntity
    {
        public Guid CourseId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? RoomOrLink { get; set; }
    }
}



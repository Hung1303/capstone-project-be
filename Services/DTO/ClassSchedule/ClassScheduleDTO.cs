using System.ComponentModel.DataAnnotations;

namespace Services.DTO.ClassSchedule
{
    public class CreateClassScheduleRequest : IValidatableObject
    {
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public Guid TeacherProfileId { get; set; }
        public string? RoomOrLink { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                if (EndDate.Value < StartDate.Value)
                {
                    yield return new ValidationResult(
                        "Ngày kết thúc phải lớn hơn ngày bắt đầu.",
                        new[] { nameof(EndDate), nameof(StartDate) }
                    );
                }
            }
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "Giờ kết thúc phải lớn hơn giờ bắt đầu.",
                    new[] { nameof(EndDate), nameof(StartDate) }
                );
            }
        }
    }
    public class UpdateClassScheduleRequest : IValidatableObject
    {
        public string? ClassName { get; set; }
        public string? ClassDescription { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public Guid? TeacherProfileId { get; set; }
        public string? RoomOrLink { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                if (EndDate.Value < StartDate.Value)
                {
                    yield return new ValidationResult(
                        "Ngày kết thúc phải lớn hơn ngày bắt đầu.",
                        new[] { nameof(EndDate), nameof(StartDate) }
                    );
                }
            }
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                   "Giờ kết thúc phải lớn hơn giờ bắt đầu.",
                    new[] { nameof(EndDate), nameof(StartDate) }
                );
            }
        }
    }
    public class ClassScheduleResponse
    {
        public Guid Id { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public Guid? TeacherProfileId { get; set; }
        public string? RoomOrLink { get; set; }
    }
}

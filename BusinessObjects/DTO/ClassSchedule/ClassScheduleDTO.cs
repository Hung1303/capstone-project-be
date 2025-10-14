using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.ClassSchedule
{
    public class CreateClassScheduleRequest : IValidatableObject
    {
        public Guid CourseId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? RoomOrLink { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                if (EndDate.Value < StartDate.Value)
                {
                    yield return new ValidationResult(
                        "The EndDate must be lower than the StartDate.",
                        new[] { nameof(EndDate), nameof(StartDate) }
                    );
                }
            }
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "The EndTime must be strictly greater than the StartTime.",
                    new[] { nameof(EndDate), nameof(StartDate) }
                );
            }
        }
    }
    public class UpdateClassScheduleRequest : IValidatableObject
    {
        public Guid? CourseId { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? RoomOrLink { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                if (EndDate.Value < StartDate.Value)
                {
                    yield return new ValidationResult(
                        "The EndDate must not be lower than the StartDate.",
                        new[] { nameof(EndDate), nameof(StartDate) }
                    );
                }
            }
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "The EndTime must be strictly greater than the StartTime.",
                    new[] { nameof(EndDate), nameof(StartDate) }
                );
            }
        }
    }
    public class ClassScheduleResponse
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? RoomOrLink { get; set; }
    }
}

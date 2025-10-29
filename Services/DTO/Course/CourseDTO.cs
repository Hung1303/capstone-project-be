﻿using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace Services.DTO.Course
{
    public class CreateCourseRequest : IValidatableObject
    {
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public Semester Semester { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TeachingMethod TeachingMethod { get; set; }
        [Range(1000.00, (double)decimal.MaxValue, ErrorMessage = "Tuition Fee must be at least 1000")]
        public decimal TuitionFee { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int Capacity { get; set; }
        //public CourseStatus Status { get; set; }
        [Required]
        public Guid? TeacherProfileId { get; set; }
        public Guid? CenterProfileId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate < EndDate)
            {
                yield return new ValidationResult(
                    "The StartDate must be lower than the EndDate.",
                    new[] { nameof(EndDate), nameof(StartDate) }
                );
            }
        }
    }
    public class CreateSubjectForCourseRequest
    {
        public Guid CourseId { get; set; }
        public Guid ClassScheduleId { get; set; }
        public Guid SubjectId { get; set; }
        public string status { get; set; }
    }
    public class UpdateCourseSubject
    {
        public Guid? ClassScheduleId { get; set; }
        public string? status { get; set; }
    }
    public class CourseSubjectResponse
    {
        public Guid id { get; set; }
        public Guid CourseId { get; set; }
        public Guid ClassScheduleId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid TeacherProfileId { get; set; }
        public string status { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? RoomOrLink { get; set; }
    }
    public class UpdateCourseRequest : IValidatableObject
    {
        public string? Title { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public Semester? Semester { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public TeachingMethod? TeachingMethod { get; set; }
        [Range(1000.00, (double)decimal.MaxValue, ErrorMessage = "Tuition Fee must be at least 1000")]
        public decimal? TuitionFee { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int? Capacity { get; set; }
        public CourseStatus? Status { get; set; }
        public Guid? TeacherProfileId { get; set; }
        public Guid? CenterProfileId { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate < EndDate)
            {
                yield return new ValidationResult(
                    "The StartDate must be lower than the EndDate.",
                    new[] { nameof(EndDate), nameof(StartDate) }
                );
            }
        }
    }
    public class CourseResponse
    {
        public Guid id { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public Semester Semester { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TeachingMethod TeachingMethod { get; set; }
        public decimal TuitionFee { get; set; }
        public int Capacity { get; set; }
        public CourseStatus Status { get; set; }
        public Guid? TeacherProfileId { get; set; }
        public Guid? CenterProfileId { get; set; }
    }
}

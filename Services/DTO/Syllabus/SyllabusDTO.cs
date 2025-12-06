using System.ComponentModel.DataAnnotations;

namespace Services.DTO.Syllabus
{
    public class CreateSyllabusRequest
    {
        public string SyllabusName { get; set; }
        public string Description { get; set; }
        [Range(6, 12, ErrorMessage = "Grade level must be between 6 and 12.")]
        public int GradeLevel { get; set; }
        public string AssessmentMethod { get; set; }
        public string CourseMaterial { get; set; }
        public Guid SubjectId { get; set; }
        public Guid TeacherProfileId { get; set; }

    }
    public class UpdateSyllabusRequest
    {
        public string? SyllabusName { get; set; }
        public string? Description { get; set; }
        [Range(6, 12, ErrorMessage = "Grade level must be between 6 and 12.")]
        public int? GradeLevel { get; set; }
        public string? AssessmentMethod { get; set; }
        public string? CourseMaterial { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? TeacherProfileId { get; set; }
    }
    public class SyllabusResponse
    {
        public Guid Id { get; set; }
        public string SyllabusName { get; set; }
        public string Description { get; set; }
        [Range(6, 12, ErrorMessage = "Grade level must be between 6 and 12.")]
        public int GradeLevel { get; set; }
        public string AssessmentMethod { get; set; }
        public string CourseMaterial { get; set; }
        public Guid SubjectId { get; set; }
        public Guid TeacherProfileId { get; set; }
    }

    public class SyllabusResponse2
    {
        public Guid Id { get; set; }
        public string SyllabusName { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public string Description { get; set; }
        [Range(6, 12, ErrorMessage = "Grade level must be between 6 and 12.")]
        public int GradeLevel { get; set; }
        public string AssessmentMethod { get; set; }
        public string CourseMaterial { get; set; }
        public Guid SubjectId { get; set; }
        public Guid TeacherProfileId { get; set; }
    }
}

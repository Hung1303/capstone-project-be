namespace Services.DTO.Syllabus
{
    public class CreateSyllabusRequest
    {
        public string SyllabusName { get; set; }
        public string Description { get; set; }
        public string GradeLevel { get; set; }
        public string AssessmentMethod { get; set; }
        public string CourseMaterial { get; set; }
        public Guid SubjectId { get; set; }
        public Guid TeacherProfileId { get; set; }

    }
    public class UpdateSyllabusRequest
    {
        public string? SyllabusName { get; set; }
        public string? Description { get; set; }
        public string? GradeLevel { get; set; }
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
        public string GradeLevel { get; set; }
        public string AssessmentMethod { get; set; }
        public string CourseMaterial { get; set; }
        public Guid SubjectId { get; set; }
        public Guid TeacherProfileId { get; set; }
    }
}

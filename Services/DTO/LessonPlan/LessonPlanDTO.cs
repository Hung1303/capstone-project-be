namespace Services.DTO.LessonPlan
{
    public class CreateLessonPlanRequest
    {
        public Guid SyllabusId { get; set; }
        public string Topic { get; set; }
        public string StudentTask { get; set; }
        public string MaterialsUsed { get; set; }
        public string? Notes { get; set; }
    }
    public class UpdateLessonPlanRequest
    {
        public Guid? SyllabusId { get; set; }
        public string? Topic { get; set; }
        public string? StudentTask { get; set; }
        public string? MaterialsUsed { get; set; }
        public string? Notes { get; set; }
    }
    public class LessonPlanResponse
    {
        public Guid Id { get; set; }
        public Guid SyllabusId { get; set; }
        public string Topic { get; set; }
        public string StudentTask { get; set; }
        public string MaterialsUsed { get; set; }
        public string? Notes { get; set; }
    }
}

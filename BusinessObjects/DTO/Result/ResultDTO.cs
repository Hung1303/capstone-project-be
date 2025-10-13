namespace BusinessObjects.DTO.Result
{
    public class ResultDTO
    {
        public bool IsSuccess { get; set; }
        public int Code { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }
    }
}

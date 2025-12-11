using System.ComponentModel.DataAnnotations;

namespace Services.DTO.Email
{
    public class SendEmailToCentersRequest
    {
        [Required(ErrorMessage = "Cần tiêu đề")]
        [StringLength(200, ErrorMessage = "Tiêu đề không vượt quá 200 kí tự.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cần nội dung.")]
        public string Body { get; set; } = string.Empty;

        public List<Guid>? CenterIds { get; set; } // Optional: send to specific centers. If null, send to all centers
    }
}


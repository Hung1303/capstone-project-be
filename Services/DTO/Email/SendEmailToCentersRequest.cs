using System.ComponentModel.DataAnnotations;

namespace Services.DTO.Email
{
    public class SendEmailToCentersRequest
    {
        [Required(ErrorMessage = "Subject is required")]
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Body is required")]
        public string Body { get; set; } = string.Empty;

        public List<Guid>? CenterIds { get; set; } // Optional: send to specific centers. If null, send to all centers
    }
}


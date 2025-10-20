using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.ApprovalRequest
{
    public class ApprovalRequestResponse
    {
        public Guid Id { get; set; }
        public string CourseTitle { get; set; }
        public string Decision { get; set; }
        public string DecidedBy { get; set; }
        public DateTimeOffset? DecidedAt { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

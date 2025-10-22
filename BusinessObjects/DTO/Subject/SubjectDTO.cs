using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.Subject
{
    public class CreateSubjectRequest
    {
        public string SubjectName { get; set; }
        public string Description { get; set; }
    }

    public class UpdateSubjectRequest
    {
        public string? SubjectName { get; set; }
        public string? Description { get; set; }
    }

    public class SubjectResponse
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
    }
}

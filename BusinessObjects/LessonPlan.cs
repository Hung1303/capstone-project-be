using Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class LessonPlan : BaseEntity
    {
        public Guid SyllabusId { get; set; }

        public string Topic { get; set; }
        public string StudentTask { get; set; }
        public string MaterialsUsed { get; set; }
        public string? Notes { get; set; }

        public virtual Syllabus Syllabus { get; set; } = null!;
    }
}

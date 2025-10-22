using Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Subject : BaseEntity
    {
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public virtual Syllabus Syllabus { get; set; }
        public virtual ICollection<SubjectBuilder> SubjectBuilders { get; set; } = new List<SubjectBuilder>();
    }
}

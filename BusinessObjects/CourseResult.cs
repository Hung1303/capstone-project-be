using Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class CourseResult : BaseEntity
    {
        public float Mark {  get; set; }
        public string Comment { get; set; }
        public Guid StudentProfileId { get; set; }
        public Guid TeacherProfileId { get; set; }
        public Guid CourseId { get; set; }
        public virtual StudentProfile StudentProfile { get; set; }
        public virtual TeacherProfile TeacherProfile { get; set; }
        public virtual Course Course { get; set; }

    }
}

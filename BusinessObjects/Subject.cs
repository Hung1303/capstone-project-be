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
        public Guid TeacherProfileId { get; set; }
        public Guid CourseId { get; set; }
        public virtual Syllabus Syllabus { get; set; }
        public virtual TeacherProfile TeacherProfile { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();
    }
}

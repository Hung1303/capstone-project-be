using Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class SubjectBuilder : BaseEntity
    {
        public Guid CourseId { get; set; }
        public Guid ClassScheduleId { get; set; }
        public Guid SubjectId { get; set; }
        public string status { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ClassSchedule ClassSchedule { get; set; }
        public virtual Course Course { get; set; }
    }
}

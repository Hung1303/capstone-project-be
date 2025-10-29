using Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.CourseResult
{
    public class CreateCourseResultRequest 
    {
        [Range(0.00, 10.00, ErrorMessage = "Mark must be 0 - 10")]
        public float Mark { get; set; }
        public string Comment { get; set; }
        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }
        public Guid CourseId { get; set; }
    }
    public class CourseResultResponse
    {
        public float Mark { get; set; }
        public string Comment { get; set; }
        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }
        public Guid CourseId { get; set; }
    }
    public class UpdateCourseResultRequest
    {
        [Range(0.00, 10.00, ErrorMessage = "Mark must be 0 - 10")]
        public float? Mark { get; set; }
        public string? Comment { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? CourseId { get; set; }
    }
}

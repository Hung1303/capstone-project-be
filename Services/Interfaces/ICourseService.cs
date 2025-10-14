using BusinessObjects;
using BusinessObjects.DTO.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICourseService
    {
        Task<CourseResponse> CreateCourse(CreateCourseRequest request);
        Task<CourseResponse> UpdateCourse(Guid id, UpdateCourseRequest request);
        Task<IEnumerable<CourseResponse>> GetAllCourse(string? searchTerm, int pageNumber, int pageSize);
        Task<CourseResponse> GetCourseById(Guid id);
        Task<bool> DeleteCourse(Guid id);
    }
}

using BusinessObjects;
using BusinessObjects.DTO.Course;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CourseResponse> CreateCourse(CreateCourseRequest request)
        {
            if (request.TeacherProfileId == null && request.CenterProfileId == null)
            {
                throw new Exception("TeacherProfile or CenterProfile Not Found");
            }
            if (request.TeacherProfileId != null)
            {
                var checkTeacherProfile = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.TeacherProfileId);
                if (checkTeacherProfile == null)
                {
                    throw new Exception("TeacherProfile Not Found");
                }
            }
            if (request.CenterProfileId != null)
            {
                var checkCenterProfile = await _unitOfWork.GetRepository<CenterProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.CenterProfileId);
                if (checkCenterProfile == null)
                {
                    throw new Exception("CenterProfile Not Found");
                }
            }

            var course = new Course
            {
                Title = request.Title,
                Subject = request.Subject,
                Description = request.Description,
                Location = request.Location,
                TeachingMethod = request.TeachingMethod,
                TuitionFee = request.TuitionFee,
                Capacity = request.Capacity,
                Status = request.Status,
                TeacherProfileId = request.TeacherProfileId,
                CenterProfileId = request.CenterProfileId,
            };
            await _unitOfWork.GetRepository<Course>().InsertAsync(course);
            await _unitOfWork.SaveAsync();
            var result = new CourseResponse
            {
                id = course.Id,
                Title = course.Title,
                Subject = course.Subject,
                Description = course.Description,
                Location = course.Location,
                TeachingMethod = course.TeachingMethod,
                TuitionFee = course.TuitionFee,
                Capacity = course.Capacity,
                Status= course.Status,
                TeacherProfileId = course.TeacherProfileId,
                CenterProfileId = course.CenterProfileId,
            };
            return result;
        }

        public async Task<bool> DeleteCourse(Guid id)
        {
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (course == null)
            {
                throw new Exception("Course Not Found");
            }
            course.IsDeleted = true;
            await _unitOfWork.GetRepository<Course>().UpdateAsync(course);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<CourseResponse>> GetAllCourse(string? searchTerm, int pageNumber, int pageSize)
        {
            var courses = _unitOfWork.GetRepository<Course>().Entities.Where(a => !a.IsDeleted);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                courses = courses.Where(c =>
                    (c.Title != null && c.Title.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.Subject != null && c.Subject.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.Description != null && c.Description.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.Location != null && c.Location.ToLower().Contains(searchTerm.Trim().ToLower()))
                );
            }

            var totalCount = await courses.CountAsync();
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;
            var paginatedCourses = await courses
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(a => new CourseResponse
                {
                    id = a.Id,
                    Title = a.Title,
                    Subject = a.Subject,
                    Description = a.Description,
                    Location = a.Location,
                    TeachingMethod = a.TeachingMethod,
                    TuitionFee = a.TuitionFee,
                    Capacity = a.Capacity,
                    Status = a.Status,
                    TeacherProfileId = a.TeacherProfileId,
                    CenterProfileId = a.CenterProfileId,
                }).ToListAsync();
            return paginatedCourses;
        }

        public async Task<CourseResponse> GetCourseById(Guid id)
        {
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (course == null)
            {
                throw new Exception("Course Not Found");
            }
            var result = new CourseResponse
            {
                id = course.Id,
                Title = course.Title,
                Subject = course.Subject,
                Description = course.Description,
                Location = course.Location,
                TeachingMethod = course.TeachingMethod,
                TuitionFee = course.TuitionFee,
                Capacity = course.Capacity,
                Status= course.Status,
                TeacherProfileId = course.TeacherProfileId,
                CenterProfileId = course.CenterProfileId,
            };
            return result;
        }

        public async Task<CourseResponse> UpdateCourse(Guid id, UpdateCourseRequest request)
        {
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (course == null)
            {
                throw new Exception("Course Not Found");
            }
            if (request.Title != null)
            {
                course.Title = request.Title;
            }
            if (request.Subject != null)
            {
                course.Subject = request.Subject;
            }
            if (request.Description != null)
            {
                course.Description = request.Description;
            }
            if (request.Location != null)
            {
                course.Location = request.Location;
            }
            if (request.TeachingMethod.HasValue)
            {
                course.TeachingMethod = request.TeachingMethod.Value;
            }
            if (request.TuitionFee.HasValue)
            {
                course.TuitionFee = request.TuitionFee.Value;
            }
            if (request.Capacity.HasValue)
            {
                course.Capacity = request.Capacity.Value;
            }
            if (request.Status.HasValue)
            {
                course.Status = request.Status.Value;
            }
            if (request.TeacherProfileId != null) 
            {
                var checkTeacherProfile = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.TeacherProfileId);
                if (checkTeacherProfile == null)
                {
                    throw new Exception("TeacherProfile Not Found");
                }
                course.TeacherProfileId = request.TeacherProfileId;
            }
            if (request.CenterProfileId != null)
            {
                var checkCenterProfile = await _unitOfWork.GetRepository<CenterProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.CenterProfileId);
                if (checkCenterProfile == null)
                {
                    throw new Exception("CenterProfile Not Found");
                }
                course.CenterProfileId = request.CenterProfileId;
            }
            await _unitOfWork.GetRepository<Course>().UpdateAsync(course);
            await _unitOfWork.SaveAsync();

            var result = new CourseResponse
            {
                id = course.Id,
                Title = course.Title,
                Subject = course.Subject,
                Description = course.Description,
                Location = course.Location,
                TeachingMethod = course.TeachingMethod,
                TuitionFee = course.TuitionFee,
                Capacity = course.Capacity,
                Status = course.Status,
                TeacherProfileId = course.TeacherProfileId,
                CenterProfileId = course.CenterProfileId,
            };
            return result;
        }
    }
}

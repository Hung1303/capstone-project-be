using BusinessObjects;
using BusinessObjects.DTO.Course;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.Interfaces;

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
            if (request.TeacherProfileId == null)
            {
                throw new Exception("TeacherProfileId is required");
            }

            var teacher = await _unitOfWork
                .GetRepository<TeacherProfile>()
                .Entities
                .FirstOrDefaultAsync(a => a.Id == request.TeacherProfileId);
            if (teacher == null)
            {
                throw new Exception("TeacherProfile Not Found");
            }

            Guid? centerToUse = request.CenterProfileId;
            if (centerToUse != null)
            {
                var center = await _unitOfWork
                    .GetRepository<CenterProfile>()
                    .Entities
                    .FirstOrDefaultAsync(a => a.Id == centerToUse);
                if (center == null)
                {
                    throw new Exception("CenterProfile Not Found");
                }

                if (teacher.CenterProfileId == null || teacher.CenterProfileId != centerToUse)
                {
                    throw new Exception("Teacher does not belong to the specified Center");
                }
            }
            else
            {
                // Auto-assign center from teacher if any; remains null for independent teachers
                centerToUse = teacher.CenterProfileId;
            }

            var course = new Course
            {
                Title = request.Title,
                Subject = request.Subject,
                Description = request.Description,
                Location = request.Location,
                Semester = request.Semester,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TeachingMethod = request.TeachingMethod,
                TuitionFee = request.TuitionFee,
                Capacity = request.Capacity,
                Status = CourseStatus.Draft,
                TeacherProfileId = request.TeacherProfileId,
                CenterProfileId = centerToUse,
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
                Semester = course.Semester,
                StartDate= course.StartDate,
                EndDate= course.EndDate,
                TeachingMethod = course.TeachingMethod,
                TuitionFee = course.TuitionFee,
                Capacity = course.Capacity,
                Status = course.Status,
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

        public async Task<IEnumerable<CourseResponse>> GetAllCourse(string? searchTerm, int pageNumber, int pageSize, Guid? TeacherProfileId, Guid? CenterProfileId)
        {
            var courses = _unitOfWork.GetRepository<Course>().Entities.Where(a => !a.IsDeleted);
            if (TeacherProfileId.HasValue)
            {
                courses = courses.Where(a => a.TeacherProfileId == TeacherProfileId);
            }
            if (CenterProfileId.HasValue)

            {
                courses = courses.Where(a => a.CenterProfileId == CenterProfileId);
            }
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
                    Semester = a.Semester,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
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
                Semester = course.Semester,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                TeachingMethod = course.TeachingMethod,
                TuitionFee = course.TuitionFee,
                Capacity = course.Capacity,
                Status = course.Status,
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

            // Determine target teacher and center
            var targetTeacherId = request.TeacherProfileId ?? course.TeacherProfileId;
            if (targetTeacherId == null)
            {
                throw new Exception("Course must have a TeacherProfileId");
            }

            var teacher = await _unitOfWork
                .GetRepository<TeacherProfile>()
                .Entities
                .FirstOrDefaultAsync(a => a.Id == targetTeacherId);
            if (teacher == null)
            {
                throw new Exception("TeacherProfile Not Found");
            }

            Guid? targetCenterId = request.CenterProfileId ?? course.CenterProfileId;
            if (request.CenterProfileId == null && request.TeacherProfileId != null)
            {
                // If teacher changed and center not explicitly provided, sync center from teacher
                targetCenterId = teacher.CenterProfileId;
            }

            if (targetCenterId != null)
            {
                var center = await _unitOfWork
                    .GetRepository<CenterProfile>()
                    .Entities
                    .FirstOrDefaultAsync(a => a.Id == targetCenterId);
                if (center == null)
                {
                    throw new Exception("CenterProfile Not Found");
                }

                if (teacher.CenterProfileId == null || teacher.CenterProfileId != targetCenterId)
                {
                    throw new Exception("Teacher does not belong to the specified Center");
                }
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
            if (request.Semester.HasValue)
            {
                course.Semester = request.Semester.Value;
            }
            if (request.StartDate.HasValue)
            {
                course.StartDate = request.StartDate.Value;
            }
            if (request.EndDate.HasValue)
            {
                course.EndDate = request.EndDate.Value;
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
            // Apply ownership updates after validation above
            course.TeacherProfileId = targetTeacherId;
            course.CenterProfileId = targetCenterId;
            await _unitOfWork.GetRepository<Course>().UpdateAsync(course);
            await _unitOfWork.SaveAsync();

            var result = new CourseResponse
            {
                id = course.Id,
                Title = course.Title,
                Subject = course.Subject,
                Description = course.Description,
                Location = course.Location,
                Semester = course.Semester,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
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

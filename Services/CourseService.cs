using BusinessObjects;
using Services.DTO.Course;
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

        public async Task<CourseSubjectResponse> CreateSubjectForCourse(CreateSubjectForCourseRequest request)
        {
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == request.CourseId && !a.IsDeleted);
            var subject = await _unitOfWork.GetRepository<Subject>().Entities.FirstOrDefaultAsync(a => a.Id == request.SubjectId && !a.IsDeleted);
            var classSchedule = await _unitOfWork.GetRepository<ClassSchedule>().Entities.FirstOrDefaultAsync(a => a.Id == request.ClassScheduleId && !a.IsDeleted);

            if (course == null || subject == null || classSchedule == null)
            {
                throw new Exception("course or subject or classSchedule not found");
            }

            var subjectBuilder = new SubjectBuilder
            {
                CourseId = request.CourseId,
                SubjectId = request.SubjectId,
                ClassScheduleId = request.ClassScheduleId,
                status = request.status
            };
            await _unitOfWork.GetRepository<SubjectBuilder>().InsertAsync(subjectBuilder);
            await _unitOfWork.SaveAsync();
            var result = new CourseSubjectResponse
            {
                id = subjectBuilder.Id,
                CourseId = subjectBuilder.CourseId,
                ClassScheduleId = subjectBuilder.ClassScheduleId,
                SubjectId = subjectBuilder.SubjectId,
                status = subjectBuilder.status,
                SubjectName = subject.SubjectName,
                Description = subject.Description,
                DayOfWeek = classSchedule.DayOfWeek,
                StartDate = classSchedule.StartDate ?? null,
                EndDate = classSchedule.EndDate ?? null,
                StartTime = classSchedule.EndTime,
                RoomOrLink = classSchedule.RoomOrLink,
                TeacherProfileId = classSchedule.TeacherProfileId,
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

        public async Task<bool> DeleteCourseSubject(Guid id)
        {
            var subjectBuilder = await _unitOfWork.GetRepository<SubjectBuilder>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (subjectBuilder == null)
            {
                throw new Exception("Course Subject Not Found");
            }
            subjectBuilder.IsDeleted = true;
            await _unitOfWork.GetRepository<SubjectBuilder>().UpdateAsync(subjectBuilder);
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

        public async Task<IEnumerable<CourseSubjectResponse>> GetAllCourseSubject(string? searchTerm, int pageNumber, int pageSize, Guid? CourseId, Guid? TeacherProfileId, string? status)
        {
            var subjectBuilder = _unitOfWork.GetRepository<SubjectBuilder>().Entities
                .Include(a => a.Course)
                .Include(a => a.Subject)
                .Include(a => a.ClassSchedule)
                .Where(a => !a.IsDeleted);
            if (CourseId.HasValue)
            {
                subjectBuilder = subjectBuilder.Where(a => a.CourseId == CourseId);
            }
            if (TeacherProfileId.HasValue)
            {
                subjectBuilder = subjectBuilder.Where(a => a.ClassSchedule.TeacherProfileId == TeacherProfileId);
            }
            if (status != null)
            {
                subjectBuilder = subjectBuilder.Where(a => a.status == status);
            }
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                subjectBuilder = subjectBuilder.Where(c =>
                    (c.Subject.SubjectName != null && c.Subject.SubjectName.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.Subject.Description != null && c.Subject.Description.ToLower().Contains(searchTerm.Trim().ToLower()))
                );
            }

            var totalCount = await subjectBuilder.CountAsync();
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;
            var paginatedCourseSubjct = await subjectBuilder
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(a => new CourseSubjectResponse
                {
                    id = a.Id,
                    CourseId = a.CourseId,
                    ClassScheduleId = a.ClassScheduleId,
                    SubjectId = a.SubjectId,
                    status = a.status,
                    SubjectName = a.Subject.SubjectName,
                    Description = a.Subject.Description,
                    DayOfWeek = a.ClassSchedule.DayOfWeek,
                    StartDate = a.ClassSchedule.StartDate ?? null,
                    EndDate = a.ClassSchedule.EndDate ?? null,
                    StartTime = a.ClassSchedule.EndTime,
                    RoomOrLink = a.ClassSchedule.RoomOrLink,
                    TeacherProfileId = a.ClassSchedule.TeacherProfileId,
                }).ToListAsync();
            return paginatedCourseSubjct;
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

        public async Task<CourseSubjectResponse> GetCourseSubjectById(Guid id)
        {
            var subjectBuilder = await _unitOfWork.GetRepository<SubjectBuilder>().Entities
                .Include(a => a.Course)
                .Include(a => a.ClassSchedule)
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (subjectBuilder == null)
            {
                throw new Exception("Course Subject Not Found");
            }
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == subjectBuilder.CourseId && !a.IsDeleted);
            var subject = await _unitOfWork.GetRepository<Subject>().Entities.FirstOrDefaultAsync(a => a.Id == subjectBuilder.SubjectId && !a.IsDeleted);
            var classSchedule = await _unitOfWork.GetRepository<ClassSchedule>().Entities.FirstOrDefaultAsync(a => a.Id == subjectBuilder.ClassScheduleId && !a.IsDeleted);
            var result = new CourseSubjectResponse
            {
                id = subjectBuilder.Id,
                CourseId = subjectBuilder.CourseId,
                ClassScheduleId = subjectBuilder.ClassScheduleId,
                SubjectId = subjectBuilder.SubjectId,
                status = subjectBuilder.status,
                SubjectName = subject.SubjectName,
                Description = subject.Description,
                DayOfWeek = classSchedule.DayOfWeek,
                StartDate = classSchedule.StartDate ?? null,
                EndDate = classSchedule.EndDate ?? null,
                StartTime = classSchedule.EndTime,
                RoomOrLink = classSchedule.RoomOrLink,
                TeacherProfileId = classSchedule.TeacherProfileId,
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

        public async Task<CourseSubjectResponse> UpdateCourseSubject(Guid id, UpdateCourseSubject request)
        {
            var subjectBuilder = await _unitOfWork.GetRepository<SubjectBuilder>().Entities
                .Include(a => a.Course)
                .Include(a => a.ClassSchedule)
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (subjectBuilder == null)
            {
                throw new Exception("Course Not Found");
            }
            if (request.ClassScheduleId.HasValue)
            {
                subjectBuilder.ClassScheduleId = request.ClassScheduleId.Value;
            }
            if (request.status != null)
            {
                subjectBuilder.status = request.status;
            }
            await _unitOfWork.GetRepository<SubjectBuilder>().UpdateAsync(subjectBuilder);
            await _unitOfWork.SaveAsync();

            var result = new CourseSubjectResponse
            {
                id = subjectBuilder.Id,
                CourseId = subjectBuilder.CourseId,
                ClassScheduleId = subjectBuilder.ClassScheduleId,
                SubjectId = subjectBuilder.SubjectId,
                status = subjectBuilder.status,
                SubjectName = subjectBuilder.Subject.SubjectName,
                Description = subjectBuilder.Subject.Description,
                DayOfWeek = subjectBuilder.ClassSchedule.DayOfWeek,
                StartDate = subjectBuilder.ClassSchedule.StartDate ?? null,
                EndDate = subjectBuilder.ClassSchedule.EndDate ?? null,
                StartTime = subjectBuilder.ClassSchedule.EndTime,
                RoomOrLink = subjectBuilder.ClassSchedule.RoomOrLink,
                TeacherProfileId = subjectBuilder.ClassSchedule.TeacherProfileId,
            };
            return result;
        }
    }
}

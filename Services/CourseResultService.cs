using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.DTO.CourseResult;
using Services.Interfaces;

namespace Services
{
    public class CourseResultService : ICourseResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CourseResultService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CourseResultResponse> CreateCourseResult(CreateCourseResultRequest request)
        {
            var student = await _unitOfWork.GetRepository<StudentProfile>().Entities
                .Include(a => a.Enrollments)
                .FirstOrDefaultAsync(a => a.Id == request.StudentId && !a.IsDeleted);
            if (student == null)
            {
                throw new Exception("Không tìm thấy học sinh.");
            }
            var enrollments = student.Enrollments.Select(s => s.CourseId).ToList();
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == request.CourseId && enrollments.Contains(request.CourseId) && !a.IsDeleted);
            if (course == null)
            {
                throw new Exception("Khóa học không tồn tại hoặc không đăng kí.");
            }
            var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.TeacherId && !a.IsDeleted);
            if (teacher == null)
            {
                throw new Exception("Không tìm thấy giáo viên.");
            }
            var checkCourseResult = await _unitOfWork.GetRepository<CourseResult>().Entities
                .FirstOrDefaultAsync(a => a.CourseId == course.Id && a.StudentProfileId == student.Id && !a.IsDeleted);
            if (checkCourseResult != null)
            {
                throw new Exception("Kết quả khóa học của học sinh này đã có sẵn.");
            }
            var courseResult = new CourseResult
            {
                Mark = request.Mark,
                Comment = request.Comment,
                StudentProfileId = request.StudentId,
                CourseId = request.CourseId,
                TeacherProfileId = request.TeacherId,
            };
            await _unitOfWork.GetRepository<CourseResult>().InsertAsync(courseResult);
            await _unitOfWork.SaveAsync();
            var result = new CourseResultResponse
            {
                Mark = courseResult.Mark,
                Comment = courseResult.Comment,
                StudentId = courseResult.StudentProfileId,
                CourseId = courseResult.CourseId,
                TeacherId = courseResult.TeacherProfileId,
            };
            return result;
        }

        public async Task<bool> DeleteCourseResult(Guid id)
        {
            var courseResult = await _unitOfWork.GetRepository<CourseResult>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (courseResult == null)
            {
                throw new Exception("Không tìm thấy kết quả khóa học.");
            }
            courseResult.IsDeleted = true;
            await _unitOfWork.GetRepository<CourseResult>().UpdateAsync(courseResult);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<CourseResultResponse>> GetAllCourseResult(string? searchTerm, int pageNumber, int pageSize, Guid? TeacherProfileId, Guid? CourseId, Guid? StudentProfileId)
        {
            var courseResult = _unitOfWork.GetRepository<CourseResult>().Entities
                .Include(c => c.Course)
                .Where(a => !a.IsDeleted);
            if (TeacherProfileId.HasValue)
            {
                courseResult = courseResult.Where(a => a.TeacherProfileId == TeacherProfileId);
            }
            if (CourseId.HasValue)
            {
                courseResult = courseResult.Where(a => a.CourseId == CourseId);
            }
            if (StudentProfileId.HasValue)
            {
                courseResult = courseResult.Where(a => a.StudentProfileId == StudentProfileId);
            }
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                courseResult = courseResult.Where(c =>
                    (c.Comment != null && c.Comment.ToLower().Contains(searchTerm.Trim().ToLower()))
                );
            }

            var totalCount = await courseResult.CountAsync();
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;
            var paginatedCourseResult = await courseResult
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(a => new CourseResultResponse
                {
                    Subject = a.Course.Subject,
                    Mark = a.Mark,
                    Comment = a.Comment,
                    StudentId = a.StudentProfileId,
                    CourseId = a.CourseId,
                    TeacherId = a.TeacherProfileId,
                }).ToListAsync();
            return paginatedCourseResult;
        }

        public async Task<CourseResultResponse> GetCourseResultById(Guid id)
        {
            var courseResult = await _unitOfWork.GetRepository<CourseResult>().Entities
                .Include(c => c.Course)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (courseResult == null)
            {
                throw new Exception("Không tìm thấy kết quả khóa học.");
            }
            var result = new CourseResultResponse
            {
                Subject = courseResult.Course.Subject,
                Mark = courseResult.Mark,
                Comment = courseResult.Comment,
                StudentId = courseResult.StudentProfileId,
                CourseId = courseResult.CourseId,
                TeacherId = courseResult.TeacherProfileId,
            };
            return result;
        }

        public async Task<CourseResultResponse> UpdateCourseResult(Guid id, UpdateCourseResultRequest request)
        {
            var courseResult = await _unitOfWork.GetRepository<CourseResult>().Entities
                .Include(c => c.Course)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (courseResult == null)
            {
                throw new Exception("Không tìm thấy kết quả khóa học.");
            }
            if (request.Mark.HasValue)
            {
                courseResult.Mark = request.Mark.Value;
            }
            if (request.Comment != null)
            {
                courseResult.Comment = request.Comment;
            }
            if (request.CourseId.HasValue)
            {
                var checkCourse = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == request.CourseId && !a.IsDeleted);
                if (checkCourse == null)
                {
                    throw new Exception("Không tìm thấy kết quả khóa học.");
                }
                courseResult.CourseId = request.CourseId.Value;
            }
            if (request.TeacherId.HasValue)
            {
                var checkTeacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.TeacherId && !a.IsDeleted);
                if (checkTeacher == null)
                {
                    throw new Exception("Không tìm thấy giáo viên.");
                }
                courseResult.TeacherProfileId = request.TeacherId.Value;
            }
            if (request.StudentId.HasValue)
            {
                var checkStudent = await _unitOfWork.GetRepository<StudentProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.StudentId && !a.IsDeleted);
                if (checkStudent == null)
                {
                    throw new Exception("Không tìm thấy học sinh.");
                }
                courseResult.StudentProfileId = request.StudentId.Value;
            }
            await _unitOfWork.GetRepository<CourseResult>().UpdateAsync(courseResult);
            await _unitOfWork.SaveAsync();

            var result = new CourseResultResponse
            {
                Subject = courseResult.Course.Subject,
                Mark = courseResult.Mark,
                Comment = courseResult.Comment,
                StudentId = courseResult.StudentProfileId,
                CourseId = courseResult.CourseId,
                TeacherId = courseResult.TeacherProfileId,
            };
            return result;
        }

        public async Task<IEnumerable<CourseResultResponse>> GetAllCourseResultsByStudentId(Guid StudentProfileId, string? Subject, string? searchTerm, int pageNumber, int pageSize)
        {
            var courseResult = _unitOfWork.GetRepository<CourseResult>().Entities
                .Include(c => c.Course)
                .Where(a => !a.IsDeleted && a.StudentProfileId == StudentProfileId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                courseResult = courseResult.Where(c =>
                    (c.Comment != null && c.Comment.ToLower().Contains(searchTerm.Trim().ToLower()))
                );
            }

            if (!string.IsNullOrWhiteSpace(Subject))
            {
                courseResult = courseResult.Where(c =>
                    (c.Course.Subject != null && c.Course.Subject.ToLower().Contains(Subject.Trim().ToLower()))
                );
            }

            var totalCount = await courseResult.CountAsync();
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;
            var paginatedCourseResult = await courseResult
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(a => new CourseResultResponse
                {
                    Subject = a.Course.Subject,
                    Mark = a.Mark,
                    Comment = a.Comment,
                    StudentId = a.StudentProfileId,
                    CourseId = a.CourseId,
                    TeacherId = a.TeacherProfileId,
                }).ToListAsync();
            return paginatedCourseResult;
        }

        public async Task<IEnumerable<CourseResultResponse>> GetAllCourseResultsByTeacherId(Guid TeacherProfileId, string? Subject, string? searchTerm, int pageNumber, int pageSize)
        {
            var courseResult = _unitOfWork.GetRepository<CourseResult>().Entities
                .Include(c => c.Course)
                .Where(a => !a.IsDeleted && a.TeacherProfileId == TeacherProfileId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                courseResult = courseResult.Where(c =>
                    (c.Comment != null && c.Comment.ToLower().Contains(searchTerm.Trim().ToLower()))
                );
            }

            if (!string.IsNullOrWhiteSpace(Subject))
            {
                courseResult = courseResult.Where(c =>
                    (c.Course.Subject != null && c.Course.Subject.ToLower().Contains(Subject.Trim().ToLower()))
                );
            }

            var totalCount = await courseResult.CountAsync();
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;
            var paginatedCourseResult = await courseResult
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(a => new CourseResultResponse
                {
                    Subject = a.Course.Subject,
                    Mark = a.Mark,
                    Comment = a.Comment,
                    StudentId = a.StudentProfileId,
                    CourseId = a.CourseId,
                    TeacherId = a.TeacherProfileId,
                }).ToListAsync();
            return paginatedCourseResult;
        }

        public async Task<IEnumerable<CourseResultResponse>> GetAllCourseResultByParentId(string? searchTerm, string? Subject, int pageNumber, int pageSize, Guid ParentId)
        {
            var parent = await _unitOfWork.GetRepository<ParentProfile>().Entities
                .Include(c => c.StudentProfiles)
                .FirstOrDefaultAsync(a => !a.IsDeleted && a.Id == ParentId);
            if (parent == null)
            {
                throw new Exception("Không tìm thấy phụ huynh.");
            }
            var studentList = parent.StudentProfiles.Select(s => s.Id).ToList();
            var courseResult = _unitOfWork.GetRepository<CourseResult>().Entities
                .Include(c => c.Course)
                .Where(a => !a.IsDeleted && studentList.Contains(a.StudentProfileId));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                courseResult = courseResult.Where(c =>
                    (c.Comment != null && c.Comment.ToLower().Contains(searchTerm.Trim().ToLower()))
                );
            }

            if (!string.IsNullOrWhiteSpace(Subject))
            {
                courseResult = courseResult.Where(c =>
                    (c.Course.Subject != null && c.Course.Subject.ToLower().Contains(Subject.Trim().ToLower()))
                );
            }

            var totalCount = await courseResult.CountAsync();
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;
            var paginatedCourseResult = await courseResult
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(a => new CourseResultResponse
                {
                    Subject = a.Course.Subject,
                    Mark = a.Mark,
                    Comment = a.Comment,
                    StudentId = a.StudentProfileId,
                    CourseId = a.CourseId,
                    TeacherId = a.TeacherProfileId,
                }).ToListAsync();
            return paginatedCourseResult;
        }
    }
}

using BusinessObjects;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.DTO.EnrollmentDTO;
using Services.Interfaces;

namespace Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ✅ Helper method to check if teacher teaches at the same school and class as student
        private bool IsTeacherTeachingOwnStudent(TeacherProfile teacher, StudentProfile student)
        {
            // Check if teacher teaches at a school (if not set, they can teach anyone)
            if (string.IsNullOrWhiteSpace(teacher.TeachingAtSchool))
                return false;

            // Check if student is from the same school (case-insensitive)
            if (string.IsNullOrWhiteSpace(student.SchoolName))
                return false;

            if (!string.Equals(teacher.TeachingAtSchool.Trim(), student.SchoolName.Trim(), StringComparison.OrdinalIgnoreCase))
                return false;

            // If same school, check if teacher teaches the student's class
            if (string.IsNullOrWhiteSpace(teacher.TeachAtClasses) || string.IsNullOrWhiteSpace(student.ClassName))
                return false;

            // Split TeachAtClasses by comma or space and check if it contains student's class
            var teacherClasses = teacher.TeachAtClasses
                .Split(new[] { ',', ';', ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToList();

            var studentClassName = student.ClassName.Trim();

            // Check if any of the teacher's classes matches the student's class (case-insensitive)
            return teacherClasses.Any(tc => string.Equals(tc, studentClassName, StringComparison.OrdinalIgnoreCase));
        }

        // ✅ CREATE
        public async Task<EnrollmentResponse> CreateEnrollment(CreateEnrollmentRequest request)
        {
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(c => c.Id == request.CourseId && !c.IsDeleted);
            if (course == null)
                throw new Exception("Course not found");

            // ✅ Block new enrollments if course is full (count only confirmed enrollments)
            var confirmedCount = await _unitOfWork.GetRepository<Enrollment>().Entities
                .CountAsync(e => e.CourseId == request.CourseId && e.Status == EnrollmentStatus.Confirmed && !e.IsDeleted);

            if (confirmedCount >= course.Capacity)
                throw new Exception("Course capacity reached");

            var student = await _unitOfWork.GetRepository<StudentProfile>().Entities.FirstOrDefaultAsync(s => s.Id == request.StudentProfileId && !s.IsDeleted);
            if (student == null)
                throw new Exception("Student profile not found");

            // ✅ Validate grade level match
            if (student.GradeLevel != course.GradeLevel)
                throw new Exception($"Student grade level ({student.GradeLevel}) does not match course grade level ({course.GradeLevel}). Only students with grade {course.GradeLevel} can enroll in this course.");

            // ✅ Validate that teacher is not teaching their own student
            if (course.TeacherProfileId.HasValue)
            {
                var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities
                    .FirstOrDefaultAsync(t => t.Id == course.TeacherProfileId.Value && !t.IsDeleted);

                if (teacher != null && IsTeacherTeachingOwnStudent(teacher, student))
                {
                    throw new Exception($"Cannot enroll: The teacher teaches at the same school ({student.SchoolName}) and class ({student.ClassName}) as this student. Teachers cannot teach their own students.");
                }
            }

            var enrollment = new Enrollment
            {
                CourseId = request.CourseId,
                StudentProfileId = request.StudentProfileId,
                Status = EnrollmentStatus.Pending,
            };

            await _unitOfWork.GetRepository<Enrollment>().InsertAsync(enrollment);
            await _unitOfWork.SaveAsync();

            return new EnrollmentResponse
            {
                Id = enrollment.Id,
                CourseId = enrollment.CourseId,
                StudentProfileId = enrollment.StudentProfileId,
                Status = enrollment.Status,
                ConfirmedAt = enrollment.ConfirmedAt,
                CancelledAt = enrollment.CancelledAt,
                CancelReason = enrollment.CancelReason
            };
        }

        // ✅ GET BY ID
        public async Task<EnrollmentResponse> GetEnrollmentById(Guid id)
        {
            var enrollment = await _unitOfWork.GetRepository<Enrollment>().Entities
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (enrollment == null)
                throw new Exception("Enrollment not found");

            return new EnrollmentResponse
            {
                Id = enrollment.Id,
                CourseId = enrollment.CourseId,
                StudentProfileId = enrollment.StudentProfileId,
                Status = enrollment.Status,
                ConfirmedAt = enrollment.ConfirmedAt,
                CancelledAt = enrollment.CancelledAt,
                CancelReason = enrollment.CancelReason
            };
        }

        // ✅ GET ALL (với search + phân trang)
        public async Task<IEnumerable<EnrollmentResponse>> GetAllEnrollments(string? searchTerm, int pageNumber, int pageSize)
        {
            // ✅ Khởi tạo query
            var enrollmentsQuery = _unitOfWork.GetRepository<Enrollment>().Entities
                .Where(e => !e.IsDeleted)
                .Include(e => e.StudentProfile)
                    .ThenInclude(s => s.ParentProfile)
                .Include(e => e.Course)
                .AsQueryable(); // 👈 Thêm dòng này để chuyển sang IQueryable

            // ✅ Tìm kiếm
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();

                enrollmentsQuery = enrollmentsQuery.Where(e =>
                    (e.CancelReason != null && e.CancelReason.ToLower().Contains(searchTerm)) ||
                    (e.StudentProfile.SchoolName != null && e.StudentProfile.SchoolName.ToLower().Contains(searchTerm)) ||
                    (e.Course.Title != null && e.Course.Title.ToLower().Contains(searchTerm)) ||
                    (e.Course.Subject != null && e.Course.Subject.ToLower().Contains(searchTerm))
                );
            }

            // ✅ Tính tổng
            var totalCount = await enrollmentsQuery.CountAsync();

            // ✅ Phân trang
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;

            // ✅ Truy vấn & map về DTO
            var pagedEnrollments = await enrollmentsQuery
                .OrderByDescending(e => e.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(e => new EnrollmentResponse
                {
                    Id = e.Id,
                    CourseId = e.CourseId,
                    StudentProfileId = e.StudentProfileId,
                    Status = e.Status,
                    ConfirmedAt = e.ConfirmedAt,
                    CancelledAt = e.CancelledAt,
                    CancelReason = e.CancelReason
                })
                .ToListAsync();

            return pagedEnrollments;
        }



        // ✅ UPDATE
        public async Task<EnrollmentResponse> UpdateEnrollment(Guid id, UpdateEnrollmentRequest request)
        {
            var enrollment = await _unitOfWork.GetRepository<Enrollment>().Entities.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            if (enrollment == null)
                throw new Exception("Enrollment not found");

            if (request.CourseId.HasValue)
            {
                var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(c => c.Id == request.CourseId.Value && !c.IsDeleted);
                if (course == null)
                    throw new Exception("Course not found");

                // ✅ Validate grade level match if course is being changed
                var studentForValidation = await _unitOfWork.GetRepository<StudentProfile>().Entities
                    .FirstOrDefaultAsync(s => s.Id == enrollment.StudentProfileId && !s.IsDeleted);
                if (studentForValidation != null && studentForValidation.GradeLevel != course.GradeLevel)
                    throw new Exception($"Student grade level ({studentForValidation.GradeLevel}) does not match course grade level ({course.GradeLevel}). Only students with grade {course.GradeLevel} can enroll in this course.");

                // ✅ Validate that teacher is not teaching their own student
                if (course.TeacherProfileId.HasValue && studentForValidation != null)
                {
                    var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities
                        .FirstOrDefaultAsync(t => t.Id == course.TeacherProfileId.Value && !t.IsDeleted);

                    if (teacher != null && IsTeacherTeachingOwnStudent(teacher, studentForValidation))
                    {
                        throw new Exception($"Cannot update enrollment: The teacher teaches at the same school ({studentForValidation.SchoolName}) and class ({studentForValidation.ClassName}) as this student. Teachers cannot teach their own students.");
                    }
                }

                enrollment.CourseId = request.CourseId.Value;
            }

            if (request.StudentProfileId.HasValue)
            {
                var student = await _unitOfWork.GetRepository<StudentProfile>().Entities.FirstOrDefaultAsync(s => s.Id == request.StudentProfileId.Value && !s.IsDeleted);
                if (student == null)
                    throw new Exception("Student profile not found");

                // ✅ Validate grade level match if student is being changed
                var courseForValidation = await _unitOfWork.GetRepository<Course>().Entities
                    .FirstOrDefaultAsync(c => c.Id == enrollment.CourseId && !c.IsDeleted);
                if (courseForValidation != null && student.GradeLevel != courseForValidation.GradeLevel)
                    throw new Exception($"Student grade level ({student.GradeLevel}) does not match course grade level ({courseForValidation.GradeLevel}). Only students with grade {courseForValidation.GradeLevel} can enroll in this course.");

                // ✅ Validate that teacher is not teaching their own student
                if (courseForValidation != null && courseForValidation.TeacherProfileId.HasValue)
                {
                    var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities
                        .FirstOrDefaultAsync(t => t.Id == courseForValidation.TeacherProfileId.Value && !t.IsDeleted);

                    if (teacher != null && IsTeacherTeachingOwnStudent(teacher, student))
                    {
                        throw new Exception($"Cannot update enrollment: The teacher teaches at the same school ({student.SchoolName}) and class ({student.ClassName}) as this student. Teachers cannot teach their own students.");
                    }
                }

                enrollment.StudentProfileId = request.StudentProfileId.Value;
            }

            if (request.Status.HasValue)
            {
                // ✅ If attempting to confirm, ensure capacity not exceeded
                if (request.Status.Value == EnrollmentStatus.Confirmed)
                {
                    var courseForCapacity = await _unitOfWork.GetRepository<Course>().Entities
                        .FirstOrDefaultAsync(c => c.Id == enrollment.CourseId && !c.IsDeleted);

                    if (courseForCapacity == null)
                        throw new Exception("Course not found");

                    var confirmedCountForCourse = await _unitOfWork.GetRepository<Enrollment>().Entities
                        .CountAsync(e => e.CourseId == enrollment.CourseId && e.Status == EnrollmentStatus.Confirmed && !e.IsDeleted);

                    if (confirmedCountForCourse >= courseForCapacity.Capacity)
                        throw new Exception("Course capacity reached");
                }

                enrollment.Status = request.Status.Value;

                if (request.Status == EnrollmentStatus.Confirmed)
                    enrollment.ConfirmedAt = DateTimeOffset.UtcNow;
                else if (request.Status == EnrollmentStatus.Cancelled)
                    enrollment.CancelledAt = DateTimeOffset.UtcNow;
            }

            if (!string.IsNullOrWhiteSpace(request.CancelReason))
            {
                enrollment.CancelReason = request.CancelReason;
            }

            await _unitOfWork.GetRepository<Enrollment>().UpdateAsync(enrollment);
            await _unitOfWork.SaveAsync();

            return new EnrollmentResponse
            {
                Id = enrollment.Id,
                CourseId = enrollment.CourseId,
                StudentProfileId = enrollment.StudentProfileId,
                Status = enrollment.Status,
                ConfirmedAt = enrollment.ConfirmedAt,
                CancelledAt = enrollment.CancelledAt,
                CancelReason = enrollment.CancelReason
            };
        }

        // ✅ APPROVE
        public async Task<EnrollmentResponse> ApproveEnrollment(Guid enrollmentId, Guid approverProfileId)
        {
            var enrollment = await _unitOfWork.GetRepository<Enrollment>().Entities
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && !e.IsDeleted);

            if (enrollment == null)
                throw new Exception("Enrollment not found");

            var course = enrollment.Course;

            // ✅ Load student profile to validate grade level
            var student = await _unitOfWork.GetRepository<StudentProfile>().Entities
                .FirstOrDefaultAsync(s => s.Id == enrollment.StudentProfileId && !s.IsDeleted);

            if (student == null)
                throw new Exception("Student profile not found");

            // ✅ Validate grade level match before approving
            if (student.GradeLevel != course.GradeLevel)
                throw new Exception($"Student grade level ({student.GradeLevel}) does not match course grade level ({course.GradeLevel}). Only students with grade {course.GradeLevel} can enroll in this course.");

            // ✅ Validate that teacher is not teaching their own student
            if (course.TeacherProfileId.HasValue)
            {
                var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities
                    .FirstOrDefaultAsync(t => t.Id == course.TeacherProfileId.Value && !t.IsDeleted);

                if (teacher != null && IsTeacherTeachingOwnStudent(teacher, student))
                {
                    throw new Exception($"Cannot approve enrollment: The teacher teaches at the same school ({student.SchoolName}) and class ({student.ClassName}) as this student. Teachers cannot teach their own students.");
                }
            }

            // ✅ Kiểm tra quyền duyệt
            bool isAuthorized = (course.TeacherProfileId == approverProfileId) ||
                                (course.CenterProfileId == approverProfileId);

            if (!isAuthorized)
                throw new Exception("You do not have permission to approve this enrollment");

            // ✅ Ensure course capacity is not exceeded before confirming
            var confirmedCount = await _unitOfWork.GetRepository<Enrollment>().Entities
                .CountAsync(e => e.CourseId == course.Id && e.Status == EnrollmentStatus.Confirmed && !e.IsDeleted);

            if (confirmedCount >= course.Capacity)
                throw new Exception("Course capacity reached");

            // ✅ Cập nhật trạng thái
            enrollment.Status = EnrollmentStatus.Confirmed;
            enrollment.ConfirmedAt = DateTimeOffset.UtcNow;
            enrollment.CancelledAt = null;
            enrollment.CancelReason = null;

            await _unitOfWork.GetRepository<Enrollment>().UpdateAsync(enrollment);
            await _unitOfWork.SaveAsync();

            return new EnrollmentResponse
            {
                Id = enrollment.Id,
                CourseId = enrollment.CourseId,
                StudentProfileId = enrollment.StudentProfileId,
                Status = enrollment.Status,
                ConfirmedAt = enrollment.ConfirmedAt
            };
        }

        // ✅ REJECT
        public async Task<EnrollmentResponse> RejectEnrollment(Guid enrollmentId, Guid approverProfileId, string reason)
        {
            var enrollment = await _unitOfWork.GetRepository<Enrollment>().Entities
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && !e.IsDeleted);

            if (enrollment == null)
                throw new Exception("Enrollment not found");

            var course = enrollment.Course;

            // ✅ Kiểm tra quyền duyệt
            bool isAuthorized = (course.TeacherProfileId == approverProfileId) ||
                                (course.CenterProfileId == approverProfileId);

            if (!isAuthorized)
                throw new Exception("You do not have permission to reject this enrollment");

            // ✅ Cập nhật trạng thái
            enrollment.Status = EnrollmentStatus.Cancelled;
            enrollment.CancelledAt = DateTimeOffset.UtcNow;
            enrollment.CancelReason = reason;

            await _unitOfWork.GetRepository<Enrollment>().UpdateAsync(enrollment);
            await _unitOfWork.SaveAsync();

            return new EnrollmentResponse
            {
                Id = enrollment.Id,
                CourseId = enrollment.CourseId,
                StudentProfileId = enrollment.StudentProfileId,
                Status = enrollment.Status,
                CancelledAt = enrollment.CancelledAt,
                CancelReason = enrollment.CancelReason
            };
        }


        // ✅ DELETE
        public async Task<bool> DeleteEnrollment(Guid id)
        {
            var enrollment = await _unitOfWork.GetRepository<Enrollment>().Entities.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            if (enrollment == null)
                throw new Exception("Enrollment not found");

            enrollment.IsDeleted = true;

            await _unitOfWork.GetRepository<Enrollment>().UpdateAsync(enrollment);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }

}

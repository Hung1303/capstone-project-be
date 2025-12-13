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

        // ✅ CREATE: Tạo Enrollment ở trạng thái Pending (chờ thanh toán)
        public async Task<EnrollmentResponse> CreateEnrollment(CreateEnrollmentRequest request)
        {
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(c => c.Id == request.CourseId && !c.IsDeleted);
            if (course == null) throw new Exception("Không tìm thấy khóa học.");

            // Check capacity logic:
            // Lưu ý: Có thể bạn vẫn muốn check capacity ngay lúc đăng ký để tránh việc nhận tiền nhưng hết chỗ.
            var confirmedCount = await _unitOfWork.GetRepository<Enrollment>().Entities
                .CountAsync(e => e.CourseId == request.CourseId && e.Status == EnrollmentStatus.Confirmed && !e.IsDeleted);

            if (confirmedCount >= course.Capacity)
                throw new Exception("Đã đạt đủ số lượng học sinh tối đa.");

            var student = await _unitOfWork.GetRepository<StudentProfile>().Entities.FirstOrDefaultAsync(s => s.Id == request.StudentProfileId && !s.IsDeleted);
            if (student == null) throw new Exception("Không tìm thấy học sinh.");

            if (student.GradeLevel != course.GradeLevel)
                throw new Exception($"Học sinh lớp ({student.GradeLevel}) không được học khóa học cho lớp ({course.GradeLevel}).");

            if (course.TeacherProfileId.HasValue)
            {
                var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities
                    .FirstOrDefaultAsync(t => t.Id == course.TeacherProfileId.Value && !t.IsDeleted);

                if (teacher != null && IsTeacherTeachingOwnStudent(teacher, student))
                {
                    throw new Exception($"Không thể đăng kí. Giáo viên dạy cùng trường cùng lớp với học sinh.");
                }
            }

            // Kiểm tra xem đã đăng ký chưa
            var existingEnrollment = await _unitOfWork.GetRepository<Enrollment>().Entities
                .FirstOrDefaultAsync(e => e.CourseId == request.CourseId && e.StudentProfileId == request.StudentProfileId && !e.IsDeleted);
            if (existingEnrollment != null)
            {
                // Nếu đã có nhưng Cancelled thì cho tạo lại, còn Pending/Paid/Confirmed thì chặn
                if (existingEnrollment.Status != EnrollmentStatus.Cancelled)
                    throw new Exception("Học sinh đã đăng kí hoặc đang chờ thanh toán cho khóa học.");
            }

            var enrollment = new Enrollment
            {
                CourseId = request.CourseId,
                StudentProfileId = request.StudentProfileId,
                Status = EnrollmentStatus.Pending, // ✅ Bắt đầu là Pending (0)
            };

            await _unitOfWork.GetRepository<Enrollment>().InsertAsync(enrollment);
            await _unitOfWork.SaveAsync();

            return new EnrollmentResponse
            {
                Id = enrollment.Id,
                CourseId = enrollment.CourseId,
                StudentProfileId = enrollment.StudentProfileId,
                Status = enrollment.Status,
                // Chưa có ConfirmedAt
            };
        }

        // ✅ GET BY ID
        public async Task<EnrollmentResponse> GetEnrollmentById(Guid id)
        {
            var enrollment = await _unitOfWork.GetRepository<Enrollment>().Entities
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (enrollment == null)
                throw new Exception("Không tìm thấy đăng kí khóa học.");

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
                    StudentName = _unitOfWork.GetRepository<User>().Entities.Where(u => u.StudentProfile.Id == e.StudentProfileId).Select(u => u.FullName).FirstOrDefault(),
                    ParentName = _unitOfWork.GetRepository<User>().Entities.Where(u => u.ParentProfile.Id == e.StudentProfile.ParentProfileId).Select(u => u.FullName).FirstOrDefault(),
                    SchoolName = e.StudentProfile.SchoolName,
                    Gradelevel = e.StudentProfile.GradeLevel,
                    Subject = e.Course.Subject,
                    TeachingMethod = e.Course.TeachingMethod.ToString(),
                    Location = e.Course.Location,
                    Status = e.Status,
                    ConfirmedAt = e.ConfirmedAt,
                    CancelledAt = e.CancelledAt,
                    CancelReason = e.CancelReason
                })
                .ToListAsync();

            return pagedEnrollments;
        }

        public async Task<IEnumerable<EnrollmentResponse>> GetAllEnrollmentsByCenter(Guid centerProfileId, string? searchTerm, EnrollmentStatus? status, int pageNumber, int pageSize)
        {
            
            var enrollmentsQuery = _unitOfWork.GetRepository<Enrollment>().Entities
                .Include(c => c.Course)
                .ThenInclude(d => d.CenterProfile)
                .Where(e => !e.IsDeleted && e.Course.CenterProfile.Id == centerProfileId)
                .Include(e => e.StudentProfile)
                    .ThenInclude(s => s.ParentProfile)
                .Include(e => e.Course)
                .AsQueryable(); 

            
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

            if (status.HasValue)
            {
                enrollmentsQuery = enrollmentsQuery.Where(e => e.Status == status.Value);
            }
            
            var totalCount = await enrollmentsQuery.CountAsync();

            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;

            var pagedEnrollments = await enrollmentsQuery
                .OrderByDescending(e => e.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(e => new EnrollmentResponse
                {
                    Id = e.Id,
                    CourseId = e.CourseId,
                    StudentProfileId = e.StudentProfileId,
                    StudentName = _unitOfWork.GetRepository<User>().Entities.Where(u => u.StudentProfile.Id == e.StudentProfileId).Select(u => u.FullName).FirstOrDefault(),
                    ParentName = _unitOfWork.GetRepository<User>().Entities.Where(u => u.ParentProfile.Id == e.StudentProfile.ParentProfileId).Select(u => u.FullName).FirstOrDefault(),
                    SchoolName = e.StudentProfile.SchoolName,
                    Gradelevel = e.StudentProfile.GradeLevel,
                    Subject = e.Course.Subject,
                    TeachingMethod = e.Course.TeachingMethod.ToString(),
                    Location = e.Course.Location,
                    Status = e.Status,
                    ConfirmedAt = e.ConfirmedAt,
                    CancelledAt = e.CancelledAt,
                    CancelReason = e.CancelReason
                })
                .ToListAsync();

            return pagedEnrollments;
        }

        public async Task<IEnumerable<StudentEnrollmentResponse>> GetAllEnrollmentsByStudent(Guid studentProfileId, string? searchTerm, EnrollmentStatus? status, int pageNumber, int pageSize)
        {

            var enrollmentsQuery = _unitOfWork.GetRepository<Enrollment>().Entities
                .Include(c => c.Course)                
                .ThenInclude(d => d.CenterProfile)
                .Include(s => s.StudentProfile)
                .Where(e => !e.IsDeleted && e.StudentProfileId == studentProfileId)
                .AsQueryable();


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

            if (status.HasValue)
            {
                enrollmentsQuery = enrollmentsQuery.Where(e => e.Status == status.Value);
            }

            var totalCount = await enrollmentsQuery.CountAsync();

            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;

            var pagedEnrollments = await enrollmentsQuery
                .OrderByDescending(e => e.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(e => new StudentEnrollmentResponse
                {
                    Id = e.Id,
                    CourseId = e.CourseId,
                    StudentProfileId = e.StudentProfileId,
                    Subject = e.Course.Subject,
                    Location = e.Course.Location,
                    TeachingMethod = e.Course.TeachingMethod.ToString(),
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
                throw new Exception("Không tìm thấy đăng kí khóa học.");

            if (request.CourseId.HasValue)
            {
                var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(c => c.Id == request.CourseId.Value && !c.IsDeleted);
                if (course == null)
                    throw new Exception("Không tìm thấy khóa học.");

                // ✅ Validate grade level match if course is being changed
                var studentForValidation = await _unitOfWork.GetRepository<StudentProfile>().Entities
                    .FirstOrDefaultAsync(s => s.Id == enrollment.StudentProfileId && !s.IsDeleted);
                if (studentForValidation != null && studentForValidation.GradeLevel != course.GradeLevel)
                    throw new Exception($"Học sinh lớp ({studentForValidation.GradeLevel}) không được học khóa học cho lớp ({course.GradeLevel}). Chỉ học sinh lớp {course.GradeLevel} có thể đăng kí khóa học này.");

                // ✅ Validate that teacher is not teaching their own student
                if (course.TeacherProfileId.HasValue && studentForValidation != null)
                {
                    var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities
                        .FirstOrDefaultAsync(t => t.Id == course.TeacherProfileId.Value && !t.IsDeleted);

                    if (teacher != null && IsTeacherTeachingOwnStudent(teacher, studentForValidation))
                    {
                        throw new Exception($"Không thể cập nhật đăng kí: Giáo viên dạy cùng trường ({studentForValidation.SchoolName}) và cùng lớp ({studentForValidation.ClassName}) với học sinh. Giáo viên không thể dạy thêm ngoài nhà trường cho học sinh này.");
                    }
                }

                enrollment.CourseId = request.CourseId.Value;
            }

            if (request.StudentProfileId.HasValue)
            {
                var student = await _unitOfWork.GetRepository<StudentProfile>().Entities.FirstOrDefaultAsync(s => s.Id == request.StudentProfileId.Value && !s.IsDeleted);
                if (student == null)
                    throw new Exception("Không tìm thấy học sinh.");

                // ✅ Validate grade level match if student is being changed
                var courseForValidation = await _unitOfWork.GetRepository<Course>().Entities
                    .FirstOrDefaultAsync(c => c.Id == enrollment.CourseId && !c.IsDeleted);
                if (courseForValidation != null && student.GradeLevel != courseForValidation.GradeLevel)
                    throw new Exception($"Học sinh lớp ({student.GradeLevel}) không được học khóa học cho lớp ({courseForValidation.GradeLevel}). Chỉ học sinh lớp {courseForValidation.GradeLevel} có thể đăng kí khóa học này.");

                // ✅ Validate that teacher is not teaching their own student
                if (courseForValidation != null && courseForValidation.TeacherProfileId.HasValue)
                {
                    var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities
                        .FirstOrDefaultAsync(t => t.Id == courseForValidation.TeacherProfileId.Value && !t.IsDeleted);

                    if (teacher != null && IsTeacherTeachingOwnStudent(teacher, student))
                    {
                        throw new Exception($"Không thể cập nhật đăng kí: Giáo viên dạy cùng trường ({student.SchoolName}) và cùng lớp ({student.ClassName}) với học sinh. Giáo viên không thể dạy thêm ngoài nhà trường cho học sinh này.");
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
                        throw new Exception("Không tìm thấy khóa học.");

                    var confirmedCountForCourse = await _unitOfWork.GetRepository<Enrollment>().Entities
                        .CountAsync(e => e.CourseId == enrollment.CourseId && e.Status == EnrollmentStatus.Confirmed && !e.IsDeleted);

                    if (confirmedCountForCourse >= courseForCapacity.Capacity)
                        throw new Exception("Đã đạt số lượng học sinh tối đa.");
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

        // ✅ APPROVE: Chỉ được approve khi đã Paid (4) -> Chuyển sang Confirmed (1)
        public async Task<EnrollmentResponse> ApproveEnrollment(Guid enrollmentId, Guid approverProfileId)
        {
            var enrollment = await _unitOfWork.GetRepository<Enrollment>().Entities
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && !e.IsDeleted);

            if (enrollment == null) throw new Exception("Không tìm thấy đăng kí.");

            // ✅ Check Logic: Phải thanh toán rồi mới được duyệt
            if (enrollment.Status != EnrollmentStatus.Paid)
            {
                throw new Exception($"Không thể duyệt đăng kí. Trạng thái hiện tại là {enrollment.Status}. Đăng kí phải được thanh toán (PAID - status 4) trước khi duyệt.");
            }

            var course = enrollment.Course;

            // Validation Teacher/Student relationship (Double check)
            var student = await _unitOfWork.GetRepository<StudentProfile>().Entities
                .FirstOrDefaultAsync(s => s.Id == enrollment.StudentProfileId && !s.IsDeleted);

            if (student == null) throw new Exception("Không tìm thấy học sinh.");

            // (Giữ nguyên logic check Teacher dạy học sinh ruột tại đây...)
            if (course.TeacherProfileId.HasValue)
            {
                var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities
                    .FirstOrDefaultAsync(t => t.Id == course.TeacherProfileId.Value && !t.IsDeleted);
                if (teacher != null && IsTeacherTeachingOwnStudent(teacher, student))
                    throw new Exception("Mâu thuẫn: Giáo viên này dạy học sinh này ở trường.");
            }

            // Kiểm tra quyền duyệt
            bool isAuthorized = (course.TeacherProfileId == approverProfileId) ||
                                (course.CenterProfileId == approverProfileId);

            if (!isAuthorized)
                throw new Exception("Bạn không có quyền duyệt.");

            // ✅ Kiểm tra Capacity một lần nữa trước khi chốt đơn (đề phòng lúc payment thì còn slot nhưng giờ hết)
            var confirmedCount = await _unitOfWork.GetRepository<Enrollment>().Entities
                .CountAsync(e => e.CourseId == course.Id && e.Status == EnrollmentStatus.Confirmed && !e.IsDeleted);

            if (confirmedCount >= course.Capacity)
                throw new Exception("Khóa học đã đạt đủ số lượng. Không thể duyệt thêm.");

            // ✅ Cập nhật trạng thái sang Confirmed (1)
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

        // ✅ REJECT: Nếu từ chối sau khi đã thanh toán, có thể cần logic hoàn tiền (Ở đây chỉ xử lý status)
        public async Task<EnrollmentResponse> RejectEnrollment(Guid enrollmentId, Guid approverProfileId, string reason)
        {
            var enrollment = await _unitOfWork.GetRepository<Enrollment>().Entities
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && !e.IsDeleted);

            if (enrollment == null) throw new Exception("Không tìm thấy đăng kí.");

            var course = enrollment.Course;

            bool isAuthorized = (course.TeacherProfileId == approverProfileId) ||
                                (course.CenterProfileId == approverProfileId);

            if (!isAuthorized)
                throw new Exception("Bạn không có quyền duyệt.");

            // ✅ Update Status
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
                throw new Exception("Không tìm thấy đăng kí.");

            enrollment.IsDeleted = true;

            await _unitOfWork.GetRepository<Enrollment>().UpdateAsync(enrollment);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<IEnumerable<EnrollmentResponse>> GetAllEnrollmentsByCourse(Guid courseId, string? searchTerm, EnrollmentStatus? status, int pageNumber, int pageSize)
        {

            var enrollmentsQuery = _unitOfWork.GetRepository<Enrollment>().Entities
                .Include(c => c.Course)
                .ThenInclude(d => d.CenterProfile)
                .Where(e => !e.IsDeleted && e.CourseId == courseId)
                .Include(e => e.StudentProfile)
                    .ThenInclude(s => s.ParentProfile)
                .Include(e => e.Course)
                .AsQueryable();


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

            if (status.HasValue)
            {
                enrollmentsQuery = enrollmentsQuery.Where(e => e.Status == status.Value);
            }

            var totalCount = await enrollmentsQuery.CountAsync();

            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;

            var pagedEnrollments = await enrollmentsQuery
                .OrderByDescending(e => e.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(e => new EnrollmentResponse
                {
                    Id = e.Id,
                    CourseId = e.CourseId,
                    StudentProfileId = e.StudentProfileId,
                    StudentName = _unitOfWork.GetRepository<User>().Entities.Where(u => u.StudentProfile.Id == e.StudentProfileId).Select(u => u.FullName).FirstOrDefault(),
                    ParentName = _unitOfWork.GetRepository<User>().Entities.Where(u => u.ParentProfile.Id == e.StudentProfile.ParentProfileId).Select(u => u.FullName).FirstOrDefault(),
                    SchoolName = e.StudentProfile.SchoolName,
                    Gradelevel = e.StudentProfile.GradeLevel,
                    Subject = e.Course.Subject,
                    TeachingMethod = e.Course.TeachingMethod.ToString(),
                    Location = e.Course.Location,
                    Status = e.Status,
                    ConfirmedAt = e.ConfirmedAt,
                    CancelledAt = e.CancelledAt,
                    CancelReason = e.CancelReason
                })
                .ToListAsync();

            return pagedEnrollments;
        }
    }

}

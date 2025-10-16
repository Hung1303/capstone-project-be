using BusinessObjects;
using BusinessObjects.DTO.EnrollmentDTO;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
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

        // ✅ CREATE
        public async Task<EnrollmentResponse> CreateEnrollment(CreateEnrollmentRequest request)
        {
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(c => c.Id == request.CourseId && !c.IsDeleted);
            if (course == null)
                throw new Exception("Course not found");

            var student = await _unitOfWork.GetRepository<StudentProfile>().Entities.FirstOrDefaultAsync(s => s.Id == request.StudentProfileId && !s.IsDeleted);
            if (student == null)
                throw new Exception("Student profile not found");

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

                enrollment.CourseId = request.CourseId.Value;
            }

            if (request.StudentProfileId.HasValue)
            {
                var student = await _unitOfWork.GetRepository<StudentProfile>().Entities.FirstOrDefaultAsync(s => s.Id == request.StudentProfileId.Value && !s.IsDeleted);
                if (student == null)
                    throw new Exception("Student profile not found");

                enrollment.StudentProfileId = request.StudentProfileId.Value;
            }

            if (request.Status.HasValue)
            {
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

using Core.Base;
using Microsoft.AspNetCore.Mvc;
using Services.DTO.EnrollmentDTO;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // ✅ GET: api/Enrollments
        [HttpGet]
        public async Task<IActionResult> GetAllEnrollments(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            var enrollments = await _enrollmentService.GetAllEnrollments(searchTerm, pageNumber, pageSize);

            if (enrollments == null || !enrollments.Any())
                return NotFound(new { message = "Không tìm thấy khóa học." });

            return Ok(new
            {
                message = "Thành công lấy được danh sách khóa học.",
                data = enrollments,
                pagination = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = enrollments.Count(),
                    // Nếu bạn có totalCount từ service, có thể thêm TotalPages = ...
                }
            });
        }

        [HttpGet("Center/{centerProfileId}/Enrollments")]
        public async Task<IActionResult> GetAllEnrollmentsByCenter(Guid centerProfileId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            EnrollmentStatus? status = null)
        {
            var enrollments = await _enrollmentService.GetAllEnrollmentsByCenter(centerProfileId,searchTerm, status, pageNumber, pageSize);

            if (enrollments == null || !enrollments.Any())
                return NotFound(new { message = "Không tìm thấy khóa học." });

            return Ok(new
            {
                message = "Thành công lấy được danh sách đăng kí khóa học.",
                data = enrollments,
                pagination = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = enrollments.Count(),
                    // Nếu bạn có totalCount từ service, có thể thêm TotalPages = ...
                }
            });
        }

        [HttpGet("Course/{courseId}/Enrollments")]
        public async Task<IActionResult> GetAllEnrollmentsByCourse(Guid courseId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            EnrollmentStatus? status = null)
        {
            var enrollments = await _enrollmentService.GetAllEnrollmentsByCourse(courseId, searchTerm, status, pageNumber, pageSize);

            if (enrollments == null || !enrollments.Any())
                return NotFound(new { message = "Không tìm thấy đăng kí khóa học." });

            return Ok(new
            {
                message = "Thành công lấy được danh sách đăng kí khóa học.",
                data = enrollments,
                pagination = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = enrollments.Count(),
                    // Nếu bạn có totalCount từ service, có thể thêm TotalPages = ...
                }
            });
        }

        [HttpGet("Student/{studentProfileId}/Enrollments")]
        public async Task<IActionResult> GetAllEnrollmentsByStudent(Guid studentProfileId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            EnrollmentStatus? status = null)
        {
            var enrollments = await _enrollmentService.GetAllEnrollmentsByStudent(studentProfileId, searchTerm, status, pageNumber, pageSize);

            if (enrollments == null || !enrollments.Any())
                return NotFound(new { message = "Không tìm thấy khóa học." });

            return Ok(new
            {
                message = "Thành công lấy được danh sách đăng kí khóa học.",
                data = enrollments,
                pagination = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = enrollments.Count(),
                    // Nếu bạn có totalCount từ service, có thể thêm TotalPages = ...
                }
            });
        }

        // ✅ GET: api/Enrollments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollmentById(Guid id)
        {
            var enrollment = await _enrollmentService.GetEnrollmentById(id);
            if (enrollment == null)
                return NotFound(new { message = "Không tìm thấy khóa học." });

            return Ok(new
            {
                message = "Thành công lấy được thông tin đăng kí khóa học.",
                data = enrollment
            });
        }

        // ✅ POST: api/Enrollments
        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Lỗi "Duplicate email" xảy ra ở đây (trong service) và cần được catch
                var result = await _enrollmentService.CreateEnrollment(request);
                return Ok(new
                {
                    message = "Tạo đăng kí khóa học thành công.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                // Bắt các lỗi khác và trả về Internal Server Error (500) hoặc Bad Request tùy ngữ cảnh
                // Trong trường hợp này, ta trả về Bad Request cho các lỗi có thông báo rõ ràng
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ PUT: api/Enrollments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnrollment(Guid id, [FromBody] UpdateEnrollmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _enrollmentService.UpdateEnrollment(id, request);
                return Ok(new
                {
                    message = "Cập nhật đăng ký khóa học thành công.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                // Bắt các lỗi khác
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ NEW 1: APPROVE ENROLLMENT
        // POST: api/Enrollments/{id}/approve?approverProfileId={guid}
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveEnrollment(Guid id, [FromQuery] Guid approverProfileId)
        {
            try
            {
                var result = await _enrollmentService.ApproveEnrollment(id, approverProfileId);
                return Ok(new
                {
                    message = "Chấp thuận đăng kí khóa học thành công.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ NEW 2: REJECT ENROLLMENT
        // POST: api/Enrollments/{id}/reject?approverProfileId={guid}
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectEnrollment(Guid id, [FromQuery] Guid approverProfileId, [FromBody] RejectEnrollmentRequest request)
        {
            try
            {
                var result = await _enrollmentService.RejectEnrollment(id, approverProfileId, request.Reason);
                return Ok(new
                {
                    message = "Từ chối đăng kí khóa học thành công.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(Guid id)
        {
            var result = await _enrollmentService.DeleteEnrollment(id);

            if (!result)

                return NotFound(new { message = "Đăng kí khóa học không tìm thấy hoặc đã bị xóa." });

            return Ok(new { message = "Xóa đăng kí khóa học thành công." });
        }
    }
}


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
                return NotFound(new { message = "No enrollments found." });

            return Ok(new
            {
                message = "Successfully retrieved enrollments.",
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
                return NotFound(new { message = "No enrollments found." });

            return Ok(new
            {
                message = "Successfully retrieved enrollments.",
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
                return NotFound(new { message = "No enrollments found." });

            return Ok(new
            {
                message = "Successfully retrieved enrollments.",
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
                return NotFound(new { message = "Enrollment not found." });

            return Ok(new
            {
                message = "Successfully retrieved enrollment.",
                data = enrollment
            });
        }

        // ✅ POST: api/Enrollments
        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _enrollmentService.CreateEnrollment(request);
            return Ok(new
            {
                message = "Enrollment created successfully.",
                data = result
            });
        }

        // ✅ PUT: api/Enrollments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnrollment(Guid id, [FromBody] UpdateEnrollmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _enrollmentService.UpdateEnrollment(id, request);
            return Ok(new
            {
                message = "Enrollment updated successfully.",
                data = result
            });
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
                    message = "Enrollment approved successfully.",
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
                    message = "Enrollment rejected successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        // ✅ DELETE: api/Enrollments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(Guid id)
        {
            var result = await _enrollmentService.DeleteEnrollment(id);

            if (!result)
                return NotFound(new { message = "Enrollment not found or already deleted." });

            return Ok(new { message = "Enrollment deleted successfully." });
        }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTO.Suspension;
using Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuspensionsController : ControllerBase
    {
        private readonly ISuspensionService _suspensionService;

        public SuspensionsController(ISuspensionService suspensionService)
        {
            _suspensionService = suspensionService;
        }

        // GET: api/<SuspensionsController>
        [HttpGet("Users")]
        public async Task<IActionResult> GetAllUserSuspensionRecords([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? fullName = null)
        {
            var (records, totalCount) = await _suspensionService.GetSuspensionRecordsAsync(pageNumber, pageSize, fullName);
            return Ok(new { totalCount, records });
        }

        [HttpGet("Courses")]
        public async Task<IActionResult> GetAllCourseSuspensions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? searchKeyword = null)
        {
            var result = await _suspensionService.GetAllCourseSuspensionRecordsAsync(pageNumber, pageSize, searchKeyword);

            if (result == null || !result.Items.Any())
                return NotFound(new { message = "Không tìm thấy hồ sơ đình chỉ." });

            return Ok(new
            {
                message = "Lấy được hồ sơ đình chỉ khóa học thành công.",
                data = result.Items,
                pagination = new
                {
                    result.TotalCount,
                    result.PageNumber,
                    result.PageSize,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)result.PageSize)
                }
            });
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetSuspensionRecordById(Guid Id)
        {
            var result = await _suspensionService.GetRecordById(Id);
            return Ok(result);
        }

        //Ban user by Admin
        [HttpPost("User/{userId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> BanUser(Guid userId, Guid supervisorId, BanRequest record)
        {
            var result = await _suspensionService.BanUser(userId, supervisorId, record);
            return result ? Ok(new { message = "Người dùng đã bị đình chỉ." }) : BadRequest();
        }

        //Ban course by Admin
        [HttpPost("Course/{courseId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> BanCourse(Guid courseId, Guid supervisorId, BanRequest record)
        {
            var result = await _suspensionService.BanCourse(courseId, supervisorId, record);
            return result ? Ok(new { message = "Khóa học đã bị đình chỉ." }) : BadRequest();
        }


        //Update ban record
        [HttpPut("{suspensionRecordId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> UpdateSuspensionRecord(Guid suspensionRecordId, UpdateSuspensionRecordRequest request)
        {
            var result = await _suspensionService.UpdateSuspensionRecord(suspensionRecordId, request);
            if (result == null) return NotFound(new { message = "không tìm thấy hồ sơ." });

            return Ok(result);
        }

        
        //Unban
        [HttpDelete("{suspensionRecordId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> RemoveBan(Guid suspensionRecordId, Guid moderatorId)
        {
            var result = await _suspensionService.RemoveBan(suspensionRecordId, moderatorId);
            return result ? Ok(new { message = "Đình chỉ đã được gỡ." }) : NotFound();
        }
    }
}

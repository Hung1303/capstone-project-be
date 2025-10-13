using BusinessObjects.DTO.Suspension;
using Microsoft.AspNetCore.Mvc;
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
                return NotFound(new { message = "No suspension records found." });

            return Ok(new
            {
                message = "Successfully retrieved course suspension records.",
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

        // POST api/<SuspensionsController>
        [HttpPost("{userId}")]
        public async Task<IActionResult> BanUser(Guid userId, Guid supervisorId, BanRequest record)
        {
            var result = await _suspensionService.BanUser(userId, supervisorId, record);
            return result ? Ok(new { message = "User Banned!!!!" }) : BadRequest();
        }

        [HttpPost("{courseId}")]
        public async Task<IActionResult> BanCourse(Guid courseId, Guid supervisorId, BanRequest record)
        {
            var result = await _suspensionService.BanCourse(courseId, supervisorId, record);
            return result ? Ok(new { message = "Course Banned!!!!" }) : BadRequest();
        }

        // PUT api/<SuspensionsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SuspensionsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

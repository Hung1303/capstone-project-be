using Microsoft.AspNetCore.Mvc;
using Services.DTO.CourseResult;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseResultController : ControllerBase
    {
        private readonly ICourseResultService _courseResultService;
        public CourseResultController(ICourseResultService courseResultService)
        {
            _courseResultService = courseResultService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourseResult(CreateCourseResultRequest request)
        {
            try
            {
                var result = await _courseResultService.CreateCourseResult(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourseResult([FromQuery] string? searchTerm, [FromQuery] Guid? teacherId, [FromQuery] Guid? studentId, [FromQuery] Guid? courseId,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _courseResultService.GetAllCourseResult(searchTerm, pageNumber, pageSize, teacherId, courseId, studentId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseResultById(Guid id)
        {
            try
            {
                var result = await _courseResultService.GetCourseResultById(id);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourseResult(Guid id, UpdateCourseResultRequest request)
        {
            try
            {
                var result = await _courseResultService.UpdateCourseResult(id, request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourseResult(Guid id)
        {
            try
            {
                var result = await _courseResultService.DeleteCourseResult(id);
                return Ok(new { success = true, message = "Delete Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

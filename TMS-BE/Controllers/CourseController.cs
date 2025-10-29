using Microsoft.AspNetCore.Mvc;
using Services.DTO.Course;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse(CreateCourseRequest request)
        {
            try
            {
                var result = await _courseService.CreateCourse(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourse([FromQuery] string? searchTerm, [FromQuery] Guid? TeacherProfileId, [FromQuery] Guid? CenterProfileId,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _courseService.GetAllCourse(searchTerm, pageNumber, pageSize, TeacherProfileId, CenterProfileId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(Guid id)
        {
            try
            {
                var result = await _courseService.GetCourseById(id);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(Guid id, UpdateCourseRequest request)
        {
            try
            {
                var result = await _courseService.UpdateCourse(id, request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            try
            {
                var result = await _courseService.DeleteCourse(id);
                return Ok(new { success = true, message = "Delete Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("/Subject")]
        public async Task<IActionResult> CreateSubjectForCourse(CreateSubjectForCourseRequest request)
        {
            try
            {
                var result = await _courseService.CreateSubjectForCourse(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("/Subject")]
        public async Task<IActionResult> GetAllCourseSubject([FromQuery] string? searchTerm, Guid? CourseId, Guid? TeacherProfileId, string? status,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _courseService.GetAllCourseSubject(searchTerm, pageNumber, pageSize, CourseId, TeacherProfileId, status);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("/Subject/{id}")]
        public async Task<IActionResult> GetAllCourseSubjectById(Guid id)
        {
            try
            {
                var result = await _courseService.GetCourseSubjectById(id);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("/Subject/{id}")]
        public async Task<IActionResult> UpdateCourseSubject(Guid id, UpdateCourseSubject request)
        {
            try
            {
                var result = await _courseService.UpdateCourseSubject(id, request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpDelete("/Subject/{id}")]
        public async Task<IActionResult> DeleteCourseSubject(Guid id)
        {
            try
            {
                var result = await _courseService.DeleteCourseSubject(id);
                return Ok(new { success = true, message = "Delete Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

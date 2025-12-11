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

        [HttpPost("CenterCreate/")]
        public async Task<IActionResult> CenterCreateCourse(CenterCreateCourseRequest request)
        {
            try
            {
                var result = await _courseService.CenterCreateCourse(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("/{id}/AsignTeacher")]
        public async Task<IActionResult> AssignTeacher(Guid id, AssignTeacherRequest request)
        {
            try
            {
                var result = await _courseService.AssignTeacher(id, request);
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

        [HttpGet("{TeacherProfileId}/Courses")]
        public async Task<IActionResult> GetApprovedCoursesByTeacher(Guid TeacherProfileId, [FromQuery] string? searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _courseService.GetAllApprovedCoursesByTeacher(TeacherProfileId, searchTerm, pageNumber, pageSize);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{CenterProfileId}/PublishedCourses")]
        public async Task<IActionResult> GetPublishedCoursesByCenter(Guid CenterProfileId, [FromQuery] string? searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _courseService.GetAllPublishedCoursesByCenter(CenterProfileId, searchTerm, pageNumber, pageSize);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Published/Courses")]
        public async Task<IActionResult> GetAllPublishedCourse([FromQuery] string? searchTerm, [FromQuery] Guid? TeacherProfileId, [FromQuery] Guid? CenterProfileId,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _courseService.GetAllPublishedCourse(searchTerm, pageNumber, pageSize, TeacherProfileId, CenterProfileId);
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

        [HttpPut("Publish/{courseId}")]
        public async Task<IActionResult> CenterPublishCourse(Guid centerProfileId, Guid courseId)
        {
            try
            {
                var result = await _courseService.PublishCourseAsync(centerProfileId, courseId);
                return Ok(new { success = true, data = "Course published." });
            }catch(Exception e)
            {
                return BadRequest(e.Message);
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
        [HttpGet("/StudentSchedule")]
        public async Task<IActionResult> GetAllStudentSchedule([FromQuery] string? searchTerm, Guid StudentId, Guid CourseId,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _courseService.GetAllStudentSchedules(searchTerm, pageNumber, pageSize, StudentId, CourseId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("/StudentSchedule/{ParentId}")]
        public async Task<IActionResult> GetAllStudentScheduleByParentId([FromQuery] string? searchTerm, Guid ParentId,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _courseService.GetAllStudentSchedulesByParentsId(searchTerm, pageNumber, pageSize, ParentId);
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

        [HttpPut("Cancel/{courseId}")]
        public async Task<IActionResult> CancelCoures(Guid courseId, string reason)
        {
            try 
            {
                var result = await _courseService.CancelCourse(courseId, reason);
                return result ? Ok(new { success = true, message = "Hủy khóa học thành công." }) : BadRequest(new {success = false, message = "Hủy khóa học thất bại."});
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("TeacherWithdraw/{courseId}")]
        public async Task<IActionResult> TeacherWithdraw(Guid courseId, string reason)
        {
            try
            {
                var result = await _courseService.TeacherWithdrawFromCourse(courseId, reason);
                return result ? Ok(new { success = true, message = "Hủy khóa học thành công." }) : BadRequest(new { success = false, message = "Hủy khóa học thất bại." });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

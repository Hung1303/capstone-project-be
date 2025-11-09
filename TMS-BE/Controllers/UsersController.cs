using Core.Base;
using Microsoft.AspNetCore.Mvc;
using Services.DTO.User;
using Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("Users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? fullName = null)
        {
            var (users, totalCount) = await _userService.GetAllUsersAsync(pageNumber, pageSize, fullName);
            return Ok(new { totalCount, users });
        }

        [HttpGet("Centers")]
        public async Task<IActionResult> GetAllCenters([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? centerName = null)
        {
            var (centers, totalCount) = await _userService.GetAllCentersAsync(pageNumber, pageSize, centerName);
            return Ok(new { totalCount, centers });
        }

        [HttpGet("Teachers")]
        public async Task<IActionResult> GetAllTeachers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? fullName = null)
        {
            var (teachers, totalCount) = await _userService.GetAllTeachersAsync(pageNumber, pageSize, fullName);
            return Ok(new { totalCount, teachers });
        }

        [HttpGet("Parents")]
        public async Task<IActionResult> GetAllParents([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? fullName = null)
        {
            var (parents, totalCount) = await _userService.GetAllParentsAsync(pageNumber, pageSize, fullName);
            return Ok(new { totalCount, parents });
        }

        [HttpGet("Students")]
        public async Task<IActionResult> GetAllStudents([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? fullName = null)
        {
            var (students, totalCount) = await _userService.GetAllStudentsAsync(pageNumber, pageSize, fullName);
            return Ok(new { totalCount, students });
        }

        [HttpGet("User/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        [HttpGet("Center/{userId}")]
        public async Task<IActionResult> GetCenterById(Guid userId)
        {
            var center = await _userService.GetCenterById(userId);
            if (center == null)
                return NotFound(new { message = "Center not found" });

            return Ok(center);
        }

        [HttpGet("{centerId}/Teachers")]
        public async Task<IActionResult> GetTeachersByCenterId(Guid centerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? fullName = null)
        {
            var (teachers, totalCount) = await _userService.GetTeachersByCenterIdAsync(centerId, pageNumber, pageSize, fullName);
            return Ok(new { totalCount, teachers });
        }

        [HttpGet("Teacher/{userId}")]
        public async Task<IActionResult> GetTeacherById(Guid userId)
        {
            var center = await _userService.GetTeacherById(userId);
            if (center == null)
                return NotFound(new { message = "Teacher not found" });

            return Ok(center);
        }

        [HttpGet("Parent/{userId}")]
        public async Task<IActionResult> GetParentById(Guid userId)
        {
            var center = await _userService.GetParentById(userId);
            if (center == null)
                return NotFound(new { message = "Parent not found" });

            return Ok(center);
        }

        [HttpGet("Student/{userId}")]
        public async Task<IActionResult> GetStudentById(Guid userId)
        {
            var center = await _userService.GetStudentById(userId);
            if (center == null)
                return NotFound(new { message = "Student not found" });

            return Ok(center);
        }

        [HttpGet("{parentProfileId}/Students")]
        public async Task<IActionResult> GetStudentsByParentProfileId(Guid parentProfileId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? fullName = null)
        {
            var (students, totalCount) = await _userService.GetStudentsByParentIdAsync(parentProfileId, pageNumber, pageSize, fullName);
            return Ok(new { totalCount, students });
        }


        [HttpPost("Admin")]
        public async Task<IActionResult> CreateAdminAccount([FromBody] CreateAdminRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userService.CreateAdminRequest(request);
            return Ok(user);
        }

        [HttpPost("Inspector")]
        public async Task<IActionResult> CreateInspectorAccount([FromBody] CreateAdminRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userService.CreateInspectorRequest(request);
            return Ok(user);
        }

        [HttpPost("Center")]
        public async Task<IActionResult> CreateCenterAccount([FromBody] CreateCenterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userService.CreateCenterRequest(request);
            return Ok(user);
        }

        [HttpPost("Teacher")]
        public async Task<IActionResult> CreateTeacherAccount([FromBody] CreateTeacherRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userService.CreateTeacherRequest(request);
            return Ok(user);
        }

        [HttpPost("Center{centerOwnerId}/Teacher")]
        public async Task<IActionResult> CenterAddTeacher(Guid centerOwnerId, [FromBody] CreateTeacherRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userService.CenterAddTeacherRequest(centerOwnerId, request);
            return Ok(user);
        }

        [HttpPost("Parent")]
        public async Task<IActionResult> CreateParentAccount([FromBody] CreateParentRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userService.CreateParentRequest(request);
            return Ok(user);
        }

        [HttpPost("{parentId}/Student")]
        public async Task<IActionResult> CreateStudentAccount(Guid parentId, [FromBody] CreateStudentRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userService.CreateStudentRequest(parentId, request);
            return Ok(user);
        }


        [HttpPut("Center/{userId}")]
        public async Task<IActionResult> UpdateCenter(Guid userId, [FromBody] CenterUpdateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.UpdateCenterAsync(userId, request);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPut("Teacher/{userId}")]
        public async Task<IActionResult> UpdateTeacher(Guid userId, [FromBody] TeacherUpdateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.UpdateTeacherAsynce(userId, request);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPut("Parent/{userId}")]
        public async Task<IActionResult> UpdateParent(Guid userId, [FromBody] ParentUpdateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.UpdateParentAsynce(userId, request);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPut("{parentProfileId}/Student")]
        public async Task<IActionResult> UpdateStudent(Guid parentProfileId, [FromQuery] Guid studentId, [FromBody] StudentUpdateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.UpdateStudentAsync(parentProfileId ,studentId, request);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPut("Status/{userId}")]
        public async Task<IActionResult> UpdateUserStatus(Guid userId, int status)
        {
            var result = await _userService.UpdateUserStatus(userId, status);
            return result ? Ok(new { message = "User Staus Changed." }) : NotFound();
        }

        [HttpPut("ChangePassword/{userId}")]
        public async Task<IActionResult> ChangePassword(Guid userId, string currentPassword, string newPassword)
        {
            var result = await _userService.ChangePassword(userId, currentPassword, newPassword);
            return result ? Ok(new { message = "Password Changed." }) : BadRequest();
        }

        [HttpGet("Centers/Status/{status}")]
        public async Task<IActionResult> GetCentersByStatus(CenterStatus status, int pageNumber = 1, int pageSize = 5, string? centerName = null)
        {
            var (centers, totalCount) = await _userService.GetCentersByStatusAsync(status, pageNumber, pageSize, centerName);
            return Ok(new { centers, totalCount, pageNumber, pageSize });
        }

        [HttpPut("Centers/{centerId}/Status")]
        public async Task<IActionResult> UpdateCenterStatus(Guid centerId, [FromBody] UpdateCenterStatusRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _userService.UpdateCenterStatusAsync(centerId, request.Status, request.Reason);
            return result ? Ok(new { message = "Center status updated successfully" }) : NotFound();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var result = await _userService.DeleteUser(userId);
            return result ? Ok(new { message = "User deleted." }) : NotFound();
        }
    }
}

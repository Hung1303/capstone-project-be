using BusinessObjects.DTO.User;
using Microsoft.AspNetCore.Mvc;
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


        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? fullName = null)
        {
            var (users, totalCount) = await _userService.GetAllUsersAsync(pageNumber, pageSize, fullName);
            return Ok(new { totalCount, users });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        // POST api/<UsersController>
        [HttpPost("Admin")]
        public async Task<IActionResult> CreateAdminAccount([FromBody] CreateAdminRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userService.CreateAdminRequest(request);
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
        public async Task<IActionResult> CreateParentAccount(Guid parentId, [FromBody] CreateStudentRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userService.CreateStudentRequest(parentId, request);
            return Ok(user);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

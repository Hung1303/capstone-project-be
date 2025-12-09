using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO.Feedbacks;
using Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherFeedbacksController : ControllerBase
    {
        private readonly ITeacherFeedbackService _teacherFeedbackService;
        public TeacherFeedbacksController(ITeacherFeedbackService teacherFeedbackService)
        {
            _teacherFeedbackService = teacherFeedbackService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeacherFeedbacks([FromQuery] TeacherFeedbackQuery query)
        {
            var (feedbacks, totalCount) = await _teacherFeedbackService.GetAllTeacherFeedbacks(query);
            var response = new
            {
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize),
                Data = feedbacks
            };

            return Ok(response);
        }

        [HttpGet("teacher/{teacherProfileId}")]
        public async Task<IActionResult> GetAllFeedbacksByCourse(Guid teacherProfileId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] int? rating = null)
        {
            var (items, count) = await _teacherFeedbackService.GetAllFeedbackByTeacher(teacherProfileId, pageNumber, pageSize, rating);
            var response = new
            {
                TotalCount = count,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                Data = items
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeacherFeedbackById(Guid id)
        {
            var result = await _teacherFeedbackService.GetTeacherFeedbackById(id);
            if (result == null) return NotFound("Feedback not found.");
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "ParentStudent")]
        public async Task<IActionResult> CreateTeacherFeedback(Guid teacherProfileId, Guid reviewerId, [FromBody] CreateTeacherFeedbackRequest request)
        {
            var result = await _teacherFeedbackService.CreateTeacherFeedback(teacherProfileId, reviewerId, request);

            if (result == null) return NotFound("Teacher not found.");
            return Ok(result);
        }

        [HttpPut("Approve/{feedbackId}")]
        [Authorize(Policy = "InspectionAccess")]
        public async Task<IActionResult> ApproveTeacherFeedback(Guid feedbackId, Guid moderatorId, [FromBody] TeacherFeedbackModerationRequest request)
        {
            try
            {
                var result = await _teacherFeedbackService.ApproveTeacherFeedback(feedbackId, moderatorId, request);

                if (result == null) return NotFound("Feedback not found.");
                return Ok(result);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{feedbackId}")]
        [Authorize(Policy = "ParentStudent")]
        public async Task<IActionResult> UpdateTeacherFeedback(Guid feedbackId, [FromBody] UpdateTeacherFeedbackRequest request)
        {
            try
            {
                var result = await _teacherFeedbackService.UpdateTeacherFeedback(feedbackId, request);
                if (result == null) return NotFound("Feedback not found.");

                return Ok(result);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveTeacherFeedback(Guid id)
        {
            var result = await _teacherFeedbackService.RemoveTeacherFeedback(id);
            if (!result) return NotFound("Delete failed.");
            return result ? Ok(new { message = "Delete successfully." }) : BadRequest();
        }
    }
}

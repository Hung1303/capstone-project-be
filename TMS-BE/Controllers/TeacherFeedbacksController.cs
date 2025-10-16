using BusinessObjects.DTO.Feedbacks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeacherFeedbackById(Guid id)
        {
            var result = await _teacherFeedbackService.GetTeacherFeedbackById(id);
            if (result == null) return NotFound("Feedback not found.");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeacherFeedback(Guid teacherProfileId, Guid reviewerProfileId, [FromBody] CreateTeacherFeedbackRequest request)
        {
            var result = await _teacherFeedbackService.CreateTeacherFeedback(teacherProfileId, reviewerProfileId, request);

            if (result == null) return NotFound("Teacher not found.");
            return Ok(result);
        }

        [HttpPut("Approve/{feedbackId}")]
        public async Task<IActionResult> ApproveTeacherFeedback(Guid feedbackId, Guid moderatorId, [FromBody] TeacherFeedbackModerationRequest request)
        {
            var result = await _teacherFeedbackService.ApproveTeacherFeedback(feedbackId, moderatorId, request);

            if (result == null) return NotFound("Feedback not found.");
            return Ok(result);
        }

        [HttpPut("{feedbackId}")]
        public async Task<IActionResult> UpdateTeacherFeedback(Guid feedbackId, [FromBody] UpdateTeacherFeedbackRequest request)
        {
            var result = await _teacherFeedbackService.UpdateTeacherFeedback(feedbackId, request);
            if (result == null) return NotFound("Feedback not found.");

            return Ok(result);
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

using Microsoft.AspNetCore.Mvc;
using Services.DTO.Feedbacks;
using Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseFeedbacksController : ControllerBase
    {
        private readonly ICourseFeedbackService _courseFeedbackService;
        public CourseFeedbacksController(ICourseFeedbackService courseFeedbackService)
        {
            _courseFeedbackService = courseFeedbackService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoursesFeedbacks([FromQuery] CourseFeedbackQuery query)
        {
            var (feedbacks, totalCount) = await _courseFeedbackService.GetAllCoursesFeedbacks(query);
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
        public async Task<IActionResult> GetCourseFeedbackById(Guid id)
        {
            var result = await _courseFeedbackService.GetCourseFeedbackById(id);
            if (result == null) return NotFound("Feedback not found.");
            return Ok(result);
        }

        [HttpPost("{courseId}")]
        public async Task<IActionResult> CreateCourseFeedback(Guid courseId, Guid reviewerId, CreateCourseFeedbackRequest request)
        {
            var result = await _courseFeedbackService.CreateCourseFeedback(courseId, reviewerId, request);
            if (result == null) return NotFound("Course not found");

            return Ok(result);
        }

        [HttpPut("Approve/{feedbackId}")]
        public async Task<IActionResult> ApproveCourseFeedback(Guid feedbackId, Guid moderatorId, [FromBody] CourseFeedbackModerationRequest request)
        {
            var result = await _courseFeedbackService.ApproveCourseFeedback(feedbackId, moderatorId, request);

            if (result == null) return NotFound("Feedback not found.");
            return Ok(result);
        }

        [HttpPut("{feedbackId}")]
        public async Task<IActionResult> UpdateCourseFeedback(Guid feedbackId, [FromBody] UpdateCourseFeedbackRequest request)
        {
            var result = await _courseFeedbackService.UpdateCourseFeedback(feedbackId, request);
            if (result == null) return NotFound("Feedback not found.");

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveCourseFeedback(Guid id)
        {
            var result = await _courseFeedbackService.RemoveCourseFeedback(id);
            if (!result) return NotFound("Delete failed.");
            return result ? Ok(new { message = "Delete successfully." }) : BadRequest();
        }
    }
}

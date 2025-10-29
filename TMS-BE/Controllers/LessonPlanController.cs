using Microsoft.AspNetCore.Mvc;
using Services.DTO.LessonPlan;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonPlanController : ControllerBase
    {
        private readonly ILessonPlanService _lessonPlanService;
        public LessonPlanController(ILessonPlanService lessonPlanService)
        {
            _lessonPlanService = lessonPlanService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateLessonPlan(CreateLessonPlanRequest request)
        {
            try
            {
                var result = await _lessonPlanService.CreateLessonPlan(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllLessonPlan([FromQuery] string? searchTerm, [FromQuery] Guid? syllabusId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _lessonPlanService.GetAllLessonPlan(searchTerm, pageNumber, pageSize, syllabusId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLessonPlanById(Guid id)
        {
            try
            {
                var result = await _lessonPlanService.GetLessonPlanById(id);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLessonPlan(Guid id, UpdateLessonPlanRequest request)
        {
            try
            {
                var result = await _lessonPlanService.UpdateLessonPlan(id, request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLessonPlan(Guid id)
        {
            try
            {
                var result = await _lessonPlanService.DeleteLessonPlan(id);
                return Ok(new { success = true, message = "Delete Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

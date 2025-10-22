using BusinessObjects.DTO.ClassSchedule;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassScheduleController : ControllerBase
    {
        private readonly IClassScheduleService _classScheduleService;
        public ClassScheduleController(IClassScheduleService classScheduleService)
        {
            _classScheduleService = classScheduleService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateClassSchedule(CreateClassScheduleRequest request)
        {
            try
            {
                var result = await _classScheduleService.CreateClassSchedule(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllClassSchedule([FromQuery] Guid? subjectId, [FromQuery] DayOfWeek? dayOfWeek,
            [FromQuery] TimeOnly? startTime, [FromQuery] TimeOnly? endTime, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate, 
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _classScheduleService.GetAllClassSchedule(subjectId, dayOfWeek, startTime, endTime, startDate, endDate, pageNumber, pageSize);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClassScheduleById(Guid id)
        {
            try
            {
                var result = await _classScheduleService.GetClassScheduleById(id);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClassSchedule(Guid id, UpdateClassScheduleRequest request)
        {
            try
            {
                var result = await _classScheduleService.UpdateClassSchedule(id, request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClassSchedule(Guid id)
        {
            try
            {
                var result = await _classScheduleService.DeleteCLassSchedule(id);
                return Ok(new { success = true, message = "Delete Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

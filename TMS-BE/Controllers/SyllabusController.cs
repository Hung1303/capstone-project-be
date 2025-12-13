using Microsoft.AspNetCore.Mvc;
using Services.DTO.Syllabus;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyllabusController : ControllerBase
    {
        private readonly ISyllabusService _syllabusService;
        public SyllabusController(ISyllabusService syllabusService)
        {
            _syllabusService = syllabusService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSyllabus(CreateSyllabusRequest request)
        {
            try
            {
                var result = await _syllabusService.CreateSyllabus(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllSyllabus([FromQuery] string? searchTerm, [FromQuery] Guid? subjectId, [FromQuery] Guid? TeacherProfileId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _syllabusService.GetAllSyllabus(searchTerm, pageNumber, pageSize, subjectId, TeacherProfileId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSyllabusById(Guid id)
        {
            try
            {
                var result = await _syllabusService.GetSyllabusById(id);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{subjectId}/{centerProfileId}")]
        public async Task<IActionResult> GetSyllabusBySubjectOfCenter(Guid subjectId, Guid centerProfileId)
        {
            try
            {
                var result = await _syllabusService.GetSyllabusBySubjectOfCenter(subjectId, centerProfileId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSyllabus(Guid id, UpdateSyllabusRequest request)
        {
            try
            {
                var result = await _syllabusService.UpdateSyllabus(id, request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSyllabus(Guid id)
        {
            try
            {
                var result = await _syllabusService.DeleteSyllabus(id);
                return Ok(new { success = true, message = "Xóa thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

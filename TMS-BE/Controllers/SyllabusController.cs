using BusinessObjects.DTO.Course;
using BusinessObjects.DTO.Syllabus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetAllSyllabus([FromQuery] string? searchTerm, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var result = await _syllabusService.GetAllSyllabus(searchTerm, pageNumber, pageSize);
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
                return Ok(new { success = true, message = "Delete Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

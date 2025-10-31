using Microsoft.AspNetCore.Mvc;
using Services.DTO.TeacherVerification;
using Services.Interfaces;

namespace TMS_BE.Controllers
{
    [ApiController]
    [Route("api/teacher-verifications")]
    public class TeacherVerificationController : ControllerBase
    {
        private readonly ITeacherVerificationService _service;
        public TeacherVerificationController(ITeacherVerificationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Request([FromBody] TeacherVerificationRequestDto request)
        {
            var result = await _service.RequestVerification(request);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] Guid? teacherProfileId = null)
        {
            var result = await _service.GetAll(search, pageNumber, pageSize, teacherProfileId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            return Ok(result);
        }

        [HttpPut("{id}/documents")]
        public async Task<IActionResult> UpdateDocuments(Guid id, [FromBody] TeacherVerificationDocumentsDto request)
        {
            var result = await _service.UpdateDocuments(id, request);
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetTeacherVerificationStatusDto request)
        {
            var result = await _service.SetStatus(id, request);
            return Ok(result);
        }
    }
}



using Core.Base;
using Microsoft.AspNetCore.Authorization;
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

        //Center creates teacher verification request
        [HttpPost]
        [Authorize(Policy = "AdminOrCenterAccess")]
        public async Task<IActionResult> Request([FromBody] TeacherVerificationRequestDto request)
        {
            var result = await _service.RequestVerification(request);
            return Ok(result);
        }

        //GET ALL for admins, inspectors
        [HttpGet]
        [Authorize(Policy = "InspectionAccess")]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] Guid? teacherProfileId = null, [FromQuery] Guid? centerProfileId = null, [FromQuery] VerificationStatus? status = null)
        {
            var result = await _service.GetAll(search, pageNumber, pageSize, teacherProfileId, centerProfileId, status);
            return Ok(result);
        }

        [HttpGet("center/{centerProfileId}")]
        public async Task<IActionResult> GetByCenter(Guid centerProfileId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] Guid? teacherProfileId = null, [FromQuery] VerificationStatus? status = null)
        {
            var result = await _service.GetByCenter(centerProfileId, pageNumber, pageSize, teacherProfileId, status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            return Ok(result);
        }

        //Update the request with the required documents
        [HttpPut("{id}/documents")]
        [Authorize(Policy = "AdminOrCenterAccess")]
        public async Task<IActionResult> UpdateDocuments(Guid id, [FromBody] TeacherVerificationDocumentsDto request)
        {
            var result = await _service.UpdateDocuments(id, request);
            return Ok(result);
        }

        //Admin or Inspector approves the verification request
        [HttpPut("{id}/status")]
        [Authorize(Policy = "InspectionAccess")]
        public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetTeacherVerificationStatusDto request)
        {
            var result = await _service.SetStatus(id, request);
            return Ok(result);
        }
    }
}



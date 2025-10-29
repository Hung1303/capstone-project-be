using Core.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTO.GeneratedReport;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneratedReportsController : ControllerBase
    {
        private readonly IGeneratedReportService _service;

        public GeneratedReportsController(IGeneratedReportService service)
        {
            _service = service;
        }

        // POST: api/GeneratedReports
        [HttpPost]
        [Authorize(Policy = "InspectionAccess")]
        public async Task<IActionResult> Create([FromBody] CreateGeneratedReportRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Ok(new { success = true, data = result });
        }

        // GET: api/GeneratedReports
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] string? searchTerm, [FromQuery] ReportType? reportType, [FromQuery] Guid? requestedByUserId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(searchTerm, reportType, requestedByUserId, pageNumber, pageSize);
            return Ok(new { success = true, data = result });
        }

        // GET: api/GeneratedReports/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { success = false, message = "Not found" });
            return Ok(new { success = true, data = result });
        }

        // PUT: api/GeneratedReports/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "InspectionAccess")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGeneratedReportRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return Ok(new { success = true, data = result });
        }

        // DELETE: api/GeneratedReports/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "InspectionAccess")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound(new { success = false, message = "Not found" });
            return Ok(new { success = true });
        }

        // GET: api/GeneratedReports/{id}/download
        [HttpGet("{id}/download")]
        [Authorize]
        public async Task<IActionResult> Download(Guid id)
        {
            var payload = await _service.DownloadAsync(id);
            if (payload == null) return NotFound(new { success = false, message = "File not found" });
            return File(payload.Value.Content, payload.Value.ContentType, payload.Value.FileName);
        }
    }
}



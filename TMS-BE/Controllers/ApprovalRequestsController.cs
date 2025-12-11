using Core.Base;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalRequestsController : ControllerBase
    {
        private readonly IApprovalRequestService _approvalService;

        public ApprovalRequestsController(IApprovalRequestService approvalService)
        {
            _approvalService = approvalService;
        }

        // POST: api/ApprovalRequests/Course/{courseId}
        [HttpPost("Course/{courseId}")]
        public async Task<IActionResult> CreateApprovalRequest(Guid courseId, Guid requestedByUserId, [FromBody] string? notes = null)
        {
            var result = await _approvalService.CreateApprovalRequestAsync(courseId, requestedByUserId, notes);
            return result
                ? Ok(new { message = "Approval request created successfully. Course is now Pending Approval." })
                : BadRequest(new { message = "Failed to create approval request." });
        }

        // PUT: api/ApprovalRequests/{approvalRequestId}
        [HttpPut("{approvalRequestId}")]
        public async Task<IActionResult> ReviewApprovalRequest(Guid approvalRequestId, Guid reviewerUserId, [FromQuery] ApprovalDecision decision, [FromBody] string? notes = null)
        {
            var result = await _approvalService.ReviewApprovalRequestAsync(approvalRequestId, reviewerUserId, decision, notes);
            return result
                ? Ok(new { message = $"Approval request {decision.ToString()} successfully." })
                : BadRequest(new { message = "Failed to review approval request." });
        }

        // GET: api/ApprovalRequests
        [HttpGet]
        public async Task<IActionResult> GetAllApprovalRequests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? searchKeyword = null)
        {
            var (records, totalCount) = await _approvalService.GetApprovalRequestsAsync(pageNumber, pageSize, searchKeyword);
            return Ok(new { totalCount, records });
        }

        // GET: api/ApprovalRequests/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApprovalRequestById(Guid id)
        {
            var result = await _approvalService.GetApprovalRequestByIdAsync(id);
            return Ok(result);
        }
        // DELETE: api/ApprovalRequests/{approvalRequestId}
        [HttpDelete("{approvalRequestId}")]
        public async Task<IActionResult> DeleteApprovalRequest(Guid approvalRequestId)
        {
            var result = await _approvalService.DeleteApprovalRequestAsync(approvalRequestId);
            return result
                ? Ok(new { message = "Approval request deleted successfully." })
                : BadRequest(new { message = "Failed to delete approval request." });
        }
    }
}

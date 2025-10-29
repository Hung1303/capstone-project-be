using Services.DTO.CenterVerification;
using Core.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CenterVerificationController : ControllerBase
    {
        private readonly ICenterVerificationService _verificationService;

        public CenterVerificationController(ICenterVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        /// <summary>
        /// Create a new verification request for a center
        /// </summary>
        [HttpPost("request")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateVerificationRequest([FromBody] CreateVerificationRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _verificationService.CreateVerificationRequestAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update verification request details (for inspectors)
        /// </summary>
        [HttpPut("request/{verificationId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVerificationRequest(Guid verificationId, [FromBody] UpdateVerificationRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _verificationService.UpdateVerificationRequestAsync(verificationId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Make admin decision on verification request
        /// </summary>
        [HttpPut("request/{verificationId}/decision")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeAdminDecision(Guid verificationId, [FromBody] AdminDecisionDto decision)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _verificationService.MakeAdminDecisionAsync(verificationId, decision);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get verification request by ID
        /// </summary>
        [HttpGet("request/{verificationId}")]
        public async Task<IActionResult> GetVerificationRequest(Guid verificationId)
        {
            var result = await _verificationService.GetVerificationRequestByIdAsync(verificationId);
            if (result == null)
                return NotFound(new { message = "Verification request not found" });

            return Ok(result);
        }

        /// <summary>
        /// Get verification requests by inspector
        /// </summary>
        [HttpGet("inspector/{inspectorId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetVerificationRequestsByInspector(Guid inspectorId)
        {
            var result = await _verificationService.GetVerificationRequestsByInspectorAsync(inspectorId);
            return Ok(result);
        }

        /// <summary>
        /// Get all pending verification requests
        /// </summary>
        [HttpGet("pending")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingVerificationRequests()
        {
            var result = await _verificationService.GetPendingVerificationRequestsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get centers pending verification
        /// </summary>
        [HttpGet("centers/pending")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCentersPendingVerification()
        {
            var result = await _verificationService.GetCentersPendingVerificationAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get centers by status
        /// </summary>
        [HttpGet("centers/status/{status}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCentersByStatus(CenterStatus status)
        {
            var result = await _verificationService.GetCentersByStatusAsync(status);
            return Ok(result);
        }

        /// <summary>
        /// Complete verification (mark as completed by inspector)
        /// </summary>
        [HttpPut("request/{verificationId}/complete")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CompleteVerification(Guid verificationId)
        {
            try
            {
                var result = await _verificationService.CompleteVerificationAsync(verificationId);
                if (result)
                    return Ok(new { message = "Verification completed successfully" });
                else
                    return BadRequest(new { message = "Failed to complete verification" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Suspend a center
        /// </summary>
        [HttpPut("centers/{centerId}/suspend")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> SuspendCenter(Guid centerId, [FromBody] SuspendCenterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _verificationService.SuspendCenterAsync(centerId, request.Reason, request.AdminId);
                if (result)
                    return Ok(new { message = "Center suspended successfully" });
                else
                    return BadRequest(new { message = "Failed to suspend center" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Restore a suspended center
        /// </summary>
        [HttpPut("centers/{centerId}/restore")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreCenter(Guid centerId, [FromBody] RestoreCenterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _verificationService.RestoreCenterAsync(centerId, request.Reason, request.AdminId);
                if (result)
                    return Ok(new { message = "Center restored successfully" });
                else
                    return BadRequest(new { message = "Failed to restore center" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

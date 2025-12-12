using Core.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTO.CenterVerification;
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
        [Authorize(Policy = "Admin")]
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
        [Authorize(Policy = "InspectionAccess")]
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
        [Authorize(Policy = "Admin")]
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
                return NotFound(new { message = "Yêu cầu duyệt không tìm thấy." });

            return Ok(result);
        }

        /// <summary>
        /// Get verification requests by inspector
        /// </summary>
        [HttpGet("inspector/{inspectorId}")]
        [Authorize(Policy = "InspectionAccess")]
        public async Task<IActionResult> GetVerificationRequestsByInspector(Guid inspectorId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _verificationService.GetVerificationRequestsByInspectorAsync(inspectorId, pageNumber, pageSize);
                return Ok(result);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get all pending verification requests
        /// </summary>
        [HttpGet("pending")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingVerificationRequests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _verificationService.GetPendingVerificationRequestsAsync(pageNumber, pageSize);
                return Ok(result);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get centers pending verification
        /// </summary>
        [HttpGet("centers/pending")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetCentersPendingVerification([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _verificationService.GetCentersPendingVerificationAsync(pageNumber, pageSize);
                return Ok(result);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get centers by status
        /// </summary>
        [HttpGet("centers/status/{status}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetCentersByStatus(CenterStatus status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _verificationService.GetCentersByStatusAsync(status, pageNumber, pageSize);
                return Ok(result);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Complete verification (mark as completed by inspector)
        /// </summary>
        [HttpPut("request/{verificationId}/complete")]
        [Authorize(Policy = "Inspector")]
        public async Task<IActionResult> CompleteVerification(Guid verificationId)
        {
                var result = await _verificationService.CompleteVerificationAsync(verificationId);
                if (result)
                    return Ok(new { message = "Hoàn tất xác minh thành công." });
                else
                    return BadRequest(new { message = "Thất bại trong việc xác minh." });
            
        }

        /// <summary>
        /// Suspend a center
        /// </summary>
        [HttpPut("centers/{centerId}/suspend")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> SuspendCenter(Guid centerId, [FromBody] SuspendCenterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

                var result = await _verificationService.SuspendCenterAsync(centerId, request.Reason, request.AdminId);
                if (result)
                    return Ok(new { message = "Trung tâm đình chỉ thành công." });
                else
                    return BadRequest(new { message = "Đình chỉ trung tâm thất bại." });
            
        }

        /// <summary>
        /// Restore a suspended center
        /// </summary>
        [HttpPut("centers/{centerId}/restore")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> RestoreCenter(Guid centerId, [FromBody] RestoreCenterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _verificationService.RestoreCenterAsync(centerId, request.Reason, request.AdminId);
            if (result)
                return Ok(new { message = "Khôi phục trung tâm thành công." });
            else
                return BadRequest(new { message = "Khôi phục trung tâm thất bại." });
            
        }
    }
}

using Core.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTO.Subscription;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        // Subscription Package Management (Admin only)
        [HttpPost("packages")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> CreateSubscriptionPackage(CreateSubscriptionPackageRequest request)
        {
            try
            {
                var result = await _subscriptionService.CreateSubscriptionPackageAsync(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("packages")]
        public async Task<IActionResult> GetAllSubscriptionPackages([FromQuery] bool? activeOnly = null)
        {
            try
            {
                var result = await _subscriptionService.GetAllSubscriptionPackagesAsync(activeOnly);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("packages/{id}")]
        public async Task<IActionResult> GetSubscriptionPackageById(Guid id)
        {
            try
            {
                var result = await _subscriptionService.GetSubscriptionPackageByIdAsync(id);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("packages/{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> UpdateSubscriptionPackage(Guid id, UpdateSubscriptionPackageRequest request)
        {
            try
            {
                var result = await _subscriptionService.UpdateSubscriptionPackageAsync(id, request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("packages/{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteSubscriptionPackage(Guid id)
        {
            try
            {
                var result = await _subscriptionService.DeleteSubscriptionPackageAsync(id);
                return Ok(new { success = true, message = "Subscription package deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // Center Subscription Management
        [HttpPost("subscribe")]
        [Authorize(Policy = "Center")]
        public async Task<IActionResult> SubscribeCenter(SubscribeCenterRequest request)
        {
            try
            {
                var result = await _subscriptionService.SubscribeCenterAsync(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("upgrade")]
        [Authorize(Policy = "Center")]
        public async Task<IActionResult> UpgradeSubscription(UpgradeSubscriptionRequest request)
        {
            try
            {
                var result = await _subscriptionService.UpgradeSubscriptionAsync(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("cancel")]
        [Authorize(Policy = "Center")]
        public async Task<IActionResult> CancelSubscription(CancelSubscriptionRequest request)
        {
            try
            {
                var result = await _subscriptionService.CancelSubscriptionAsync(request);
                return Ok(new { success = true, message = "Subscription cancelled successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("center/{centerProfileId}/active")]
        public async Task<IActionResult> GetActiveSubscription(Guid centerProfileId)
        {
            try
            {
                var result = await _subscriptionService.GetActiveSubscriptionAsync(centerProfileId);
                if (result == null)
                {
                    return Ok(new { success = true, data = (object?)null, message = "No active subscription found" });
                }
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("center/{centerProfileId}")]
        public async Task<IActionResult> GetCenterSubscriptions(Guid centerProfileId)
        {
            try
            {
                var result = await _subscriptionService.GetCenterSubscriptionsAsync(centerProfileId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("center/{centerProfileId}/limit")]
        public async Task<IActionResult> GetSubscriptionLimit(Guid centerProfileId)
        {
            try
            {
                var canPost = await _subscriptionService.CanCenterPostCourseAsync(centerProfileId);
                var remaining = await _subscriptionService.GetRemainingCoursePostsAsync(centerProfileId);
                var max = await _subscriptionService.GetMaxCoursePostsAsync(centerProfileId);

                return Ok(new { success = true, data = new { canPost, remaining, max } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // Admin: Get all subscriptions of centers
        [HttpGet("all")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAllCenterSubscriptions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] SubscriptionStatus? status = null)
        {
            try
            {
                var result = await _subscriptionService.GetAllCenterSubscriptionsAsync(pageNumber, pageSize, status);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}


using BusinessObjects;
using Core.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.DTO.Email;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService, IUnitOfWork unitOfWork, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Send email to centers (Admin only)
        /// </summary>
        [HttpPost("send-to-centers")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> SendEmailToCenters([FromBody] SendEmailToCentersRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Yêu cầu không hợp lệ.", errors = ModelState });
            }

            try
            {
                List<string> centerEmails;

                if (request.CenterIds != null && request.CenterIds.Any())
                {
                    // Send to specific centers
                    var centers = await _unitOfWork.GetRepository<CenterProfile>()
                        .Entities
                        .Where(c => request.CenterIds.Contains(c.Id) && !c.IsDeleted)
                        .Join(
                            _unitOfWork.GetRepository<User>().Entities.Where(u => !u.IsDeleted),
                            center => center.UserId,
                            user => user.Id,
                            (center, user) => new
                            {
                                ContactEmail = center.ContactEmail,
                                UserEmail = user.Email
                            }
                        )
                        .ToListAsync();

                    centerEmails = centers
                        .Select(c => !string.IsNullOrWhiteSpace(c.ContactEmail) ? c.ContactEmail : c.UserEmail)
                        .Where(email => !string.IsNullOrWhiteSpace(email))
                        .Distinct()
                        .ToList();
                }
                else
                {
                    // Send to all active centers
                    var centers = await _unitOfWork.GetRepository<CenterProfile>()
                        .Entities
                        .Where(c => !c.IsDeleted)
                        .Join(
                            _unitOfWork.GetRepository<User>().Entities.Where(u => !u.IsDeleted),
                            center => center.UserId,
                            user => user.Id,
                            (center, user) => new
                            {
                                ContactEmail = center.ContactEmail,
                                UserEmail = user.Email
                            }
                        )
                        .ToListAsync();

                    centerEmails = centers
                        .Select(c => !string.IsNullOrWhiteSpace(c.ContactEmail) ? c.ContactEmail : c.UserEmail)
                        .Where(email => !string.IsNullOrWhiteSpace(email))
                        .Distinct()
                        .ToList();
                }

                if (!centerEmails.Any())
                {
                    return BadRequest(new { success = false, message = "Không tìm thấy email trung tâm hợp lệ." });
                }

                var result = await _emailService.SendEmailToCentersAsync(centerEmails, request.Subject, request.Body);

                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = $"Email đã gửi đến {centerEmails.Count} trung tâm.",
                        recipientsCount = centerEmails.Count
                    });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Gửi mail thất bại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi mail đến trung tâm.");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}


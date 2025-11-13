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
                return BadRequest(new { success = false, message = "Invalid request", errors = ModelState });
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
                    return BadRequest(new { success = false, message = "No valid center emails found" });
                }

                var result = await _emailService.SendEmailToCentersAsync(centerEmails, request.Subject, request.Body);

                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = $"Email sent to {centerEmails.Count} center(s)",
                        recipientsCount = centerEmails.Count
                    });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Failed to send emails" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending emails to centers");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}


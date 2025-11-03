using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO.LessonPlan;
using Services.DTO.Payment;
using Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public PaymentController(IPaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }

        [HttpGet("VNPAY")]
        public async Task<IActionResult> CreateVNPAYUrl([FromQuery] Guid paymentId)
        {
            try
            {
                var IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var result = await _paymentService.CreateVNPAYUrl(paymentId, IpAddress);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        

        [HttpPost]
        public async Task<IActionResult> CreatePayment(CreatePaymentRequest request)
        {
            try
            {
                var result = await _paymentService.CreatePayment(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments([FromQuery] Guid? UserId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _paymentService.GetAllPayments( pageNumber, pageSize, UserId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            try
            {
                var result = await _paymentService.GetPaymentById(id);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(Guid id)
        {
            try
            {
                var result = await _paymentService.DeletePayment(id);
                return Ok(new { success = true, message = "Delete Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("Callback")]
        public async Task<IActionResult> Callback()
        {
            try
            {
                var query = Request.Query;
                var hashSecret = _configuration["Vnpay:HashSecret"];
                if (!query.TryGetValue("vnp_SecureHash", out var receivedHashValues) ||
                    string.IsNullOrEmpty(receivedHashValues[0]))
                {
                    return Ok(new { success = false, message = "Missing vnp_SecureHash" });
                }
                string receivedHash = receivedHashValues[0];

                var hashData = new StringBuilder();
                var sortedKeys = query.Keys
                    .Where(k => !string.IsNullOrEmpty(k) &&
                                !k.Equals("vnp_SecureHash", StringComparison.Ordinal))
                    .OrderBy(k => k, StringComparer.Ordinal)
                    .ToList();

                foreach (var key in sortedKeys)
                {
                    var rawValue = query[key].ToString(); 
                    var encodedValue = Uri.EscapeDataString(rawValue);
                    if (hashData.Length > 0) hashData.Append('&');
                    hashData.Append($"{key}={encodedValue}");
                }

                string calculatedHash;
                using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hashSecret)))
                {
                    var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashData.ToString()));
                    calculatedHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }

                if (!string.Equals(calculatedHash, receivedHash, StringComparison.Ordinal))
                {
                    return Ok(new { success = false, message = "Invalid signature" });
                }

                var responseCode = query["vnp_ResponseCode"].ToString();
                var txnStatus = query["vnp_TransactionStatus"].ToString();

                if (responseCode == "00" && txnStatus == "00")
                {
                    var txnRef = query["vnp_TxnRef"].ToString();
                    if (!Guid.TryParse(txnRef, out _))
                    {
                        return Ok(new { success = false, message = "Invalid TxnRef" });
                    }

                    var result = await _paymentService.UpdatePayment(Guid.Parse(txnRef));
                    return Ok(new { success = true, data = result });
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Payment failed",
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

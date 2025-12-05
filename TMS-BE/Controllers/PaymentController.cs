using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO.LessonPlan;
using Services.DTO.Payment;
using Services.Interfaces;
using System.Net;
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
                var ipAddress = HttpContext.Connection.RemoteIpAddress;
                if (ipAddress != null)
                {
                    if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        ipAddress = ipAddress.MapToIPv4();
                    }
                }

                var ipAddressString = ipAddress?.ToString();
                if (string.IsNullOrEmpty(ipAddressString) || ipAddressString == "0.0.0.1" || ipAddressString == "::1")
                {
                    ipAddressString = "127.0.0.1";
                }
                var result = await _paymentService.CreateVNPAYUrl(paymentId, ipAddressString);
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
            var query = Request.Query;
            var hashSecret = _configuration["Vnpay:HashSecret"];
            var paymentSuccessUrl = _configuration["Vnpay:PaymentSuccessUrl"];
            var paymentFailureUrl = _configuration["Vnpay:PaymentFailureUrl"];
            try
            {

                if (!query.TryGetValue("vnp_SecureHash", out var receivedHashValues))
                {
                    return Redirect(paymentFailureUrl);
                }
                string receivedHash = receivedHashValues.ToString();

                var sortedParams = query.Keys
                    .Where(k => !string.IsNullOrEmpty(k) &&
                                !k.StartsWith("vnp_SecureHash")) 
                    .OrderBy(k => k, StringComparer.InvariantCultureIgnoreCase) 
                    .ToList();

                var hashData = new StringBuilder();
                foreach (var key in sortedParams)
                {
                    string value = query[key];

                    if (!string.IsNullOrEmpty(value))
                    {
                        if (hashData.Length > 0) hashData.Append('&');
                        hashData.Append($"{key}={WebUtility.UrlEncode(value)}");
                    }
                }

                string calculatedHash;
                using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hashSecret)))
                {
                    var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashData.ToString()));
                    calculatedHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }

                if (!calculatedHash.Equals(receivedHash, StringComparison.InvariantCultureIgnoreCase))
                {
                    return Redirect(paymentFailureUrl);
                }

                var responseCode = query["vnp_ResponseCode"].ToString();
                var txnStatus = query["vnp_TransactionStatus"].ToString();
                var txnRef = query["vnp_TxnRef"].ToString();
                var vnpAmount = Convert.ToInt64(query["vnp_Amount"]) / 100; // VNPAY amount is *100

                if (responseCode == "00" && txnStatus == "00")
                {
                    if (!Guid.TryParse(txnRef, out Guid paymentId))
                    {
                        return Redirect(paymentFailureUrl);
                    }

                    var result = await _paymentService.UpdatePayment(paymentId);

                    return Redirect($"{paymentSuccessUrl}?paymentId={paymentId}");
                }
                else
                {
                    return Redirect($"{paymentFailureUrl}");
                }
            }
            catch (Exception ex)
            {
                return Redirect($"{paymentFailureUrl}");
            }

        }
    }
}

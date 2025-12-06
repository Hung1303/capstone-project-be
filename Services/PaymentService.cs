using Azure.Core;
using BusinessObjects;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;
using Services.DTO.LessonPlan;
using Services.DTO.Payment;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<PaymentResponse> CreatePayment(CreatePaymentRequest request)
        {
            var user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(a => a.Id == request.UserId && !a.IsDeleted);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            if (request.CenterSubscriptionId.HasValue)
            {
                var centersub = await _unitOfWork.GetRepository<CenterSubscription>().Entities.FirstOrDefaultAsync(a => a.Id == request.CenterSubscriptionId && !a.IsDeleted);
                if (centersub == null)
                {
                    throw new Exception("Center Not Found");
                }
            }
            if (request.EnrollmentId.HasValue)
            {
                var enrollment = await _unitOfWork.GetRepository<Enrollment>().Entities.FirstOrDefaultAsync(a => a.Id == request.EnrollmentId && !a.IsDeleted);
                if (enrollment == null)
                {
                    throw new Exception("Enrollment Not Found");
                }
            }
            if (request.CenterSubscriptionId.HasValue && request.EnrollmentId.HasValue)
            {
                throw new Exception("Only CenterSubscriptionId or EnrollmentId allowed not both");
            }
            var payment = new Payment
            {
                Amount = request.Amount,
                Description = request.Description,
                status = "PENDING",
                CenterSubscriptionId = request.CenterSubscriptionId,
                EnrollmentId = request.EnrollmentId,
                UserId = request.UserId,
            };
            await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
            await _unitOfWork.SaveAsync();
            var result = new PaymentResponse
            {
                Id = payment.Id,
                Amount = payment.Amount,
                Description = payment.Description,
                status = payment.status,
                PaymentDate = payment.PaymentDate,
                UserId = payment.UserId,
                CenterSubscriptionId = payment.CenterSubscriptionId,
                EnrollmentId = payment.EnrollmentId,
            };
            return result;
        }

        public async Task<string> CreateVNPAYUrl(Guid paymentId, string IpAddress)
        {
            var payment = await _unitOfWork.GetRepository<Payment>().Entities.FirstOrDefaultAsync(a => a.Id == paymentId && !a.IsDeleted);
            if (payment == null)
            {
                throw new Exception("Payment Not Found");
            }
            if (payment.Amount < 10000 || payment.Amount > 1000000000)
            {
                throw new Exception("ammount must be from 10.000 to 1.000.000.000.");
            }

            if (string.IsNullOrEmpty(payment.Description))
            {
                throw new Exception("description must not be null");
            }

            if (string.IsNullOrEmpty(IpAddress))
            {
                throw new Exception("IpAddress not found.");
            }

            var vnp_Params = new SortedList<string, string>(new VnpayCompare());

            vnp_Params.Add("vnp_Version", "2.1.0");
            vnp_Params.Add("vnp_Command", "pay");
            vnp_Params.Add("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            vnp_Params.Add("vnp_Amount", (payment.Amount * 100).ToString());
            vnp_Params.Add("vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss"));
            vnp_Params.Add("vnp_CurrCode", "VND");
            vnp_Params.Add("vnp_IpAddr", IpAddress); // Ensure this is IPv4 from Controller
            vnp_Params.Add("vnp_Locale", "vn");
            vnp_Params.Add("vnp_OrderInfo", payment.Description);
            vnp_Params.Add("vnp_OrderType", "other");
            vnp_Params.Add("vnp_ReturnUrl", _configuration["Vnpay:ReturnUrl"]);
            vnp_Params.Add("vnp_ExpireDate", DateTime.UtcNow.AddHours(7).AddMinutes(15).ToString("yyyyMMddHHmmss"));
            vnp_Params.Add("vnp_TxnRef", payment.Id.ToString());

            var data = new StringBuilder();

            foreach (KeyValuePair<string, string> kv in vnp_Params)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string queryString = data.ToString();
            if (queryString.Length > 0)
            {
                queryString = queryString.Remove(queryString.Length - 1, 1);
            }

            string vnp_SecureHash = HmacSHA512(_configuration["Vnpay:HashSecret"], queryString);

            string baseUrl = _configuration["Vnpay:BaseUrl"];
            return $"{baseUrl}?{queryString}&vnp_SecureHash={vnp_SecureHash}";
        }

        public class VnpayCompare : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                var vnpCompare = CompareInfo.GetCompareInfo("en-US");
                return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
            }
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }

        public async Task<bool> DeletePayment(Guid id)
        {
            var payment = await _unitOfWork.GetRepository<Payment>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (payment == null)
            {
                throw new Exception("Payment Not Found");
            }
            payment.IsDeleted = true;
            await _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<PaymentResponse>> GetAllPayments(int pageNumber, int pageSize, Guid? userId ,Guid? CenterSubscriptionId, Guid? EnrollmentId, string? status)
        {
            var payment = _unitOfWork.GetRepository<Payment>().Entities.Where(a => !a.IsDeleted);
            if (userId.HasValue)
            {
                payment = payment.Where(a => a.UserId == userId);
            }
            if (CenterSubscriptionId.HasValue)
            {
                payment = payment.Where(a => a.CenterSubscriptionId == CenterSubscriptionId);
            }
            if (CenterSubscriptionId.HasValue)
            {
                payment = payment.Where(a => a.EnrollmentId == EnrollmentId);
            }
            if (status != null)
            {
                payment = payment.Where(a => a.status == status);
            }
            var totalCount = await payment.CountAsync();
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;
            var paginatedPayments = await payment
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(a => new PaymentResponse
                {
                    Id = a.Id,
                    Amount = a.Amount,
                    Description = a.Description,
                    status = a.status,
                    PaymentDate = a.PaymentDate,
                    UserId = a.UserId,
                    CenterSubscriptionId = a.CenterSubscriptionId,
                    EnrollmentId = a.EnrollmentId,
                }).ToListAsync();
            return paginatedPayments;
        }

        public async Task<PaymentResponse> GetPaymentById(Guid id)
        {
            var payment = await _unitOfWork.GetRepository<Payment>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (payment == null)
            {
                throw new Exception("Payment Not Found");
            }
            var result = new PaymentResponse
            {
                Id = payment.Id,
                Amount = payment.Amount,
                Description = payment.Description,
                status = payment.status,
                PaymentDate = payment.PaymentDate,
                UserId = payment.UserId,
                CenterSubscriptionId = payment.CenterSubscriptionId,
                EnrollmentId = payment.EnrollmentId,
            };
            return result;
        }

        public async Task<PaymentResponse> UpdatePayment(Guid id)
        {
            var payment = await _unitOfWork.GetRepository<Payment>().Entities
                .Include(c => c.CenterSubscription)
                .Include(c => c.Enrollment)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (payment == null)
            {
                throw new Exception("Payment Not Found");
            }

            payment.status = "DONE";
            payment.PaymentDate = DateTime.UtcNow;

            if (payment.Enrollment != null && payment.CenterSubscription == null)
            {
                payment.Enrollment.Status = EnrollmentStatus.Paid;
                await _unitOfWork.GetRepository<Enrollment>().UpdateAsync(payment.Enrollment);
            }
            if (payment.CenterSubscription != null && payment.Enrollment == null)
            {
                payment.CenterSubscription.Status = SubscriptionStatus.Paid;
                await _unitOfWork.GetRepository<CenterSubscription>().UpdateAsync(payment.CenterSubscription);
            }

            await _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);
            await _unitOfWork.SaveAsync();

            var result = new PaymentResponse
            {
                Id = payment.Id,
                Amount = payment.Amount,
                Description = payment.Description,
                status = payment.status,
                PaymentDate = payment.PaymentDate,
                UserId = payment.UserId,
                CenterSubscriptionId = payment.CenterSubscriptionId,
                EnrollmentId = payment.EnrollmentId,
            };
            return result;
        }
    }
}

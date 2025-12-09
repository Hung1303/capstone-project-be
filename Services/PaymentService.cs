using Azure.Core;
using BusinessObjects;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X9;
using Repository.Interfaces;
using Services.DTO.LessonPlan;
using Services.DTO.Payment;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpClient = httpClient;
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

        public async Task<PaymentResponse> UpdatePayment(Guid id, DateTime payDate)
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
            payDate = DateTime.SpecifyKind(payDate, DateTimeKind.Utc);
            payment.PaymentDate = payDate;

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

        public async Task<PaymentResponse> RefundVnpay(Guid paymentId)
        {
            var payment = await _unitOfWork.GetRepository<Payment>().Entities
                .Include(c => c.CenterSubscription)
                .Include(c => c.Enrollment)
                .FirstOrDefaultAsync(a => a.Id == paymentId && !a.IsDeleted && a.status == "DONE");
            if (payment == null)
            {
                throw new Exception("Payment Not Found");
            }
            string vnp_TmnCode = _configuration["Vnpay:TmnCode"];
            string vnp_HashSecret = _configuration["Vnpay:HashSecret"];
            string vnp_ApiUrl = _configuration["Vnpay:PaymendRefundUrl"];

            var requestData = new VnPayRefundRequest
            {
                RequestId = Guid.NewGuid().ToString().Replace("-", ""),
                Version = "2.1.0",
                Command = "refund",
                TmnCode = vnp_TmnCode,
                TransactionType = "02", 
                TxnRef = payment.Id.ToString(),
                Amount = payment.Amount * 100, 
                TransactionNo = "", 
                TransactionDate = payment.PaymentDate.ToString(), 
                CreateBy = payment.UserId.ToString(),
                CreateDate = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss"),
                IpAddr = "127.0.0.1",
                OrderInfo = "Hoan tien giao dich " + payment.Id.ToString(),
            };

            var signData = $"{requestData.RequestId}|{requestData.Version}|{requestData.Command}|{requestData.TmnCode}|" +
                           $"{requestData.TransactionType}|{requestData.TxnRef}|{requestData.Amount}|{requestData.TransactionNo}|" +
                           $"{requestData.TransactionDate}|{requestData.CreateBy}|{requestData.CreateDate}|{requestData.IpAddr}|" +
                           $"{requestData.OrderInfo}";

            requestData.SecureHash = HmacSHA512(vnp_HashSecret, signData);
            Console.WriteLine("request hash:" + requestData.SecureHash);

            try
            {
                var response = await _httpClient.PostAsJsonAsync(vnp_ApiUrl, requestData);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<VnPayRefundResponse>();
                //var ResponseData = new VnPayRefundResponse
                //{
                //    ResponseId = result.ResponseId,
                //    Command = result.Command,
                //    ResponseCode = result.ResponseCode,
                //    Message = result.Message,
                //    TmnCode = result.TmnCode,
                //    TxnRef = result.TxnRef,
                //    Amount = result.Amount,
                //    BankCode = result.BankCode,
                //    PayDate = result.PayDate,
                //    TransactionNo = result.TransactionNo,
                //    TransactionType = result.TransactionType,
                //    TransactionStatus = result.TransactionStatus,
                //    OrderInfo = result.OrderInfo,
                //};
                //string ResponseSignData = $"{ResponseData.ResponseId}|{ResponseData.Command}|{ResponseData.ResponseCode}|{ResponseData.Message}|{ResponseData.TmnCode}|" +
                //     $"{ResponseData.TxnRef}|{ResponseData.Amount}|{ResponseData.BankCode}|{ResponseData.PayDate}|{ResponseData.TransactionNo}|" +
                //     $"{ResponseData.TransactionType}|{ResponseData.TransactionStatus}|{ResponseData.OrderInfo}";
                //string myChecksum = HmacSHA512(vnp_HashSecret, ResponseSignData);
                //Console.WriteLine("response hash:" + myChecksum);
                //if (!myChecksum.Equals(result.SecureHash))
                //{
                //    throw new Exception("checksum failure");
                //}
                if (result.ResponseCode == "00")
                {
                    payment.status = "REFUNDED";
                }
                else
                {
                    throw new Exception("Refund Failure");
                }
                await _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);
                await _unitOfWork.SaveAsync();
                var paymentResult = new PaymentResponse
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
                return paymentResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw new Exception( ex.Message);
            }
        }
    }
}

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
using System.Linq;
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
            var payment = new Payment
            {
                Amount = request.Amount,
                Description = request.Description,
                status = "PENDING",
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
            };
            return result;
        }

        public async Task<string> CreateVNPAYUrl( Guid paymentId, string IpAddress)
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

            var uriBuilder = new UriBuilder(_configuration["Vnpay:BaseUrl"]);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["vnp_Version"] = "2.1.0";
            query["vnp_Command"] = "pay";
            query["vnp_TmnCode"] = _configuration["Vnpay:TmnCode"];
            query["vnp_Amount"] = (payment.Amount*100).ToString();
            query["vnp_CreateDate"] = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss");
            query["vnp_CurrCode"] = "VND";
            query["vnp_IpAddr"] = IpAddress;
            query["vnp_Locale"] = "vn";
            query["vnp_OrderInfo"] = payment.Description;
            query["vnp_OrderType"] = "other";
            query["vnp_ReturnUrl"] = _configuration["Vnpay:ReturnUrl"];
            query["vnp_ExpireDate"] = DateTime.UtcNow.AddHours(7).AddMinutes(5).ToString("yyyyMMddHHmmss");
            query["vnp_TxnRef"] = payment.Id.ToString();

            var hashData = new StringBuilder();
            var hashKeys = query.AllKeys
                .Where(k => k != "vnp_SecureHash")
                .OrderBy(k => k, StringComparer.Ordinal);

            foreach (var key in hashKeys)
            {
                var value = query[key];
                if (hashData.Length > 0) hashData.Append('&');
                hashData.Append($"{key}={Uri.EscapeDataString(value)}");
            }

            string hashSecret = _configuration["Vnpay:HashSecret"];
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hashSecret)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashData.ToString()));
                query["vnp_SecureHash"] = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }

            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();
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

        public async Task<IEnumerable<PaymentResponse>> GetAllPayments(int pageNumber, int pageSize, Guid? userId)
        {
            var payment = _unitOfWork.GetRepository<Payment>().Entities.Where(a => !a.IsDeleted);
            if (userId.HasValue)
            {
                payment = payment.Where(a => a.UserId == userId);
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
            };
            return result;
        }

        public async Task<PaymentResponse> UpdatePayment(Guid id)
        {
            var payment = await _unitOfWork.GetRepository<Payment>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (payment == null)
            {
                throw new Exception("Payment Not Found");
            }
            
            payment.status = "DONE";
            payment.PaymentDate = DateTime.UtcNow;
            
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
            };
            return result;
        }
    }
}

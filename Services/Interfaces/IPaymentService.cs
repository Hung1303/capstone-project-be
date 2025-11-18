using BusinessObjects;
using Core.Base;
using Services.DTO.LessonPlan;
using Services.DTO.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPaymentService 
    {
        Task<PaymentResponse> CreatePayment(CreatePaymentRequest request);
        Task<PaymentResponse> UpdatePayment(Guid id);
        Task<IEnumerable<PaymentResponse>> GetAllPayments(int pageNumber, int pageSize, Guid? userId);
        Task<PaymentResponse> GetPaymentById(Guid id);
        Task<bool> DeletePayment(Guid id);
        Task<string> CreateVNPAYUrl( Guid paymentId, string IpAddress);
    }
}

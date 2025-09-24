using Core.Base;

namespace BusinessObjects
{
    public class BillingRecord : BaseEntity
    {
        public BillingType BillingType { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? EnrollmentId { get; set; }
        public Guid ChargedUserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public DateTimeOffset? PaidAt { get; set; }
        public string? PaymentReference { get; set; }
        public string? Notes { get; set; }
    }
}



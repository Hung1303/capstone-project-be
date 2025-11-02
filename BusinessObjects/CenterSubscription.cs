using Core.Base;

namespace BusinessObjects
{
    public class CenterSubscription : BaseEntity
    {
        public Guid CenterProfileId { get; set; }
        public Guid SubscriptionPackageId { get; set; }
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Inactive;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? AutoRenewalDate { get; set; }
        public bool AutoRenewalEnabled { get; set; } = true;

        // Navigation properties
        public virtual CenterProfile CenterProfile { get; set; } = null!;
        public virtual SubscriptionPackage SubscriptionPackage { get; set; } = null!;
    }
}


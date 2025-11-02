using Core.Base;

namespace BusinessObjects
{
    public class SubscriptionPackage : BaseEntity
    {
        public string PackageName { get; set; } = string.Empty;
        public SubscriptionPackageTier Tier { get; set; }
        public int MaxCoursePosts { get; set; }
        public decimal MonthlyPrice { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;

        public virtual ICollection<CenterSubscription> CenterSubscriptions { get; set; } = new List<CenterSubscription>();
    }
}


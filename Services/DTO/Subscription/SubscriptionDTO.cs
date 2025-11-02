using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace Services.DTO.Subscription
{
    public class SubscriptionPackageResponse
    {
        public Guid Id { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public SubscriptionPackageTier Tier { get; set; }
        public int MaxCoursePosts { get; set; }
        public decimal MonthlyPrice { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class CenterSubscriptionResponse
    {
        public Guid Id { get; set; }
        public Guid CenterProfileId { get; set; }
        public string CenterName { get; set; } = string.Empty;
        public Guid SubscriptionPackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public SubscriptionPackageTier Tier { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? AutoRenewalDate { get; set; }
        public bool AutoRenewalEnabled { get; set; }
        public int MaxCoursePosts { get; set; }
        public int CurrentCoursePosts { get; set; }
        public int RemainingCoursePosts { get; set; }
    }

    public class CreateSubscriptionPackageRequest
    {
        [Required]
        [MaxLength(256)]
        public string PackageName { get; set; } = string.Empty;
        
        [Required]
        public SubscriptionPackageTier Tier { get; set; }
        
        [Range(1, int.MaxValue)]
        public int MaxCoursePosts { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal MonthlyPrice { get; set; }
        
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateSubscriptionPackageRequest
    {
        [MaxLength(256)]
        public string? PackageName { get; set; }
        
        public SubscriptionPackageTier? Tier { get; set; }
        
        [Range(1, int.MaxValue)]
        public int? MaxCoursePosts { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal? MonthlyPrice { get; set; }
        
        public string? Description { get; set; }
        
        public bool? IsActive { get; set; }
        
        public int? DisplayOrder { get; set; }
    }

    public class SubscribeCenterRequest
    {
        [Required]
        public Guid CenterProfileId { get; set; }
        
        [Required]
        public Guid SubscriptionPackageId { get; set; }
        
        public bool AutoRenewalEnabled { get; set; } = true;
    }

    public class UpgradeSubscriptionRequest
    {
        [Required]
        public Guid CenterProfileId { get; set; }
        
        [Required]
        public Guid NewSubscriptionPackageId { get; set; }
        
        public bool AutoRenewalEnabled { get; set; } = true;
    }

    public class CancelSubscriptionRequest
    {
        [Required]
        public Guid CenterSubscriptionId { get; set; }
        
        public string? CancellationReason { get; set; }
    }
}


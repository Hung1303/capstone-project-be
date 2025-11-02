using Core.Base;
using Services.DTO.Subscription;

namespace Services.Interfaces
{
    public interface ISubscriptionService
    {
        // Subscription Package Management
        Task<SubscriptionPackageResponse> CreateSubscriptionPackageAsync(CreateSubscriptionPackageRequest request);
        Task<SubscriptionPackageResponse> UpdateSubscriptionPackageAsync(Guid id, UpdateSubscriptionPackageRequest request);
        Task<bool> DeleteSubscriptionPackageAsync(Guid id);
        Task<IEnumerable<SubscriptionPackageResponse>> GetAllSubscriptionPackagesAsync(bool? activeOnly = null);
        Task<SubscriptionPackageResponse> GetSubscriptionPackageByIdAsync(Guid id);

        // Center Subscription Management
        Task<CenterSubscriptionResponse> SubscribeCenterAsync(SubscribeCenterRequest request);
        Task<CenterSubscriptionResponse> UpgradeSubscriptionAsync(UpgradeSubscriptionRequest request);
        Task<bool> CancelSubscriptionAsync(CancelSubscriptionRequest request);
        Task<CenterSubscriptionResponse?> GetActiveSubscriptionAsync(Guid centerProfileId);
        Task<IEnumerable<CenterSubscriptionResponse>> GetCenterSubscriptionsAsync(Guid centerProfileId);
        Task<IEnumerable<CenterSubscriptionResponse>> GetAllCenterSubscriptionsAsync(int pageNumber, int pageSize, SubscriptionStatus? status = null);
        
        // Subscription Validation
        Task<bool> CanCenterPostCourseAsync(Guid centerProfileId);
        Task<int> GetRemainingCoursePostsAsync(Guid centerProfileId);
        Task<int> GetMaxCoursePostsAsync(Guid centerProfileId);
    }
}


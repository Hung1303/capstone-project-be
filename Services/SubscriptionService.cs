using BusinessObjects;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.DTO.Subscription;
using Services.Interfaces;

namespace Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SubscriptionPackageResponse> CreateSubscriptionPackageAsync(CreateSubscriptionPackageRequest request)
        {
            var package = new SubscriptionPackage
            {
                PackageName = request.PackageName,
                Tier = request.Tier,
                MaxCoursePosts = request.MaxCoursePosts,
                MonthlyPrice = request.MonthlyPrice,
                Description = request.Description,
                IsActive = request.IsActive,
                DisplayOrder = request.DisplayOrder
            };

            await _unitOfWork.GetRepository<SubscriptionPackage>().InsertAsync(package);
            await _unitOfWork.SaveAsync();

            return new SubscriptionPackageResponse
            {
                Id = package.Id,
                PackageName = package.PackageName,
                Tier = package.Tier,
                MaxCoursePosts = package.MaxCoursePosts,
                MonthlyPrice = package.MonthlyPrice,
                Description = package.Description,
                IsActive = package.IsActive,
                DisplayOrder = package.DisplayOrder
            };
        }

        public async Task<SubscriptionPackageResponse> UpdateSubscriptionPackageAsync(Guid id, UpdateSubscriptionPackageRequest request)
        {
            var package = await _unitOfWork.GetRepository<SubscriptionPackage>()
                .Entities
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (package == null)
            {
                throw new Exception("Không tìm thấy gói dịch vụ.");
            }

            if (request.PackageName != null)
                package.PackageName = request.PackageName;
            if (request.Tier.HasValue)
                package.Tier = request.Tier.Value;
            if (request.MaxCoursePosts.HasValue)
                package.MaxCoursePosts = request.MaxCoursePosts.Value;
            if (request.MonthlyPrice.HasValue)
                package.MonthlyPrice = request.MonthlyPrice.Value;
            if (request.Description != null)
                package.Description = request.Description;
            if (request.IsActive.HasValue)
                package.IsActive = request.IsActive.Value;
            if (request.DisplayOrder.HasValue)
                package.DisplayOrder = request.DisplayOrder.Value;

            package.LastUpdatedAt = DateTime.UtcNow;
            await _unitOfWork.GetRepository<SubscriptionPackage>().UpdateAsync(package);
            await _unitOfWork.SaveAsync();

            return new SubscriptionPackageResponse
            {
                Id = package.Id,
                PackageName = package.PackageName,
                Tier = package.Tier,
                MaxCoursePosts = package.MaxCoursePosts,
                MonthlyPrice = package.MonthlyPrice,
                Description = package.Description,
                IsActive = package.IsActive,
                DisplayOrder = package.DisplayOrder
            };
        }

        public async Task<bool> DeleteSubscriptionPackageAsync(Guid id)
        {
            var package = await _unitOfWork.GetRepository<SubscriptionPackage>()
                .Entities
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (package == null)
            {
                throw new Exception("Không tìm thấy gói dịch vụ.");
            }

            
            var hasActiveSubscriptions = await _unitOfWork.GetRepository<CenterSubscription>()
                .Entities
                .AnyAsync(s => s.SubscriptionPackageId == id &&
                              s.Status == SubscriptionStatus.Active &&
                              !s.IsDeleted);

            if (hasActiveSubscriptions)
            {
                throw new Exception("Không thể xóa gói dịch vụ đang có người đăng ký.");
            }

            package.IsDeleted = true;
            package.LastUpdatedAt = DateTime.UtcNow;
            await _unitOfWork.GetRepository<SubscriptionPackage>().UpdateAsync(package);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<IEnumerable<SubscriptionPackageResponse>> GetAllSubscriptionPackagesAsync(bool? activeOnly = null)
        {
            var query = _unitOfWork.GetRepository<SubscriptionPackage>().Entities
                .Where(p => !p.IsDeleted);

            if (activeOnly == true)
            {
                query = query.Where(p => p.IsActive);
            }

            var packages = await query
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.MonthlyPrice)
                .Select(p => new SubscriptionPackageResponse
                {
                    Id = p.Id,
                    PackageName = p.PackageName,
                    Tier = p.Tier,
                    MaxCoursePosts = p.MaxCoursePosts,
                    MonthlyPrice = p.MonthlyPrice,
                    Description = p.Description,
                    IsActive = p.IsActive,
                    DisplayOrder = p.DisplayOrder
                })
                .ToListAsync();

            return packages;
        }

        public async Task<SubscriptionPackageResponse> GetSubscriptionPackageByIdAsync(Guid id)
        {
            var package = await _unitOfWork.GetRepository<SubscriptionPackage>()
                .Entities
                .Where(p => p.Id == id && !p.IsDeleted)
                .Select(p => new SubscriptionPackageResponse
                {
                    Id = p.Id,
                    PackageName = p.PackageName,
                    Tier = p.Tier,
                    MaxCoursePosts = p.MaxCoursePosts,
                    MonthlyPrice = p.MonthlyPrice,
                    Description = p.Description,
                    IsActive = p.IsActive,
                    DisplayOrder = p.DisplayOrder
                })
                .FirstOrDefaultAsync();

            if (package == null)
            {
                throw new Exception("Không tìm thấy gói dịch vụ.");
            }

            return package;
        }

        public async Task<CenterSubscriptionResponse> SubscribeCenterAsync(SubscribeCenterRequest request)
        {
            var center = await _unitOfWork.GetRepository<CenterProfile>()
                .Entities
                .FirstOrDefaultAsync(c => c.Id == request.CenterProfileId && !c.IsDeleted);

            if (center == null)
            {
                throw new Exception("Không tìm thấy trung tâm.");
            }

            var package = await _unitOfWork.GetRepository<SubscriptionPackage>()
                .Entities
                .FirstOrDefaultAsync(p => p.Id == request.SubscriptionPackageId && !p.IsDeleted && p.IsActive);

            if (package == null)
            {
                throw new Exception("Không tìm thấy gói dịch vụ hoặc gói dịch vụ chưa kích hoạt.");
            }

            var existingActiveSubscription = await _unitOfWork.GetRepository<CenterSubscription>()
                .Entities
                .FirstOrDefaultAsync(s => s.CenterProfileId == request.CenterProfileId &&
                                         s.Status == SubscriptionStatus.Active &&
                                         !s.IsDeleted);

            if (existingActiveSubscription != null)
            {
                existingActiveSubscription.Status = SubscriptionStatus.Cancelled;
                existingActiveSubscription.CancelledAt = DateTime.UtcNow;
                existingActiveSubscription.LastUpdatedAt = DateTime.UtcNow;
                await _unitOfWork.GetRepository<CenterSubscription>().UpdateAsync(existingActiveSubscription);
            }

           
            var now = DateTime.UtcNow;
            var endDate = now.AddMonths(1);
            var autoRenewalDate = request.AutoRenewalEnabled ? endDate : (DateTime?)null;

            var subscription = new CenterSubscription
            {
                CenterProfileId = request.CenterProfileId,
                SubscriptionPackageId = request.SubscriptionPackageId,
                Status = SubscriptionStatus.Active,
                StartDate = now,
                EndDate = endDate,
                AutoRenewalEnabled = request.AutoRenewalEnabled,
                AutoRenewalDate = autoRenewalDate
            };

            await _unitOfWork.GetRepository<CenterSubscription>().InsertAsync(subscription);
            await _unitOfWork.SaveAsync();

            return await GetCenterSubscriptionResponseAsync(subscription.Id);
        }

        public async Task<CenterSubscriptionResponse> UpgradeSubscriptionAsync(UpgradeSubscriptionRequest request)
        {
            var center = await _unitOfWork.GetRepository<CenterProfile>()
                .Entities
                .FirstOrDefaultAsync(c => c.Id == request.CenterProfileId && !c.IsDeleted);

            if (center == null)
            {
                throw new Exception("Không tìm thấy trung tâm.");
            }

            var newPackage = await _unitOfWork.GetRepository<SubscriptionPackage>()
                .Entities
                .FirstOrDefaultAsync(p => p.Id == request.NewSubscriptionPackageId && !p.IsDeleted && p.IsActive);

            if (newPackage == null)
            {
                throw new Exception("Gói dịch vụ mới không tìm thấy hoặc chưa kích hoạt.");
            }

            var currentSubscription = await _unitOfWork.GetRepository<CenterSubscription>()
                .Entities
                .FirstOrDefaultAsync(s => s.CenterProfileId == request.CenterProfileId &&
                                         s.Status == SubscriptionStatus.Active &&
                                         !s.IsDeleted);

            if (currentSubscription == null)
            {
               
                return await SubscribeCenterAsync(new SubscribeCenterRequest
                {
                    CenterProfileId = request.CenterProfileId,
                    SubscriptionPackageId = request.NewSubscriptionPackageId,
                    AutoRenewalEnabled = request.AutoRenewalEnabled
                });
            }

           
            var currentPackage = await _unitOfWork.GetRepository<SubscriptionPackage>()
                .Entities
                .FirstOrDefaultAsync(p => p.Id == currentSubscription.SubscriptionPackageId && !p.IsDeleted);

            if (currentPackage != null && newPackage.Tier <= currentPackage.Tier)
            {
                throw new Exception("Chỉ có thể nâng cấp lên gói bậc cao hơn.");
            }

           
            currentSubscription.Status = SubscriptionStatus.Cancelled;
            currentSubscription.CancelledAt = DateTime.UtcNow;
            currentSubscription.LastUpdatedAt = DateTime.UtcNow;
            await _unitOfWork.GetRepository<CenterSubscription>().UpdateAsync(currentSubscription);

            
            var now = DateTime.UtcNow;
            var endDate = now.AddMonths(1);
            var autoRenewalDate = request.AutoRenewalEnabled ? endDate : (DateTime?)null;

            var newSubscription = new CenterSubscription
            {
                CenterProfileId = request.CenterProfileId,
                SubscriptionPackageId = request.NewSubscriptionPackageId,
                Status = SubscriptionStatus.Active,
                StartDate = now,
                EndDate = endDate,
                AutoRenewalEnabled = request.AutoRenewalEnabled,
                AutoRenewalDate = autoRenewalDate
            };

            await _unitOfWork.GetRepository<CenterSubscription>().InsertAsync(newSubscription);
            await _unitOfWork.SaveAsync();

            return await GetCenterSubscriptionResponseAsync(newSubscription.Id);
        }

        public async Task<bool> CancelSubscriptionAsync(CancelSubscriptionRequest request)
        {
            var subscription = await _unitOfWork.GetRepository<CenterSubscription>()
                .Entities
                .FirstOrDefaultAsync(s => s.Id == request.CenterSubscriptionId && !s.IsDeleted);

            if (subscription == null)
            {
                throw new Exception("Không tìm thấy gói dịch vụ.");
            }

            if (subscription.Status != SubscriptionStatus.Active)
            {
                throw new Exception("Chỉ gói dịch vụ đang kích hoạt mới có thể bị hủy.");
            }

            subscription.Status = SubscriptionStatus.Cancelled;
            subscription.CancelledAt = DateTime.UtcNow;
            subscription.CancellationReason = request.CancellationReason;
            subscription.AutoRenewalEnabled = false;
            subscription.AutoRenewalDate = null;
            subscription.LastUpdatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<CenterSubscription>().UpdateAsync(subscription);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<CenterSubscriptionResponse?> GetActiveSubscriptionAsync(Guid centerProfileId)
        {
            var subscription = await _unitOfWork.GetRepository<CenterSubscription>()
                .Entities
                .Where(s => s.CenterProfileId == centerProfileId &&
                           s.Status == SubscriptionStatus.Active &&
                           s.EndDate >= DateTime.UtcNow &&
                           !s.IsDeleted)
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                return null;
            }

            return await GetCenterSubscriptionResponseAsync(subscription.Id);
        }

        public async Task<IEnumerable<CenterSubscriptionResponse>> GetCenterSubscriptionsAsync(Guid centerProfileId)
        {
            var subscriptions = await _unitOfWork.GetRepository<CenterSubscription>()
                .Entities
                .Where(s => s.CenterProfileId == centerProfileId && !s.IsDeleted)
                .OrderByDescending(s => s.StartDate)
                .Select(s => s.Id)
                .ToListAsync();

            var responses = new List<CenterSubscriptionResponse>();
            foreach (var subscriptionId in subscriptions)
            {
                var response = await GetCenterSubscriptionResponseAsync(subscriptionId);
                if (response != null)
                {
                    responses.Add(response);
                }
            }

            return responses;
        }

        public async Task<IEnumerable<CenterSubscriptionResponse>> GetAllCenterSubscriptionsAsync(int pageNumber, int pageSize, SubscriptionStatus? status = null)
        {
            var query = _unitOfWork.GetRepository<CenterSubscription>()
                .Entities
                .Where(s => !s.IsDeleted);

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;

            var subscriptionIds = await query
                .OrderByDescending(s => s.StartDate)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(s => s.Id)
                .ToListAsync();

            var responses = new List<CenterSubscriptionResponse>();
            foreach (var subscriptionId in subscriptionIds)
            {
                var response = await GetCenterSubscriptionResponseAsync(subscriptionId);
                if (response != null)
                {
                    responses.Add(response);
                }
            }

            return responses;
        }

        public async Task<bool> CanCenterPostCourseAsync(Guid centerProfileId)
        {
            var activeSubscription = await GetActiveSubscriptionAsync(centerProfileId);
            if (activeSubscription == null)
            {
                return false;
            }

            return activeSubscription.RemainingCoursePosts > 0;
        }

        public async Task<int> GetRemainingCoursePostsAsync(Guid centerProfileId)
        {
            var activeSubscription = await GetActiveSubscriptionAsync(centerProfileId);
            if (activeSubscription == null)
            {
                return 0;
            }

            return activeSubscription.RemainingCoursePosts;
        }

        public async Task<int> GetMaxCoursePostsAsync(Guid centerProfileId)
        {
            var activeSubscription = await GetActiveSubscriptionAsync(centerProfileId);
            if (activeSubscription == null)
            {
                return 0;
            }

            return activeSubscription.MaxCoursePosts;
        }

        
        private async Task<CenterSubscriptionResponse?> GetCenterSubscriptionResponseAsync(Guid subscriptionId)
        {
            var subscription = await _unitOfWork.GetRepository<CenterSubscription>()
                .Entities
                .Include(s => s.CenterProfile)
                .Include(s => s.SubscriptionPackage)
                .FirstOrDefaultAsync(s => s.Id == subscriptionId && !s.IsDeleted);

            if (subscription == null)
            {
                return null;
            }

            //To calculate the remaining course posts
            var currentPublishedCoursePosts = await _unitOfWork.GetRepository<Course>()
                .Entities
                .CountAsync(c => c.CenterProfileId == subscription.CenterProfileId &&
                     !c.IsDeleted &&
                     c.IsPublished && 
                     c.PublishedAt >= subscription.StartDate &&
                     c.PublishedAt <= subscription.EndDate);

            var remaining = subscription.SubscriptionPackage.MaxCoursePosts - currentPublishedCoursePosts;
            if (remaining < 0) remaining = 0;


            // Update subscription status if expired
            if (subscription.Status == SubscriptionStatus.Active && subscription.EndDate < DateTime.UtcNow)
            {
                subscription.Status = SubscriptionStatus.Expired;
                subscription.LastUpdatedAt = DateTime.UtcNow;
                await _unitOfWork.GetRepository<CenterSubscription>().UpdateAsync(subscription);
                await _unitOfWork.SaveAsync();
            }

            return new CenterSubscriptionResponse
            {
                Id = subscription.Id,
                CenterProfileId = subscription.CenterProfileId,
                CenterName = subscription.CenterProfile.CenterName,
                SubscriptionPackageId = subscription.SubscriptionPackageId,
                PackageName = subscription.SubscriptionPackage.PackageName,
                Tier = subscription.SubscriptionPackage.Tier,
                Status = subscription.Status,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                CancelledAt = subscription.CancelledAt,
                CancellationReason = subscription.CancellationReason,
                AutoRenewalDate = subscription.AutoRenewalDate,
                AutoRenewalEnabled = subscription.AutoRenewalEnabled,
                MaxCoursePosts = subscription.SubscriptionPackage.MaxCoursePosts,
                CurrentCoursePosts = currentPublishedCoursePosts,
                RemainingCoursePosts = remaining
            };
        }
    }
}


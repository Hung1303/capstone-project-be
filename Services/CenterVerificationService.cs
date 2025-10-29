using BusinessObjects;
using Services.DTO.CenterVerification;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.Interfaces;
using System.Text.Json;

namespace Services
{
    public class CenterVerificationService : ICenterVerificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CenterVerificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateVerificationRequestDto> CreateVerificationRequestAsync(CreateVerificationRequestDto request)
        {
            // Check if center exists and is in pending status
            var center = await _unitOfWork.GetRepository<CenterProfile>()
                .Entities
                .Include(c => c.VerificationRequests)
                .FirstOrDefaultAsync(c => c.Id == request.CenterProfileId);

            if (center == null)
                throw new Exception("Center not found");

            if (center.Status != CenterStatus.Pending)
                throw new Exception("Center is not in pending status for verification");

            // Check if there's already a pending verification request
            var existingRequest = center.VerificationRequests
                .FirstOrDefault(vr => vr.Status == VerificationStatus.Pending || vr.Status == VerificationStatus.InProgress);

            if (existingRequest != null)
                throw new Exception("There is already a pending or in-progress verification request for this center");

            // Create verification request
            var verificationRequest = new CenterVerificationRequest
            {
                CenterProfileId = request.CenterProfileId,
                InspectorId = request.InspectorId,
                Status = VerificationStatus.Pending,
                ScheduledDate = request.ScheduledDate
            };

            await _unitOfWork.GetRepository<CenterVerificationRequest>().InsertAsync(verificationRequest);

            // Update center status
            center.Status = CenterStatus.UnderVerification;
            center.VerificationRequestedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return request;
        }

        public async Task<UpdateVerificationRequestDto> UpdateVerificationRequestAsync(Guid verificationId, UpdateVerificationRequestDto request)
        {
            var verificationRequest = await _unitOfWork.GetRepository<CenterVerificationRequest>()
                .Entities
                .FirstOrDefaultAsync(vr => vr.Id == verificationId);

            if (verificationRequest == null)
                throw new Exception("Verification request not found");

            if (verificationRequest.Status != VerificationStatus.Pending && verificationRequest.Status != VerificationStatus.InProgress)
                throw new Exception("Verification request is not in a state that can be updated");

            // Update verification request
            verificationRequest.Status = VerificationStatus.InProgress;
            verificationRequest.CompletedDate = request.CompletedDate;
            verificationRequest.InspectorNotes = request.InspectorNotes;
            verificationRequest.VerificationPhotos = request.VerificationPhotos != null ? JsonSerializer.Serialize(request.VerificationPhotos) : null;
            verificationRequest.DocumentChecklist = request.DocumentChecklist != null ? JsonSerializer.Serialize(request.DocumentChecklist) : null;
            verificationRequest.IsLocationVerified = request.IsLocationVerified;
            verificationRequest.IsDocumentsVerified = request.IsDocumentsVerified;
            verificationRequest.IsLicenseValid = request.IsLicenseValid;

            await _unitOfWork.SaveAsync();

            return request;
        }

        public async Task<AdminDecisionDto> MakeAdminDecisionAsync(Guid verificationId, AdminDecisionDto decision)
        {
            var verificationRequest = await _unitOfWork.GetRepository<CenterVerificationRequest>()
                .Entities
                .Include(vr => vr.CenterProfile)
                .FirstOrDefaultAsync(vr => vr.Id == verificationId);

            if (verificationRequest == null)
                throw new Exception("Verification request not found");

            if (verificationRequest.Status != VerificationStatus.InProgress && verificationRequest.Status != VerificationStatus.Completed)
                throw new Exception("Verification request is not ready for admin decision");

            // Update verification request with admin decision
            verificationRequest.AdminId = decision.AdminId;
            verificationRequest.AdminDecisionDate = DateTime.UtcNow;
            verificationRequest.AdminDecision = decision.Decision;
            verificationRequest.AdminNotes = decision.AdminNotes;
            verificationRequest.Status = VerificationStatus.Finalized;

            // Update center status based on admin decision
            var center = verificationRequest.CenterProfile;
            if (decision.Decision == ApprovalDecision.Approved)
            {
                center.Status = CenterStatus.Active; // Set to Active for operational centers
                center.VerificationCompletedAt = DateTime.UtcNow;
                center.VerificationNotes = decision.AdminNotes;

                // Also update the user account status to Active
                var user = await _unitOfWork.GetRepository<User>()
                    .Entities
                    .FirstOrDefaultAsync(u => u.Id == center.UserId);

                if (user != null)
                {
                    user.Status = AccountStatus.Active;
                    await _unitOfWork.GetRepository<User>().UpdateAsync(user);
                }
            }
            else
            {
                center.Status = CenterStatus.Rejected;
                center.RejectionReason = decision.AdminNotes;
            }

            await _unitOfWork.SaveAsync();

            return decision;
        }

        public async Task<VerificationRequestResponseDto?> GetVerificationRequestByIdAsync(Guid verificationId)
        {
            var verificationRequest = await _unitOfWork.GetRepository<CenterVerificationRequest>()
                .Entities
                .Include(vr => vr.CenterProfile)
                .Include(vr => vr.Inspector)
                .Include(vr => vr.Admin)
                .FirstOrDefaultAsync(vr => vr.Id == verificationId);

            if (verificationRequest == null)
                return null;

            return new VerificationRequestResponseDto
            {
                Id = verificationRequest.Id,
                CenterProfileId = verificationRequest.CenterProfileId,
                CenterName = verificationRequest.CenterProfile.CenterName,
                InspectorId = verificationRequest.InspectorId,
                InspectorName = verificationRequest.Inspector.FullName,
                Status = verificationRequest.Status,
                ScheduledDate = verificationRequest.ScheduledDate,
                CompletedDate = verificationRequest.CompletedDate,
                InspectorNotes = verificationRequest.InspectorNotes,
                VerificationPhotos = verificationRequest.VerificationPhotos != null ? JsonSerializer.Deserialize<List<string>>(verificationRequest.VerificationPhotos) : null,
                DocumentChecklist = verificationRequest.DocumentChecklist != null ? JsonSerializer.Deserialize<List<string>>(verificationRequest.DocumentChecklist) : null,
                IsLocationVerified = verificationRequest.IsLocationVerified,
                IsDocumentsVerified = verificationRequest.IsDocumentsVerified,
                IsLicenseValid = verificationRequest.IsLicenseValid,
                AdminId = verificationRequest.AdminId,
                AdminName = verificationRequest.Admin?.FullName,
                AdminDecisionDate = verificationRequest.AdminDecisionDate,
                AdminDecision = verificationRequest.AdminDecision,
                AdminNotes = verificationRequest.AdminNotes,
                CreatedAt = verificationRequest.CreatedAt
            };
        }

        public async Task<List<VerificationRequestResponseDto>> GetVerificationRequestsByInspectorAsync(Guid inspectorId)
        {
            var verificationRequests = await _unitOfWork.GetRepository<CenterVerificationRequest>()
                .Entities
                .Include(vr => vr.CenterProfile)
                .Include(vr => vr.Inspector)
                .Include(vr => vr.Admin)
                .Where(vr => vr.InspectorId == inspectorId)
                .OrderByDescending(vr => vr.CreatedAt)
                .ToListAsync();

            return verificationRequests.Select(vr => new VerificationRequestResponseDto
            {
                Id = vr.Id,
                CenterProfileId = vr.CenterProfileId,
                CenterName = vr.CenterProfile.CenterName,
                InspectorId = vr.InspectorId,
                InspectorName = vr.Inspector.FullName,
                Status = vr.Status,
                ScheduledDate = vr.ScheduledDate,
                CompletedDate = vr.CompletedDate,
                InspectorNotes = vr.InspectorNotes,
                VerificationPhotos = vr.VerificationPhotos != null ? JsonSerializer.Deserialize<List<string>>(vr.VerificationPhotos) : null,
                DocumentChecklist = vr.DocumentChecklist != null ? JsonSerializer.Deserialize<List<string>>(vr.DocumentChecklist) : null,
                IsLocationVerified = vr.IsLocationVerified,
                IsDocumentsVerified = vr.IsDocumentsVerified,
                IsLicenseValid = vr.IsLicenseValid,
                AdminId = vr.AdminId,
                AdminName = vr.Admin?.FullName,
                AdminDecisionDate = vr.AdminDecisionDate,
                AdminDecision = vr.AdminDecision,
                AdminNotes = vr.AdminNotes,
                CreatedAt = vr.CreatedAt
            }).ToList();
        }

        public async Task<List<VerificationRequestResponseDto>> GetPendingVerificationRequestsAsync()
        {
            var verificationRequests = await _unitOfWork.GetRepository<CenterVerificationRequest>()
                .Entities
                .Include(vr => vr.CenterProfile)
                .Include(vr => vr.Inspector)
                .Include(vr => vr.Admin)
                .Where(vr => vr.Status == VerificationStatus.Pending || vr.Status == VerificationStatus.InProgress || vr.Status == VerificationStatus.Completed)
                .OrderByDescending(vr => vr.CreatedAt)
                .ToListAsync();

            return verificationRequests.Select(vr => new VerificationRequestResponseDto
            {
                Id = vr.Id,
                CenterProfileId = vr.CenterProfileId,
                CenterName = vr.CenterProfile.CenterName,
                InspectorId = vr.InspectorId,
                InspectorName = vr.Inspector.FullName,
                Status = vr.Status,
                ScheduledDate = vr.ScheduledDate,
                CompletedDate = vr.CompletedDate,
                InspectorNotes = vr.InspectorNotes,
                VerificationPhotos = vr.VerificationPhotos != null ? JsonSerializer.Deserialize<List<string>>(vr.VerificationPhotos) : null,
                DocumentChecklist = vr.DocumentChecklist != null ? JsonSerializer.Deserialize<List<string>>(vr.DocumentChecklist) : null,
                IsLocationVerified = vr.IsLocationVerified,
                IsDocumentsVerified = vr.IsDocumentsVerified,
                IsLicenseValid = vr.IsLicenseValid,
                AdminId = vr.AdminId,
                AdminName = vr.Admin?.FullName,
                AdminDecisionDate = vr.AdminDecisionDate,
                AdminDecision = vr.AdminDecision,
                AdminNotes = vr.AdminNotes,
                CreatedAt = vr.CreatedAt
            }).ToList();
        }

        public async Task<List<CenterVerificationListDto>> GetCentersPendingVerificationAsync()
        {
            var centers = await _unitOfWork.GetRepository<CenterProfile>()
                .Entities
                .Where(c => c.Status == CenterStatus.Pending || c.Status == CenterStatus.UnderVerification)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return centers.Select(c => new CenterVerificationListDto
            {
                Id = c.Id,
                CenterName = c.CenterName,
                OwnerName = c.OwnerName ?? "",
                Address = c.Address,
                ContactEmail = c.ContactEmail ?? "",
                ContactPhone = c.ContactPhone ?? "",
                Status = c.Status,
                VerificationRequestedAt = c.VerificationRequestedAt,
                VerificationCompletedAt = c.VerificationCompletedAt,
                RejectionReason = c.RejectionReason,
                CreatedAt = c.CreatedAt
            }).ToList();
        }

        public async Task<List<CenterVerificationListDto>> GetCentersByStatusAsync(CenterStatus status)
        {
            var centers = await _unitOfWork.GetRepository<CenterProfile>()
                .Entities
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return centers.Select(c => new CenterVerificationListDto
            {
                Id = c.Id,
                CenterName = c.CenterName,
                OwnerName = c.OwnerName ?? "",
                Address = c.Address,
                ContactEmail = c.ContactEmail ?? "",
                ContactPhone = c.ContactPhone ?? "",
                Status = c.Status,
                VerificationRequestedAt = c.VerificationRequestedAt,
                VerificationCompletedAt = c.VerificationCompletedAt,
                RejectionReason = c.RejectionReason,
                CreatedAt = c.CreatedAt
            }).ToList();
        }

        public async Task<bool> CompleteVerificationAsync(Guid verificationId)
        {
            var verificationRequest = await _unitOfWork.GetRepository<CenterVerificationRequest>()
                .Entities
                .Include(vr => vr.CenterProfile)
                .FirstOrDefaultAsync(vr => vr.Id == verificationId);

            if (verificationRequest == null)
                return false;

            if (verificationRequest.Status != VerificationStatus.InProgress)
                return false;

            // Update verification request status
            verificationRequest.Status = VerificationStatus.Completed;
            verificationRequest.CompletedDate = DateTime.UtcNow;

            // Update center status to Verified
            var center = verificationRequest.CenterProfile;
            center.Status = CenterStatus.Verified;
            center.VerificationCompletedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> SuspendCenterAsync(Guid centerId, string reason, Guid adminId)
        {
            var center = await _unitOfWork.GetRepository<CenterProfile>()
                .Entities
                .FirstOrDefaultAsync(c => c.Id == centerId && !c.IsDeleted);

            if (center == null)
                return false;

            if (center.Status != CenterStatus.Active)
                return false; // Can only suspend active centers

            // Update center status
            center.Status = CenterStatus.Suspended;
            center.LastUpdatedAt = DateTime.UtcNow;
            center.RejectionReason = reason; // Reusing field for suspension reason

            // Also suspend the user account
            var user = await _unitOfWork.GetRepository<User>()
                .Entities
                .FirstOrDefaultAsync(u => u.Id == center.UserId);

            if (user != null)
            {
                user.Status = AccountStatus.Suspended;
                await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            }

            await _unitOfWork.GetRepository<CenterProfile>().UpdateAsync(center);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> RestoreCenterAsync(Guid centerId, string reason, Guid adminId)
        {
            var center = await _unitOfWork.GetRepository<CenterProfile>()
                .Entities
                .FirstOrDefaultAsync(c => c.Id == centerId && !c.IsDeleted);

            if (center == null)
                return false;

            if (center.Status != CenterStatus.Suspended)
                return false; // Can only restore suspended centers

            // Update center status
            center.Status = CenterStatus.Active;
            center.LastUpdatedAt = DateTime.UtcNow;
            center.VerificationNotes = reason; // Store restoration reason

            // Also restore the user account
            var user = await _unitOfWork.GetRepository<User>()
                .Entities
                .FirstOrDefaultAsync(u => u.Id == center.UserId);

            if (user != null)
            {
                user.Status = AccountStatus.Active;
                await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            }

            await _unitOfWork.GetRepository<CenterProfile>().UpdateAsync(center);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}

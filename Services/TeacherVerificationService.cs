using BusinessObjects;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.DTO.TeacherVerification;
using Services.Interfaces;

namespace Services
{
    public class TeacherVerificationService : ITeacherVerificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public TeacherVerificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TeacherVerificationResponse> RequestVerification(TeacherVerificationRequestDto request)
        {
            var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities
                .FirstOrDefaultAsync(t => t.Id == request.TeacherProfileId && !t.IsDeleted);
            if (teacher == null) throw new Exception("TeacherProfile not found");

            // Guard: Already verified
            if (teacher.VerificationStatus == VerificationStatus.Completed || teacher.VerificationStatus == VerificationStatus.Finalized)
            {
                throw new Exception("Teacher is already verified");
            }

            // Guard: User already active
            var user = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == teacher.UserId && !u.IsDeleted);
            if (user != null && user.Status == AccountStatus.Active)
            {
                throw new Exception("User account is already active; verification not required");
            }

            // Guard: Existing pending verification request
            var existingPending = await _unitOfWork.GetRepository<TeacherVerificationRequest>().Entities
                .AnyAsync(v => v.TeacherProfileId == teacher.Id && !v.IsDeleted && v.Status == VerificationStatus.Pending);
            if (existingPending)
            {
                throw new Exception("A pending verification request already exists");
            }

            var ver = new TeacherVerificationRequest
            {
                TeacherProfileId = request.TeacherProfileId,
                Status = VerificationStatus.Pending,
                RequestedAt = DateTime.UtcNow,
                Notes = request.Notes
            };
            await _unitOfWork.GetRepository<TeacherVerificationRequest>().InsertAsync(ver);

            // update teacher status timestamps
            teacher.VerificationStatus = VerificationStatus.Pending;
            teacher.VerificationRequestedAt = DateTime.UtcNow;
            await _unitOfWork.GetRepository<TeacherProfile>().UpdateAsync(teacher);

            await _unitOfWork.SaveAsync();
            return Map(ver);
        }

        public async Task<TeacherVerificationResponse> GetById(Guid id)
        {
            var ver = await _unitOfWork.GetRepository<TeacherVerificationRequest>().Entities
                .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
            if (ver == null) throw new Exception("Verification request not found");
            return Map(ver);
        }

        public async Task<IEnumerable<TeacherVerificationResponse>> GetAll(string? search, int pageNumber, int pageSize, Guid? teacherProfileId)
        {
            var q = _unitOfWork.GetRepository<TeacherVerificationRequest>().Entities.Where(x => !x.IsDeleted);
            if (teacherProfileId.HasValue) q = q.Where(x => x.TeacherProfileId == teacherProfileId);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(x => (x.Notes != null && x.Notes.ToLower().Contains(s)));
            }
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var items = await q.OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return items.Select(Map);
        }

        public async Task<TeacherVerificationResponse> UpdateDocuments(Guid id, TeacherVerificationDocumentsDto request)
        {
            var ver = await _unitOfWork.GetRepository<TeacherVerificationRequest>().Entities.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
            if (ver == null) throw new Exception("Verification request not found");

            if (request.QualificationCertificatePath != null) ver.QualificationCertificatePath = request.QualificationCertificatePath;
            if (request.EmploymentContractPath != null) ver.EmploymentContractPath = request.EmploymentContractPath;
            if (request.ApprovalFromCenterPath != null) ver.ApprovalFromCenterPath = request.ApprovalFromCenterPath;
            if (request.OtherDocumentsPath != null) ver.OtherDocumentsPath = request.OtherDocumentsPath;
            ver.Status = VerificationStatus.InProgress;

            await _unitOfWork.GetRepository<TeacherVerificationRequest>().UpdateAsync(ver);
            await _unitOfWork.SaveAsync();
            return Map(ver);
        }

        public async Task<TeacherVerificationResponse> SetStatus(Guid id, SetTeacherVerificationStatusDto request)
        {
            var ver = await _unitOfWork.GetRepository<TeacherVerificationRequest>().Entities.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
            if (ver == null) throw new Exception("Verification request not found");

            ver.Status = request.Status;
            ver.Notes = request.Notes ?? ver.Notes;
            ver.InspectorId = request.InspectorId;
            ver.AdminId = request.AdminId;
            if (request.Status == VerificationStatus.Completed || request.Status == VerificationStatus.Finalized)
            {
                ver.CompletedAt = DateTime.UtcNow;
            }

            await _unitOfWork.GetRepository<TeacherVerificationRequest>().UpdateAsync(ver);

            // reflect into teacher profile
            var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(t => t.Id == ver.TeacherProfileId && !t.IsDeleted);
            if (teacher != null)
            {
                teacher.VerificationStatus = request.Status;
                teacher.VerificationCompletedAt = (request.Status == VerificationStatus.Completed || request.Status == VerificationStatus.Finalized) ? DateTime.UtcNow : teacher.VerificationCompletedAt;
                teacher.VerificationNotes = request.Notes ?? teacher.VerificationNotes;
                await _unitOfWork.GetRepository<TeacherProfile>().UpdateAsync(teacher);

                // If verification completed/finalized, activate the associated User account
                if (request.Status == VerificationStatus.Completed || request.Status == VerificationStatus.Finalized)
                {
                    var user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Id == teacher.UserId && !u.IsDeleted);
                    if (user != null)
                    {
                        user.Status = AccountStatus.Active;
                        await _unitOfWork.GetRepository<User>().UpdateAsync(user);
                    }
                }
            }

            await _unitOfWork.SaveAsync();
            return Map(ver);
        }

        private static TeacherVerificationResponse Map(TeacherVerificationRequest v) => new TeacherVerificationResponse
        {
            Id = v.Id,
            TeacherProfileId = v.TeacherProfileId,
            Status = v.Status,
            RequestedAt = v.RequestedAt,
            CompletedAt = v.CompletedAt,
            Notes = v.Notes,
            QualificationCertificatePath = v.QualificationCertificatePath,
            EmploymentContractPath = v.EmploymentContractPath,
            ApprovalFromCenterPath = v.ApprovalFromCenterPath,
            OtherDocumentsPath = v.OtherDocumentsPath
        };
    }
}



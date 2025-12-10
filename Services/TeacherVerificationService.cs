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
            if (teacher == null) throw new Exception("Không tìm thấy giáo viên.");

            
            if (teacher.VerificationStatus == VerificationStatus.Completed || teacher.VerificationStatus == VerificationStatus.Finalized)
            {
                throw new Exception("Giáo viên này đã được xác minh.");
            }

            
            var user = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == teacher.UserId && !u.IsDeleted);
            if (user != null && user.Status == AccountStatus.Active)
            {
                throw new Exception("Tài khoản đã được kích hoạt. Không cần phải xác minh.");
            }

            
            var existingPending = await _unitOfWork.GetRepository<TeacherVerificationRequest>().Entities
                .AnyAsync(v => v.TeacherProfileId == teacher.Id && !v.IsDeleted && v.Status == VerificationStatus.Pending);
            if (existingPending)
            {
                throw new Exception("Đã có yêu cầu xác minh đang chờ duyệt.");
            }

            var ver = new TeacherVerificationRequest
            {
                TeacherProfileId = request.TeacherProfileId,
                Status = VerificationStatus.Pending,
                RequestedAt = DateTime.UtcNow,
                Notes = request.Notes
            };
            await _unitOfWork.GetRepository<TeacherVerificationRequest>().InsertAsync(ver);

            
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
            if (ver == null) throw new Exception("Không tìm thấy yêu cầu xác minh.");
            return Map(ver);
        }

        public async Task<IEnumerable<TeacherVerificationResponse>> GetAll(string? search, int pageNumber, int pageSize, Guid? teacherProfileId, Guid? centerProfileId, VerificationStatus? status)
        {
            var q = _unitOfWork.GetRepository<TeacherVerificationRequest>().Entities
                .Include(x => x.TeacherProfile)
                .Where(x => !x.IsDeleted);
            if (teacherProfileId.HasValue) q = q.Where(x => x.TeacherProfileId == teacherProfileId);
            if (centerProfileId.HasValue)
            {
                q = q.Where(x => x.TeacherProfile != null && x.TeacherProfile.CenterProfileId == centerProfileId && !x.TeacherProfile.IsDeleted);
            }
            if (status.HasValue) q = q.Where(x => x.Status == status);
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

        public async Task<IEnumerable<TeacherVerificationResponse>> GetByCenter(Guid centerProfileId, int pageNumber, int pageSize, Guid? teacherProfileId, VerificationStatus? status)
        {
            var q = _unitOfWork.GetRepository<TeacherVerificationRequest>().Entities
                .Include(x => x.TeacherProfile)
                .Where(x => !x.IsDeleted && x.TeacherProfile != null && x.TeacherProfile.CenterProfileId == centerProfileId && !x.TeacherProfile.IsDeleted);
            if (teacherProfileId.HasValue) q = q.Where(x => x.TeacherProfileId == teacherProfileId);
            if (status.HasValue) q = q.Where(x => x.Status == status);
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
            if (ver == null) throw new Exception("Yêu cầu xác minh không tồn tại.");

            if (request.QualificationCertificatePath != null) ver.QualificationCertificatePath = request.QualificationCertificatePath;
            if (request.EmploymentContractPath != null) ver.EmploymentContractPath = request.EmploymentContractPath;
            if (request.ApprovalFromCenterPath != null) ver.ApprovalFromCenterPath = request.ApprovalFromCenterPath;
            if (request.OtherDocumentsPath != null) ver.OtherDocumentsPath = request.OtherDocumentsPath;
            ver.Status = VerificationStatus.InProgress;

            var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(t => t.Id == ver.TeacherProfileId && !t.IsDeleted);
            if(teacher != null)
            {
                teacher.VerificationStatus = ver.Status;
                teacher.LastUpdatedAt = DateTime.UtcNow;
                await _unitOfWork.GetRepository<TeacherProfile>().UpdateAsync(teacher);
            }

            await _unitOfWork.GetRepository<TeacherVerificationRequest>().UpdateAsync(ver);
            await _unitOfWork.SaveAsync();
            return Map(ver);
        }

        public async Task<TeacherVerificationResponse> SetStatus(Guid id, SetTeacherVerificationStatusDto request)
        {
            var ver = await _unitOfWork.GetRepository<TeacherVerificationRequest>().Entities.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
            if (ver == null) throw new Exception("Yêu cầu xác minh không tồn tại.");

            var verifier = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(v => v.Id == request.VerifierId && !v.IsDeleted);

            ver.Status = request.Status;
            ver.Notes = request.Notes ?? ver.Notes;
            if(verifier.Role == UserRole.Admin)
            {
                ver.AdminId = verifier.Id;
                ver.InspectorId = null;
            }else if(verifier.Role == UserRole.Inspector)
            {
                ver.InspectorId = verifier.Id;
                ver.AdminId = null;
            }

            if (request.Status == VerificationStatus.Completed || request.Status == VerificationStatus.Finalized)
            {
                ver.CompletedAt = DateTime.UtcNow;
            }

            await _unitOfWork.GetRepository<TeacherVerificationRequest>().UpdateAsync(ver);

           
            var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(t => t.Id == ver.TeacherProfileId && !t.IsDeleted && t.VerificationStatus == VerificationStatus.InProgress);
            if (teacher != null)
            {
                teacher.VerificationStatus = request.Status;
                teacher.VerificationCompletedAt = (request.Status == VerificationStatus.Completed || request.Status == VerificationStatus.Finalized) ? DateTime.UtcNow : teacher.VerificationCompletedAt;
                teacher.VerificationNotes = request.Notes ?? teacher.VerificationNotes;
                await _unitOfWork.GetRepository<TeacherProfile>().UpdateAsync(teacher);

               
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



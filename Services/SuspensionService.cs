using BusinessObjects;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.DTO.Suspension;
using Services.Interfaces;

namespace Services
{
    public class SuspensionService : ISuspensionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SuspensionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> BanUser(Guid userId, Guid supervisorId, BanRequest record)
        {
            if (userId == supervisorId) return false;

            var userRepo = _unitOfWork.GetRepository<User>();
            var suspensionRepo = _unitOfWork.GetRepository<SuspensionRecord>();

            var user = await userRepo.Entities
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted && u.Status != AccountStatus.Suspended);

            var supervisor = await userRepo.Entities
                .FirstOrDefaultAsync(u => u.Id == supervisorId && u.Role == UserRole.Admin && !u.IsDeleted);

            if (user == null || supervisor == null) return false;

            var ban = new SuspensionRecord
            {
                UserId = userId,
                Reason = record.Reason,
                SuspendedFrom = record.SuspendedFrom,
                SuspendedTo = record.SuspendedTo,
                ActionByUserId = supervisor.Id
            };

            user.Status = AccountStatus.Suspended;

            await userRepo.UpdateAsync(user);
            await suspensionRepo.InsertAsync(ban);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> BanCourse(Guid courseId, Guid supervisorId, BanRequest record)
        {
            if (courseId == supervisorId) return false;

            var course = await _unitOfWork.GetRepository<Course>().Entities
                .FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted && c.Status != CourseStatus.Suspended);

            var supervisor = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == supervisorId && u.Role == UserRole.Admin && !u.IsDeleted);

            if (course == null || supervisor == null) return false;

            var ban = new SuspensionRecord
            {
                CourseId = course.Id,
                Reason = record.Reason,
                SuspendedFrom = record.SuspendedFrom,
                SuspendedTo = record.SuspendedTo,
                ActionByUserId = supervisor.Id
            };

            course.Status = CourseStatus.Suspended;

            await _unitOfWork.GetRepository<Course>().UpdateAsync(course);
            await _unitOfWork.GetRepository<SuspensionRecord>().InsertAsync(ban);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<(IEnumerable<UserSuspensionRecordResponse> Records, int TotalCount)> GetSuspensionRecordsAsync(
    int pageNumber, int pageSize, string? search = null)
        {
            var suspensionRepo = _unitOfWork.GetRepository<SuspensionRecord>().Entities;
            var userRepo = _unitOfWork.GetRepository<User>().Entities;

            var query = from s in suspensionRepo
                        join bannedUser in userRepo on s.UserId equals bannedUser.Id into bannedUserGroup
                        from bannedUser in bannedUserGroup.DefaultIfEmpty()
                        join admin in userRepo on s.ActionByUserId equals admin.Id into adminGroup
                        from admin in adminGroup.DefaultIfEmpty()
                        where !s.IsDeleted
                        select new { s, bannedUser, admin };

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.bannedUser.FullName, $"%{search}%") ||
                    EF.Functions.Like(x.admin.FullName, $"%{search}%"));
            }

            var totalCount = await query.CountAsync();

            var records = await query
                .OrderByDescending(x => x.s.SuspendedFrom)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new UserSuspensionRecordResponse
                {
                    Id = x.s.Id,
                    BannedUserFullName = x.bannedUser != null ? x.bannedUser.FullName : "(Unknown User)",
                    AdminFullName = x.admin != null ? x.admin.FullName : "(Unknown Admin)",
                    Reason = x.s.Reason,
                    SuspendedFrom = x.s.SuspendedFrom,
                    SuspendedTo = x.s.SuspendedTo,
                    CreatedAt = x.s.CreatedAt,
                    LastUpdatedAt = (DateTime)x.s.LastUpdatedAt
                })
                .ToListAsync();

            return (records, totalCount);
        }

        public async Task<PagedResult<CourseSuspensionRecordResponse>> GetAllCourseSuspensionRecordsAsync(
            int pageNumber = 1,
            int pageSize = 5,
            string? searchKeyword = null)
        {
            var suspensions = from s in _unitOfWork.GetRepository<SuspensionRecord>().Entities
                              join c in _unitOfWork.GetRepository<Course>().Entities on s.CourseId equals c.Id into courseGroup
                              from c in courseGroup.DefaultIfEmpty()

                              join tp in _unitOfWork.GetRepository<TeacherProfile>().Entities on c.TeacherProfileId equals tp.Id into teacherGroup
                              from tp in teacherGroup.DefaultIfEmpty()

                              join tu in _unitOfWork.GetRepository<User>().Entities on tp.UserId equals tu.Id into teacherUserGroup
                              from tu in teacherUserGroup.DefaultIfEmpty()

                              join admin in _unitOfWork.GetRepository<User>().Entities on s.ActionByUserId equals admin.Id into adminGroup
                              from admin in adminGroup.DefaultIfEmpty()

                              where s.CourseId != null && !s.IsDeleted
                              select new CourseSuspensionRecordResponse
                              {
                                  SuspensionId = s.Id,
                                  CourseTitle = c.Title,
                                  Subject = c.Subject,
                                  TeacherName = tu != null ? tu.FullName : "(Unknown Teacher)",
                                  AdminName = admin != null ? admin.FullName : "(Unknown Admin)",
                                  Reason = s.Reason,
                                  CreatedAt = s.CreatedAt,
                                  LastUpdatedAt = (DateTime)s.LastUpdatedAt
                              };

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                string keyword = searchKeyword.ToLower();
                suspensions = suspensions.Where(x =>
                    x.CourseTitle.ToLower().Contains(keyword) ||
                    x.Subject.ToLower().Contains(keyword) ||
                    x.TeacherName.ToLower().Contains(keyword) ||
                    x.AdminName.ToLower().Contains(keyword)
                );
            }

            var totalCount = await suspensions.CountAsync();

            var items = await suspensions
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CourseSuspensionRecordResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<SuspensionRecordResponse> GetRecordById(Guid Id)
        {
            var record = await _unitOfWork.GetRepository<SuspensionRecord>().Entities
                .Include(u => u.User)
                .Include(c => c.Course)
                .Include(a => a.ActionByUser)
                .FirstOrDefaultAsync(r => r.Id == Id && !r.IsDeleted);

            if (record == null) throw new Exception("Record not found.");

            if (record != null && record.UserId != null)
            {
                var response = new SuspensionRecordResponse
                {
                    Id = record.Id,
                    BannedId = record.User.Id,
                    Type = "User",
                    BanBy = record.ActionByUser.FullName,
                    SuspendedFrom = record.SuspendedFrom,
                    SuspendedTo = record.SuspendedTo,
                    Reason = record.Reason,
                    CreatedAt = record.CreatedAt,
                    LastUpdatedAt = record.LastUpdatedAt
                };

                return response;
            }
            else
            {
                var response = new SuspensionRecordResponse
                {
                    Id = record.Id,
                    BannedId = record.Course.Id,
                    Type = "Course",
                    BanBy = record.ActionByUser.FullName,
                    SuspendedFrom = record.SuspendedFrom,
                    SuspendedTo = record.SuspendedTo,
                    Reason = record.Reason,
                    CreatedAt = record.CreatedAt,
                    LastUpdatedAt = record.LastUpdatedAt
                };

                return response;
            }

        }

        public async Task<bool> RemoveBan(Guid suspensionRecordId, Guid moderatorId)
        {
            var record = await _unitOfWork.GetRepository<SuspensionRecord>().Entities
                .Include(u => u.User)
                .Include(c => c.Course)
                .Include(a => a.ActionByUser)
                .FirstOrDefaultAsync(r => r.Id == suspensionRecordId && !r.IsDeleted);

            if (record == null) return false;

            var mod = await _unitOfWork.GetRepository<User>().Entities
                    .FirstOrDefaultAsync(u => u.Id == moderatorId && u.Role == UserRole.Admin && u.Status == AccountStatus.Active && !u.IsDeleted);
            if (mod == null) return false;

            if (record.UserId != null && record.CourseId == null)
            {
                var user = await _unitOfWork.GetRepository<User>().Entities
                    .FirstOrDefaultAsync(u => u.Id == record.UserId && u.Status == AccountStatus.Suspended && !u.IsDeleted);
                if (user == null) return false;

                user.Status = AccountStatus.Active;
                record.LastUpdatedAt = DateTime.UtcNow;
                record.IsDeleted = true;

                await _unitOfWork.GetRepository<User>().UpdateAsync(user);
                await _unitOfWork.GetRepository<SuspensionRecord>().UpdateAsync(record);
                await _unitOfWork.SaveAsync();

                return true;
            }

            if (record.UserId == null && record.CourseId != null)
            {
                var course = await _unitOfWork.GetRepository<Course>().Entities
                    .FirstOrDefaultAsync(u => u.Id == record.CourseId && u.Status == CourseStatus.Suspended && !u.IsDeleted);
                if (course == null) return false;

                course.Status = CourseStatus.Approved;
                record.LastUpdatedAt = DateTime.UtcNow;
                record.IsDeleted = true;

                await _unitOfWork.GetRepository<Course>().UpdateAsync(course);
                await _unitOfWork.GetRepository<SuspensionRecord>().UpdateAsync(record);
                await _unitOfWork.SaveAsync();

                return true;
            }

            return false;
        }

        public async Task<SuspensionRecordResponse?> UpdateSuspensionRecord(Guid suspensionRecordId, UpdateSuspensionRecordRequest request)
        {
            var record = await _unitOfWork.GetRepository<SuspensionRecord>().Entities
                .FirstOrDefaultAsync(r => r.Id == suspensionRecordId && !r.IsDeleted);
            if (record == null) return null;

            if (!string.IsNullOrEmpty(request.Reason))
            {
                record.Reason = request.Reason;
            }

            if (request.SuspendedTo == null)
            {
                record.SuspendedTo = record.SuspendedTo;
            }
            else
            {
                record.SuspendedTo = request.SuspendedTo;
            }

            record.LastUpdatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<SuspensionRecord>().UpdateAsync(record);
            await _unitOfWork.SaveAsync();

            return await GetRecordById(suspensionRecordId);
        }

        public class PagedResult<T>
        {
            public IEnumerable<T> Items { get; set; } = new List<T>();
            public int TotalCount { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
        }
    }
}

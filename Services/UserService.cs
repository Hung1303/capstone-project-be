using BusinessObjects;
using Services.DTO.User;
using Core.Base;
using Core.Security;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.Interfaces;
using System.Text.RegularExpressions;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<CreateAdminRequest> CreateAdminRequest(CreateAdminRequest request)
        {
            var namePattern = @"^([A-ZÀ-Ỹ][a-zà-ỹ]+)(\s[A-ZÀ-Ỹ][a-zà-ỹ]+)*$";
            if (string.IsNullOrWhiteSpace(request.FullName) || !Regex.IsMatch(request.FullName.Trim(), namePattern))
                throw new Exception("Each word in the full name must start with an uppercase letter and contain only letters.");

            var checkUser = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Email == request.Email || u.UserName == request.UserName || u.PhoneNumber == request.PhoneNumber);
            if (checkUser != null)
            {
                if (checkUser.Email == request.Email)
                    throw new Exception("Duplicate email");
                else if (checkUser.UserName == request.UserName)
                    throw new Exception("Duplicate Username");
                else
                    throw new Exception("Duplicate Phonenumber");
            }

            var user = new User
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                Role = UserRole.Admin,
                Status = AccountStatus.Active
            };

            await _unitOfWork.GetRepository<User>().InsertAsync(user);
            await _unitOfWork.SaveAsync();

            return request;
        }

        public async Task<CreateAdminRequest> CreateInspectorRequest(CreateAdminRequest request)
        {
            var namePattern = @"^([A-ZÀ-Ỹ][a-zà-ỹ]+)(\s[A-ZÀ-Ỹ][a-zà-ỹ]+)*$";
            if (string.IsNullOrWhiteSpace(request.FullName) || !Regex.IsMatch(request.FullName.Trim(), namePattern))
                throw new Exception("Each word in the full name must start with an uppercase letter and contain only letters.");

            var checkUser = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Email == request.Email || u.UserName == request.UserName || u.PhoneNumber == request.PhoneNumber);
            if (checkUser != null)
            {
                if (checkUser.Email == request.Email)
                    throw new Exception("Duplicate email");
                else if (checkUser.UserName == request.UserName)
                    throw new Exception("Duplicate Username");
                else
                    throw new Exception("Duplicate Phonenumber");
            }

            var user = new User
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                Role = UserRole.Inspector,
                Status = AccountStatus.Active
            };

            await _unitOfWork.GetRepository<User>().InsertAsync(user);
            await _unitOfWork.SaveAsync();

            return request;
        }

        public async Task<CreateCenterRequest> CreateCenterRequest(CreateCenterRequest request)
        {

            try
            {
                var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(request.Email, emailPattern))
                    throw new Exception("Invalid Email.");

                var namePattern = @"^([A-ZÀ-Ỹ][a-zà-ỹ]+)(\s[A-ZÀ-Ỹ][a-zà-ỹ]+)*$";
                if (string.IsNullOrWhiteSpace(request.FullName) || !Regex.IsMatch(request.FullName.Trim(), namePattern))
                    throw new Exception("Each word in the full name must start with an uppercase letter and contain only letters.");

                var checkUser = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Email == request.Email || u.UserName == request.UserName || u.PhoneNumber == request.PhoneNumber);
                if (checkUser != null)
                {
                    if (checkUser.Email == request.Email)
                        throw new Exception("Duplicate email");
                    else if (checkUser.UserName == request.UserName)
                        throw new Exception("Duplicate Username");
                    else
                        throw new Exception("Duplicate Phonenumber");
                }

                var user = new User
                {
                    Email = request.Email,
                    UserName = request.UserName,
                    FullName = request.FullName,
                    PasswordHash = PasswordHasher.HashPassword(request.Password),
                    PhoneNumber = request.PhoneNumber,
                    Role = UserRole.Center,
                    Status = AccountStatus.Pending
                };

                var center = new CenterProfile
                {
                    UserId = user.Id,
                    CenterName = request.CenterName,
                    OwnerName = request.FullName,
                    LicenseNumber = request.LicenseNumber,
                    IssueDate = request.IssueDate,
                    LicenseIssuedBy = request.LicenseIssuedBy,
                    Address = request.Address,
                    ContactEmail = request.Email,
                    ContactPhone = request.PhoneNumber,
                    Status = CenterStatus.Pending
                };

                await _unitOfWork.GetRepository<User>().InsertAsync(user);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.GetRepository<CenterProfile>().InsertAsync(center);
                await _unitOfWork.SaveAsync();
            }
            catch
            {
                throw;
            }

            return request;
        }

        public async Task<CreateParentRequest> CreateParentRequest(CreateParentRequest request)
        {
            var namePattern = @"^([A-ZÀ-Ỹ][a-zà-ỹ]+)(\s[A-ZÀ-Ỹ][a-zà-ỹ]+)*$";
            if (string.IsNullOrWhiteSpace(request.FullName) || !Regex.IsMatch(request.FullName.Trim(), namePattern))
                throw new Exception("Each word in the full name must start with an uppercase letter and contain only letters.");

            var checkUser = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Email == request.Email || u.UserName == request.UserName || u.PhoneNumber == request.PhoneNumber);
            if (checkUser != null)
            {
                if (checkUser.Email == request.Email)
                    throw new Exception("Duplicate email");
                else if (checkUser.UserName == request.UserName)
                    throw new Exception("Duplicate Username");
                else
                    throw new Exception("Duplicate Phonenumber");
            }

            if (request.PhoneSecondary == request.PhoneNumber)
                throw new InvalidOperationException("Cannot use main phone number as secondary.");

            var user = new User
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                Role = UserRole.Parent,
                Status = AccountStatus.Pending
            };

            var parent = new ParentProfile
            {
                UserId = user.Id,
                Address = request.Address,
                PhoneSecondary = request.PhoneSecondary
            };

            await _unitOfWork.GetRepository<User>().InsertAsync(user);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.GetRepository<ParentProfile>().InsertAsync(parent);
            await _unitOfWork.SaveAsync();

            return request;
        }

        public async Task<CreateStudentRequest> CreateStudentRequest(Guid parentId, CreateStudentRequest request)
        {
            var parent = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(o => o.Id == parentId && o.Role == UserRole.Parent && o.Status == AccountStatus.Active);
            if (parent == null)
                throw new Exception("Parent not found");

            var parentProfile = await _unitOfWork.GetRepository<ParentProfile>().Entities
                .FirstOrDefaultAsync(c => c.UserId == parentId);
            if (parentProfile == null)
                throw new Exception("Parent Profile not found.");

            // ✅ Validate FullName format
            var namePattern = @"^([A-ZÀ-Ỹ][a-zà-ỹ]+)(\s[A-ZÀ-Ỹ][a-zà-ỹ]+)*$";
            if (string.IsNullOrWhiteSpace(request.FullName) || !Regex.IsMatch(request.FullName.Trim(), namePattern))
                throw new Exception("Each word in the full name must start with an uppercase letter and contain only letters.");

            // ✅ Validate SchoolYear format (e.g., "2024-2025")
            var yearRangePattern = @"^(\d{4})-(\d{4})$";
            if (string.IsNullOrWhiteSpace(request.SchoolYear) || !Regex.IsMatch(request.SchoolYear.Trim(), yearRangePattern))
                throw new Exception("School year must follow the format 'YYYY-YYYY' (e.g., 2024-2025).");

            var match = Regex.Match(request.SchoolYear.Trim(), yearRangePattern);
            int startYear = int.Parse(match.Groups[1].Value);
            int endYear = int.Parse(match.Groups[2].Value);

            if (endYear != startYear + 1)
                throw new Exception("The second year in SchoolYear must be exactly one greater than the first (e.g., 2024-2025).");

            // ✅ Check duplicates
            var checkUser = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(u => u.Email == request.Email || u.UserName == request.UserName || u.PhoneNumber == request.PhoneNumber);

            if (checkUser != null)
            {
                if (checkUser.Email == request.Email)
                    throw new Exception("Duplicate email");
                else if (checkUser.UserName == request.UserName)
                    throw new Exception("Duplicate username");
                else
                    throw new Exception("Duplicate phone number");
            }

            // ✅ Create User
            var user = new User
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                Role = UserRole.Student,
                Status = AccountStatus.Pending
            };

            // ✅ Create Student Profile
            var student = new StudentProfile
            {
                UserId = user.Id,
                SchoolName = request.SchoolName,
                SchoolYear = request.SchoolYear.Trim(),
                GradeLevel = request.GradeLevel,
                ParentProfileId = parentProfile.Id
            };

            await _unitOfWork.GetRepository<User>().InsertAsync(user);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.GetRepository<StudentProfile>().InsertAsync(student);
            await _unitOfWork.SaveAsync();

            return request;
        }

        public async Task<CreateTeacherRequest> CreateTeacherRequest(CreateTeacherRequest request)
        {
            var namePattern = @"^([A-ZÀ-Ỹ][a-zà-ỹ]+)(\s[A-ZÀ-Ỹ][a-zà-ỹ]+)*$";
            if (string.IsNullOrWhiteSpace(request.FullName) || !Regex.IsMatch(request.FullName.Trim(), namePattern))
                throw new Exception("Each word in the full name must start with an uppercase letter and contain only letters.");

            var checkUser = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Email == request.Email || u.UserName == request.UserName || u.PhoneNumber == request.PhoneNumber);
            if (checkUser != null)
            {
                if (checkUser.Email == request.Email)
                    throw new Exception("Duplicate email");
                else if (checkUser.UserName == request.UserName)
                    throw new Exception("Duplicate Username");
                else
                    throw new Exception("Duplicate Phonenumber");
            }

            var user = new User
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                Role = UserRole.Teacher,
                Status = AccountStatus.Pending
            };

            var teacher = new TeacherProfile
            {
                UserId = user.Id,
                YearOfExperience = request.YearOfExperience,
                Qualifications = request.Qualifications,
                LicenseNumber = request.LicenseNumber,
                Subjects = request.Subjects,
                Bio = request.Bio
            };

            await _unitOfWork.GetRepository<User>().InsertAsync(user);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.GetRepository<TeacherProfile>().InsertAsync(teacher);
            await _unitOfWork.SaveAsync();

            return request;
        }

        public async Task<CreateTeacherRequest> CenterAddTeacherRequest(Guid centerOwnerId, CreateTeacherRequest request)
        {
            var owner = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(o => o.Id == centerOwnerId && o.Role == UserRole.Center && o.Status == AccountStatus.Active);
            if (owner == null)
                throw new Exception("Center not available");

            var center = await _unitOfWork.GetRepository<CenterProfile>().Entities.FirstOrDefaultAsync(c => c.UserId == centerOwnerId);
            if (center == null)
            {
                throw new Exception("Center not found.");
            }

            var namePattern = @"^([A-ZÀ-Ỹ][a-zà-ỹ]+)(\s[A-ZÀ-Ỹ][a-zà-ỹ]+)*$";
            if (string.IsNullOrWhiteSpace(request.FullName) || !Regex.IsMatch(request.FullName.Trim(), namePattern))
                throw new Exception("Each word in the full name must start with an uppercase letter and contain only letters.");

            var checkUser = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Email == request.Email || u.UserName == request.UserName || u.PhoneNumber == request.PhoneNumber);
            if (checkUser != null)
            {
                if (checkUser.Email == request.Email)
                    throw new Exception("Duplicate email");
                else if (checkUser.UserName == request.UserName)
                    throw new Exception("Duplicate Username");
                else
                    throw new Exception("Duplicate Phonenumber");
            }

            var user = new User
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                Role = UserRole.Teacher,
                Status = AccountStatus.Pending
            };

            var teacher = new TeacherProfile
            {
                UserId = user.Id,
                YearOfExperience = request.YearOfExperience,
                Qualifications = request.Qualifications,
                LicenseNumber = request.LicenseNumber,
                Subjects = request.Subjects,
                Bio = request.Bio,
                CenterProfileId = center.Id
            };

            await _unitOfWork.GetRepository<User>().InsertAsync(user);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.GetRepository<TeacherProfile>().InsertAsync(teacher);
            await _unitOfWork.SaveAsync();

            return request;
        }

        public async Task<(IEnumerable<UserSummaryDto> Users, int TotalCount)> GetAllUsersAsync(int pageNumber, int pageSize, string? fullName = null)
        {
            IQueryable<User> query = _unitOfWork.GetRepository<User>().Entities;

            // Optional search by FullName
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                query = query.Where(u => EF.Functions.Like(u.FullName, $"%{fullName}%"));
            }

            int totalCount = await query.CountAsync();

            // Query with projection into DTO
            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserSummaryDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role.ToString(),
                    Status = u.Status.ToString(),
                    CreatedAt = u.CreatedAt,
                    ProfileType =
                        u.TeacherProfile != null ? "Teacher" :
                        u.CenterProfile != null ? "Center" :
                        u.StudentProfile != null ? "Student" :
                        u.ParentProfile != null ? "Parent" :
                        "None"
                })
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<UserDetailResponse?> GetUserByIdAsync(Guid userId)
        {
            var query = _unitOfWork.GetRepository<User>().Entities
                .Include(c => c.CenterProfile)
                .Include(t => t.TeacherProfile)
                .Include(p => p.ParentProfile)
                .Include(s => s.StudentProfile)
                .Where(u => u.Id == userId && !u.IsDeleted)
                .Select(u => new UserDetailResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role.ToString(),
                    Status = u.Status.ToString(),
                    CenterProfileId = u.CenterProfile != null ? u.CenterProfile.Id : (Guid?)null,
                    TeacherProfileId = u.TeacherProfile != null ? u.TeacherProfile.Id : (Guid?)null,
                    ParentProfileId = u.ParentProfile != null ? u.ParentProfile.Id : (Guid?)null,
                    StudentProfileId = u.StudentProfile != null ? u.StudentProfile.Id : (Guid?)null
                });

            return await query.FirstOrDefaultAsync();
        }

        public async Task<CenterDetailRespone?> UpdateCenterAsync(Guid userId, CenterUpdateRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var owner = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(o => o.Id == userId && o.Status == AccountStatus.Active && o.Role == UserRole.Center && !o.IsDeleted);
            if (owner == null)
            {
                return null;
            }

            var center = await _unitOfWork.GetRepository<CenterProfile>().Entities
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (center == null)
            {
                return null;
            }

            bool emailExists = await _unitOfWork.GetRepository<User>().Entities
                .AnyAsync(u => u.Email == request.Email && u.Id != userId && !u.IsDeleted);
            if (emailExists)
                throw new InvalidOperationException("Email is already in use by another user.");

            bool phoneExists = await _unitOfWork.GetRepository<User>().Entities
                .AnyAsync(u => u.PhoneNumber == request.PhoneNumber && u.Id != userId && !u.IsDeleted);
            if (phoneExists)
                throw new InvalidOperationException("Phone number is already in use by another user.");

            owner.Email = request.Email;
            owner.PhoneNumber = request.PhoneNumber;

            center.Address = request.Address;
            center.ContactEmail = request.Email;
            center.ContactPhone = request.PhoneNumber;

            await _unitOfWork.GetRepository<User>().UpdateAsync(owner);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.GetRepository<CenterProfile>().UpdateAsync(center);
            await _unitOfWork.SaveAsync();

            return await GetCenterById(userId);
        }

        public async Task<TeacherDetailResponse?> UpdateTeacherAsynce(Guid userId, TeacherUpdateRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var user = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(o => o.Id == userId && o.Status == AccountStatus.Active && o.Role == UserRole.Teacher && !o.IsDeleted);
            if (user == null)
            {
                return null;
            }

            var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (teacher == null)
            {
                return null;
            }

            bool emailExists = await _unitOfWork.GetRepository<User>().Entities
                .AnyAsync(u => u.Email == request.Email && u.Id != userId && !u.IsDeleted);
            if (emailExists)
                throw new InvalidOperationException("Email is already in use by another user.");

            bool phoneExists = await _unitOfWork.GetRepository<User>().Entities
                .AnyAsync(u => u.PhoneNumber == request.PhoneNumber && u.Id != userId && !u.IsDeleted);
            if (phoneExists)
                throw new InvalidOperationException("Phone number is already in use by another user.");

            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;

            teacher.Subjects = request.Subjects;
            teacher.Bio = request.Bio;

            await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.GetRepository<TeacherProfile>().UpdateAsync(teacher);
            await _unitOfWork.SaveAsync();

            return await GetTeacherById(userId);
        }

        public async Task<ParentDetailResponse?> UpdateParentAsynce(Guid userId, ParentUpdateRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var user = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(o => o.Id == userId && o.Status == AccountStatus.Active && o.Role == UserRole.Parent && !o.IsDeleted);
            if (user == null)
            {
                return null;
            }

            var parent = await _unitOfWork.GetRepository<ParentProfile>().Entities
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (parent == null)
            {
                return null;
            }

            bool emailExists = await _unitOfWork.GetRepository<User>().Entities
                .AnyAsync(u => u.Email == request.Email && u.Id != userId && !u.IsDeleted);
            if (emailExists)
                throw new InvalidOperationException("Email is already in use by another user.");

            bool phoneExists = await _unitOfWork.GetRepository<User>().Entities
                .AnyAsync(u => u.PhoneNumber == request.PhoneNumber && u.Id != userId && !u.IsDeleted);
            if (phoneExists)
                throw new InvalidOperationException("Phone number is already in use by another user.");

            if (request.PhoneSecondary == user.PhoneNumber)
                throw new InvalidOperationException("Cannot use main phone number as secondary.");

            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;

            parent.Address = request.Address;
            parent.PhoneSecondary = request.PhoneSecondary;

            await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.GetRepository<ParentProfile>().UpdateAsync(parent);
            await _unitOfWork.SaveAsync();

            return await GetParentById(userId);
        }

        public async Task<StudentDetailResponse?> UpdateStudentAsynce(Guid userId, StudentUpdateRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var user = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(o => o.Id == userId && o.Status == AccountStatus.Active && o.Role == UserRole.Student && !o.IsDeleted);
            if (user == null)
            {
                return null;
            }

            var student = await _unitOfWork.GetRepository<StudentProfile>().Entities
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (student == null)
            {
                return null;
            }

            bool emailExists = await _unitOfWork.GetRepository<User>().Entities
                .AnyAsync(u => u.Email == request.Email && u.Id != userId && !u.IsDeleted);
            if (emailExists)
                throw new InvalidOperationException("Email is already in use by another user.");

            bool phoneExists = await _unitOfWork.GetRepository<User>().Entities
                .AnyAsync(u => u.PhoneNumber == request.PhoneNumber && u.Id != userId && !u.IsDeleted);
            if (phoneExists)
                throw new InvalidOperationException("Phone number is already in use by another user.");

            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;

            await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return await GetStudentById(userId);
        }

        public async Task<(IEnumerable<CenterListResponse> Centers, int TotalCount)> GetAllCentersAsync(int pageNumber, int pageSize, string? centerName = null)
        {
            var centerRepo = _unitOfWork.GetRepository<CenterProfile>().Entities;
            var userRepo = _unitOfWork.GetRepository<User>().Entities;

            // Build base query with join to User
            var query = from c in centerRepo
                        join u in userRepo on c.UserId equals u.Id
                        where !c.IsDeleted && !u.IsDeleted
                        select new { Center = c, User = u };

            // Optional search by center name
            if (!string.IsNullOrWhiteSpace(centerName))
            {
                query = query.Where(x => EF.Functions.Like(x.Center.CenterName, $"%{centerName}%"));
            }

            int totalCount = await query.CountAsync();

            // Projection to DTO
            var centers = await query
                .OrderByDescending(x => x.Center.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CenterListResponse
                {
                    Id = x.Center.Id,
                    UserId = x.Center.UserId,
                    CenterProfileId = x.Center.Id,
                    CenterName = x.Center.CenterName,
                    OwnerName = x.Center.OwnerName,
                    LicenseNumber = x.Center.LicenseNumber,
                    Address = x.Center.Address,
                    ContactEmail = x.Center.ContactEmail,
                    ContactPhone = x.Center.ContactPhone,
                    Status = x.User.Status.ToString()
                })
                .ToListAsync();

            return (centers, totalCount);
        }

        public async Task<(IEnumerable<TeacherListResponse> Teachers, int TotalCount)> GetAllTeachersAsync(int pageNumber, int pageSize, string? fullName = null)
        {
            var teacherRepo = _unitOfWork.GetRepository<TeacherProfile>().Entities;
            var userRepo = _unitOfWork.GetRepository<User>().Entities;

            var query = from t in teacherRepo
                        join u in userRepo on t.UserId equals u.Id
                        where !t.IsDeleted && !u.IsDeleted
                        select new { Teacher = t, User = u };

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                query = query.Where(x => EF.Functions.Like(x.User.FullName, $"%{fullName}%"));
            }

            int totalCount = await query.CountAsync();

            var teachers = await query
                .OrderByDescending(x => x.User.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TeacherListResponse
                {
                    Id = x.Teacher.Id,
                    FullName = x.User.FullName,
                    YearOfExperience = x.Teacher.YearOfExperience,
                    Qualification = x.Teacher.Qualifications,
                    Subject = x.Teacher.Subjects,
                    Status = x.User.Status.ToString()
                })
                .ToListAsync();

            return (teachers, totalCount);
        }

        public async Task<(IEnumerable<ParentListResponse> Parents, int TotalCount)> GetAllParentsAsync(int pageNumber, int pageSize, string? fullName = null)
        {
            var parentRepo = _unitOfWork.GetRepository<ParentProfile>().Entities;
            var userRepo = _unitOfWork.GetRepository<User>().Entities;

            var query = from p in parentRepo
                        join u in userRepo on p.UserId equals u.Id
                        where !p.IsDeleted && !u.IsDeleted
                        select new { Parent = p, User = u };

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                query = query.Where(x => EF.Functions.Like(x.User.FullName, $"%{fullName}%"));
            }

            int totalCount = await query.CountAsync();

            var parents = await query
                .OrderByDescending(x => x.User.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ParentListResponse
                {
                    Id = x.Parent.Id,
                    FullName = x.User.FullName,
                    Email = x.User.Email,
                    PhoneNumber = x.User.PhoneNumber,
                    Address = x.Parent.Address,
                    PhoneSecondary = x.Parent.PhoneSecondary,
                    Status = x.User.Status.ToString()
                })
                .ToListAsync();

            return (parents, totalCount);
        }

        public async Task<(IEnumerable<StudentListResponse> Students, int TotalCount)> GetAllStudentsAsync(int pageNumber, int pageSize, string? fullName = null)
        {
            var studentRepo = _unitOfWork.GetRepository<StudentProfile>().Entities;
            var userRepo = _unitOfWork.GetRepository<User>().Entities;

            var query = from s in studentRepo
                        join u in userRepo on s.UserId equals u.Id
                        where !s.IsDeleted && !u.IsDeleted
                        select new { Student = s, User = u };

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                query = query.Where(x => EF.Functions.Like(x.User.FullName, $"%{fullName}%"));
            }

            int totalCount = await query.CountAsync();

            var students = await query
                .OrderByDescending(x => x.User.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new StudentListResponse
                {
                    Id = x.Student.Id,
                    FullName = x.User.FullName,
                    Email = x.User.Email,
                    PhoneNumber = x.User.PhoneNumber,
                    SchoolName = x.Student.SchoolName,
                    SchoolYear = x.Student.SchoolYear,
                    GradeLevel = x.Student.GradeLevel,
                    Status = x.User.Status.ToString()
                })
                .ToListAsync();

            return (students, totalCount);
        }

        public async Task<CenterDetailRespone?> GetCenterById(Guid userId)
        {
            var query = _unitOfWork.GetRepository<User>().Entities
                .Include(c => c.CenterProfile)
                .Where(u => u.Id == userId && u.Role == UserRole.Center && u.Status == AccountStatus.Active && !u.IsDeleted)
                .Select(u => new CenterDetailRespone
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Status = u.Status.ToString(),
                    CenterId = u.CenterProfile.Id,
                    CenterName = u.CenterProfile.CenterName,
                    OwnerName = u.CenterProfile.OwnerName,
                    LicenseNumber = u.CenterProfile.LicenseNumber,
                    LicenseIssuedBy = u.CenterProfile.LicenseIssuedBy,
                    IssueDate = u.CenterProfile.IssueDate,
                    Address = u.CenterProfile.Address,
                    ContactEmail = u.CenterProfile.ContactEmail,
                    ContactPhone = u.CenterProfile.ContactPhone
                });

            return await query.FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<TeacherListResponse> Teachers, int TotalCount)> GetTeachersByCenterIdAsync(
            Guid centerId, int pageNumber, int pageSize, string? fullName = null)
        {
            var teacherRepo = _unitOfWork.GetRepository<TeacherProfile>().Entities;
            var userRepo = _unitOfWork.GetRepository<User>().Entities;

            // Join teachers and users, filter by centerId
            var query = from t in teacherRepo
                        join u in userRepo on t.UserId equals u.Id
                        where !t.IsDeleted && !u.IsDeleted && t.CenterProfileId == centerId
                        select new { Teacher = t, User = u };

            // Optional search by teacher full name
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                query = query.Where(x => EF.Functions.Like(x.User.FullName, $"%{fullName}%"));
            }

            int totalCount = await query.CountAsync();

            // Paginate + project into DTO
            var teachers = await query
                .OrderByDescending(x => x.User.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TeacherListResponse
                {
                    Id = x.Teacher.Id,
                    FullName = x.User.FullName,
                    YearOfExperience = x.Teacher.YearOfExperience,
                    Qualification = x.Teacher.Qualifications,
                    Subject = x.Teacher.Subjects,
                    Status = x.User.Status.ToString()
                })
                .ToListAsync();

            return (teachers, totalCount);
        }

        public async Task<TeacherDetailResponse?> GetTeacherById(Guid userId)
        {
            var query = _unitOfWork.GetRepository<User>().Entities
                .Include(c => c.TeacherProfile)
                .Where(u => u.Id == userId && u.Role == UserRole.Teacher && u.Status == AccountStatus.Active && !u.IsDeleted)
                .Select(u => new TeacherDetailResponse
                {
                    Id = u.Id,
                    CenterId = u.TeacherProfile.CenterProfileId,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Status = u.Status.ToString(),
                    TeacherId = u.TeacherProfile.Id,
                    YearOfExperience = u.TeacherProfile.YearOfExperience,
                    Qualifications = u.TeacherProfile.Qualifications,
                    LicenseNumber = u.TeacherProfile.LicenseNumber,
                    Subjects = u.TeacherProfile.Subjects,
                    Bio = u.TeacherProfile.Bio
                });

            return await query.FirstOrDefaultAsync();
        }

        public async Task<ParentDetailResponse?> GetParentById(Guid userId)
        {
            var query = _unitOfWork.GetRepository<User>().Entities
                .Include(c => c.ParentProfile)
                .Where(u => u.Id == userId && u.Role == UserRole.Parent && u.Status == AccountStatus.Active && !u.IsDeleted)
                .Select(u => new ParentDetailResponse
                {
                    Id = u.Id,
                    ParentId = u.ParentProfile.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Status = u.Status.ToString(),
                    Address = u.ParentProfile.Address,
                    PhoneSecondary = u.ParentProfile.PhoneSecondary
                });

            return await query.FirstOrDefaultAsync();
        }

        public async Task<StudentDetailResponse?> GetStudentById(Guid userId)
        {
            var query = _unitOfWork.GetRepository<User>().Entities
                .Include(c => c.StudentProfile)
                .Where(u => u.Id == userId && u.Role == UserRole.Student && u.Status == AccountStatus.Active && !u.IsDeleted)
                .Select(u => new StudentDetailResponse
                {
                    Id = u.Id,
                    StudentId = u.StudentProfile.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Status = u.Status.ToString(),
                    SchoolName = u.StudentProfile.SchoolName,
                    SchoolYear = u.StudentProfile.SchoolYear,
                    GradeLevel = u.StudentProfile.GradeLevel
                });

            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            var result = false;
            var user = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null)
                throw new Exception("User not found");

            user.IsDeleted = true;
            result = true;

            await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return result;
        }

        public async Task<bool> UpdateUserStatus(Guid userId, int status)
        {
            var result = false;

            var user = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null) throw new Exception("User not found.");

            if (status < 1 || status > 3) throw new Exception("Must between 1 and 3");

            switch (status)
            {
                case 1:
                    user.Status = AccountStatus.Active; result = true;
                    break;

                case 2:
                    user.Status = AccountStatus.Suspended; result = true;
                    break;

                case 3:
                    user.Status = AccountStatus.Deactivated; result = true;
                    break;
            }

            await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return result;
        }

        public async Task<bool> ChangePassword(Guid userId, string currentPassword, string newPassword)
        {
            var result = false;
            var user = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null) throw new Exception("User not found");

            if (!PasswordHasher.VerifyPassword(currentPassword, user.PasswordHash))
                throw new Exception("Current password is incorrect.");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new Exception("New password must be at least 6 characters long.");

            if (PasswordHasher.VerifyPassword(newPassword, user.PasswordHash))
                throw new Exception("New password cannot be the same as the current password.");

            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            user.LastUpdatedAt = DateTime.UtcNow;

            result = true;
            await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return result;
        }

        public async Task<(IEnumerable<CenterListResponse> Centers, int TotalCount)> GetCentersByStatusAsync(CenterStatus status, int pageNumber, int pageSize, string? centerName = null)
        {
            var query = _unitOfWork.GetRepository<CenterProfile>().Entities
                .Where(c => !c.IsDeleted && c.Status == status);

            if (!string.IsNullOrWhiteSpace(centerName))
            {
                query = query.Where(c => c.CenterName.Contains(centerName));
            }

            var totalCount = await query.CountAsync();

            var centers = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CenterListResponse
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    CenterProfileId = c.Id,
                    CenterName = c.CenterName,
                    OwnerName = c.OwnerName ?? "",
                    LicenseNumber = c.LicenseNumber,
                    Address = c.Address,
                    ContactEmail = c.ContactEmail ?? "",
                    ContactPhone = c.ContactPhone ?? "",
                    Status = c.Status.ToString()
                })
                .ToListAsync();

            return (centers, totalCount);
        }

        public async Task<bool> UpdateCenterStatusAsync(Guid centerId, CenterStatus status, string? reason = null)
        {
            var center = await _unitOfWork.GetRepository<CenterProfile>().Entities
                .FirstOrDefaultAsync(c => c.Id == centerId && !c.IsDeleted);

            if (center == null)
                return false;

            center.Status = status;
            center.LastUpdatedAt = DateTime.UtcNow;

            if (status == CenterStatus.Rejected && !string.IsNullOrEmpty(reason))
            {
                center.RejectionReason = reason;
            }
            else if (status == CenterStatus.Verified)
            {
                center.VerificationCompletedAt = DateTime.UtcNow;
            }

            await _unitOfWork.GetRepository<CenterProfile>().UpdateAsync(center);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}

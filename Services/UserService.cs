using BusinessObjects;
using BusinessObjects.DTO.User;
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
                    ContactPhone = request.PhoneNumber
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
            var parent = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(o => o.Id == parentId && o.Role == UserRole.Parent && o.Status == AccountStatus.Active);
            if (parent == null)
                throw new Exception("Parent not found");

            var parentProfile = await _unitOfWork.GetRepository<ParentProfile>().Entities.FirstOrDefaultAsync(c => c.UserId == parentId);
            if (parentProfile == null)
            {
                throw new Exception("Parent Profile not found.");
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
                Role = UserRole.Student,
                Status = AccountStatus.Pending
            };

            var student = new StudentProfile
            {
                UserId = user.Id,
                SchoolName = request.SchoolName,
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
    }
}

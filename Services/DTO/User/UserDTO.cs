using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace Services.DTO.User
{
    public class CreateAdminRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Sai cú pháp email")]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Tên người dùng phải có ít nhất 6 kí tự")]
        public string UserName { get; set; }
        [Required, MinLength(6, ErrorMessage = "Mật khẩu phải dài ít nhất 6 kí tự")]
        public string Password { get; set; }
        [Required, MinLength(2, ErrorMessage = "Họ và tên phải dài ít nhất 2 kí tự")]
        public string FullName { get; set; }
        [Phone(ErrorMessage = "Sai số điện thoại.")]
        public string PhoneNumber { get; set; }
    }

    public class CreateCenterRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Sai cú pháp email")]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Tên người dùng phải có ít nhất 6 kí tự")]
        public string UserName { get; set; }
        [Required, MinLength(6, ErrorMessage = "Mật khẩu phải dài ít nhất 6 kí tự")]
        public string Password { get; set; }
        [Required, MinLength(2, ErrorMessage = "Họ và tên phải dài ít nhất 2 kí tự")]
        public string FullName { get; set; }
        [Phone(ErrorMessage = "Sai số điện thoại.")]
        public string PhoneNumber { get; set; }
        public string CenterName { get; set; }
        [Phone(ErrorMessage = "Sai số điện thoại.")]
        public string ContactPhone { get; set; } //số điện thoại bàn
        public string LicenseNumber { get; set; }
        public DateOnly IssueDate { get; set; }
        public string LicenseIssuedBy { get; set; }
        public string Address { get; set; }

    }

    public class CreateTeacherRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Sai cú pháp email")]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Tên người dùng phải có ít nhất 6 kí tự")]
        public string UserName { get; set; }
        [Required, MinLength(6, ErrorMessage = "Mật khẩu phải dài ít nhất 6 kí tự")]
        public string Password { get; set; }
        [Required, MinLength(2, ErrorMessage = "Họ và tên phải dài ít nhất 2 kí tự")]
        public string FullName { get; set; }
        [Phone(ErrorMessage = "Sai số điện thoại.")]
        public string PhoneNumber { get; set; }
        [Range(0, 50, ErrorMessage = "Năm kinh nghiệm phải từ 0 đến 50")]
        public int YearOfExperience { get; set; } = 0;
        public string Qualifications { get; set; }
        public string LicenseNumber { get; set; }
        public string Subjects { get; set; }
        public string Bio { get; set; }
        public string? TeachingAtSchool { get; set; }
        public string? TeachAtClasses { get; set; } // list of classes the teacher teaches
    }

    public class CreateParentRequest
    {

        [Required]
        [EmailAddress(ErrorMessage = "Sai cú pháp email")]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Tên người dùng phải có ít nhất 6 kí tự")]
        public string UserName { get; set; }
        [Required, MinLength(6, ErrorMessage = "Mật khẩu phải dài ít nhất 6 kí tự")]
        public string Password { get; set; }
        [Required, MinLength(2, ErrorMessage = "Họ và tên phải dài ít nhất 2 kí tự")]
        public string FullName { get; set; }
        [Phone(ErrorMessage = "Sai số điện thoại.")]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        [Phone(ErrorMessage = "Sai số điện thoại.")]
        public string? PhoneSecondary { get; set; }
    }

    public class CreateStudentRequest
    {

        [Required]
        [EmailAddress(ErrorMessage = "Sai cú pháp email")]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Tên người dùng phải có ít nhất 6 kí tự")]
        public string UserName { get; set; }
        [Required, MinLength(6, ErrorMessage = "Mật khẩu phải dài ít nhất 6 kí tự")]
        public string Password { get; set; }
        [Required, MinLength(2, ErrorMessage = "Họ và tên phải dài ít nhất 2 kí tự")]
        public string FullName { get; set; }
        [Phone(ErrorMessage = "Sai số điện thoại.")]
        public string PhoneNumber { get; set; }
        public string SchoolName { get; set; }
        public string SchoolYear { get; set; }
        [Range(6, 12, ErrorMessage = "Lớp học phải từ lớp 6 đến lớp 12.")]
        public int GradeLevel { get; set; }
        public string ClassName { get; set; }
    }

    public class UserSummaryDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ProfileType { get; set; } = "None";
        public DateTime CreatedAt { get; set; }
    }

    public class CenterUpdateRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Sai cú pháp email")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Sai số điện thoại.")]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }

    public class TeacherUpdateRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Sai cú pháp email")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Sai số điện thoại.")]
        public string PhoneNumber { get; set; }
        public string Subjects { get; set; }
        public string Bio { get; set; }
    }

    public class ParentUpdateRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Sai cú pháp email")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Sai số điện thoại.")]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        [Phone(ErrorMessage = "Sai số điện thoại.")]
        public string? PhoneSecondary { get; set; }
    }

    public class StudentUpdateRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Sai cú pháp email")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Sai số điện thoại.")]
        public string PhoneNumber { get; set; }
        public string SchoolName { get; set; }
        public string SchoolYear { get; set; }
        [Range(6, 12, ErrorMessage = "Lớp học phải từ lớp 6 đến lớp 12.")]
        public int GradeLevel { get; set; }
        public string ClassName { get; set; }

    }

    public class UserUpdateRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Sai cú pháp email")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Sai số điện thoại.")]
        public string PhoneNumber { get; set; }
    }

    public class CenterListResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CenterProfileId { get; set; }
        public string CenterName { get; set; }
        public string OwnerName { get; set; }
        public string LicenseNumber { get; set; }
        public string Address { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string Status { get; set; }
    }

    public class TeacherListResponse
    {
        public Guid Id { get; set; }
        public Guid ProfileId { get; set; }
        public string FullName { get; set; }
        public int YearOfExperience { get; set; }
        public string Qualification { get; set; }
        public string Subject { get; set; }
        public string? TeachingAtSchool { get; set; }
        public string? TeachAtClasses { get; set; } // list of classes the teacher teaches
        public string Status { get; set; }
    }

    public class ParentListResponse
    {
        public Guid Id { get; set; }
        public Guid ProfileId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string PhoneSecondary { get; set; }
        public string Status { get; set; }
    }

    public class StudentListResponse
    {
        public Guid UserId { get; set; }
        public Guid ProfileId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string SchoolName { get; set; }
        public string SchoolYear { get; set; }
        [Range(6, 12, ErrorMessage = "Lớp học phải từ lớp 6 đến lớp 12.")]
        public int GradeLevel { get; set; }
        public string ClassName { get; set; }
        public string Status { get; set; }
    }

    public class UserDetailResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public Guid? TeacherProfileId { get; set; }
        public Guid? CenterProfileId { get; set; }
        public Guid? StudentProfileId { get; set; }
        public Guid? ParentProfileId { get; set; }
    }

    public class CenterDetailRespone
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }

        public Guid CenterId { get; set; }
        public string CenterName { get; set; }
        public string OwnerName { get; set; }
        public string LicenseNumber { get; set; }
        public DateOnly IssueDate { get; set; }
        public string? LicenseIssuedBy { get; set; }
        public string Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
    }

    public class TeacherDetailResponse
    {
        public Guid Id { get; set; }
        public Guid? CenterId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }

        public Guid TeacherId { get; set; }
        public int YearOfExperience { get; set; }
        public string Qualifications { get; set; }
        public string LicenseNumber { get; set; }
        public string Subjects { get; set; }
        public string? TeachingAtSchool { get; set; }
        public string? TeachAtClasses { get; set; } // list of classes the teacher teaches
        public string? Bio { get; set; }
    }

    public class ParentDetailResponse
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }

        public string Address { get; set; }
        public string PhoneSecondary { get; set; }
    }

    public class StudentDetailResponse
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }

        public string SchoolName { get; set; }
        public string SchoolYear { get; set; }
        [Range(6, 12, ErrorMessage = "Lớp học phải từ lớp 6 đến lớp 12.")]
        public int GradeLevel { get; set; }
        public string ClassName { get; set; }
    }

    public class UpdateCenterStatusRequest
    {
        [Required]
        public CenterStatus Status { get; set; }
        public string? Reason { get; set; }
    }

    public class StudentEnrollInCourseResponse
    {
        public Guid UserId { get; set; }
        public Guid StudentProfileId { get; set; }
        public Guid EnrollmentId { get; set; }
        public Guid CourseId { get; set; }
        public string StudentName { get; set; }
        public string SchoolName { get; set; }
        public string SchoolYear { get; set; }
        public int GradeLevel { get; set; }
        public string ClassName { get; set; }
    }
}

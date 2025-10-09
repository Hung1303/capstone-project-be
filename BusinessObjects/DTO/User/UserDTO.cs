using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTO.User
{
    public class CreateAdminRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Username must be at least 6 characters long")]
        public string UserName { get; set; }
        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }
        [Required, MinLength(2, ErrorMessage = "Fullname must be at least 2 characters long")]
        public string FullName { get; set; }
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
    }

    public class CreateCenterRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Username must be at least 6 characters long")]
        public string UserName { get; set; }
        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }
        [Required, MinLength(2, ErrorMessage = "Fullname must be at least 2 characters long")]
        public string FullName { get; set; }
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
        public string CenterName { get; set; }
        public string LicenseNumber { get; set; }
        public DateOnly IssueDate { get; set; }
        public string LicenseIssuedBy { get; set; }
        public string Address { get; set; }

    }

    public class CreateTeacherRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Username must be at least 6 characters long")]
        public string UserName { get; set; }
        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }
        [Required, MinLength(2, ErrorMessage = "Fullname must be at least 2 characters long")]
        public string FullName { get; set; }
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
        [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50.")]
        public int YearOfExperience { get; set; } = 0;
        public string Qualifications { get; set; }
        public string LicenseNumber { get; set; }
        public string Subjects { get; set; }
        public string Bio { get; set; }
    }

    public class CreateParentRequest
    {

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Username must be at least 6 characters long")]
        public string UserName { get; set; }
        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }
        [Required, MinLength(2, ErrorMessage = "Fullname must be at least 2 characters long")]
        public string FullName { get; set; }
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneSecondary { get; set; }
    }

    public class CreateStudentRequest
    {

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Username must be at least 6 characters long")]
        public string UserName { get; set; }
        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }
        [Required, MinLength(2, ErrorMessage = "Fullname must be at least 2 characters long")]
        public string FullName { get; set; }
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
        public string SchoolName { get; set; }
        public string GradeLevel { get; set; }
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
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }

    public class TeacherUpdateRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
        public string Subjects { get; set; }
        public string Bio { get; set; }
    }

    public class ParentUpdateRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneSecondary { get; set; }
    }

    public class StudentUpdateRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
    }

    public class UserUpdateRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
    }

    public class CenterListResponse
    {
        public Guid UserId { get; set; }
        public string CenterName { get; set; }
        public string OwnerName { get; set; }
        public string LicenseNumber { get; set; }
        public string Address { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Status { get; set; }
    }

    public class TeacherListResponse
    {
        public string FullName { get; set; }
        public int YearOfExperience { get; set; }
        public string Qualification { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
    }

    public class ParentListResponse
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string PhoneSecondary { get; set; }
        public string Status { get; set; }
    }

    public class StudentListResponse
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string SchoolName { get; set; }
        public string GradeLevel { get; set; }
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
}

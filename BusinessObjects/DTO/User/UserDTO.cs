namespace BusinessObjects.DTO.User
{
    public class CreateAdminRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
    }

    public class CreateCenterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string CenterName { get; set; }
        public string LicenseNumber { get; set; }
        public DateOnly IssueDate { get; set; }
        public string LicenseIssuedBy { get; set; }
        public string Address { get; set; }
        
    }

    public class CreateTeacherRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public int YearOfExperience { get; set; }
        public string Qualifications { get; set; }
        public string LicenseNumber { get; set; }
        public string Subjects { get; set; }
        public string Bio { get; set; }
    }

    public class CreateParentRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
        public string PhoneSecondary { get; set; }
    }

    public class CreateStudentRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string SchoolName { get; set; }
        public string GradeLevel { get; set; }
    }
}

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
}

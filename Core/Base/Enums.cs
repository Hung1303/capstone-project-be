namespace Core.Base
{
    public enum UserRole
    {
        Admin = 1,
        Teacher = 2,
        Center = 3,
        Student = 4,
        Parent = 5
    }

    public enum AccountStatus
    {
        Pending = 0,
        Active = 1,
        Suspended = 2,
        Deactivated = 3
    }

    public enum TeachingMethod
    {
        InPerson = 1,
        Online = 2,
        Hybrid = 3
    }

    public enum CourseStatus
    {
        Draft = 0,
        PendingApproval = 1,
        Approved = 2,
        Rejected = 3,
        Suspended = 4,
        Archived = 5
    }

    public enum EnrollmentStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Completed = 3
    }

    public enum ReviewStatus
    {
        PendingModeration = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum ApprovalDecision
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum AuditActionType
    {
        Create = 1,
        Update = 2,
        Delete = 3,
        Login = 4,
        Logout = 5,
        Approve = 6,
        Reject = 7,
        Suspend = 8,
        Restore = 9
    }

    public enum ReportType
    {
        ComplianceCircular29 = 1,
        OperationalDashboard = 2
    }

    public enum BillingType
    {
        CoursePostFee = 1,
        ReferralCommission = 2
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Paid = 1,
        Failed = 2,
        Refunded = 3
    }
}



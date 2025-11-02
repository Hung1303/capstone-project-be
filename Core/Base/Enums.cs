namespace Core.Base
{
    public enum UserRole
    {
        Admin = 1,  //System management
        Center = 2,    //Register for the platform, courses
        Teacher = 3, //Register for the platform, accept course enrollment
        Student = 4,    //Search for courses, enroll for courses
        Parent = 5,  //Search for courses, enroll children for courses, track children study progress
        Inspector = 6 //To check if course is following the Circular 29
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
        //Refunded = 3
    }

    public enum Semester
    {
        Spring = 1,
        Summer = 2,
        Fall = 3,
    }

    public enum CenterStatus
    {
        Pending = 0,
        UnderVerification = 1,
        Verified = 2,
        Rejected = 3,
        Active = 4,
        Suspended = 5
    }

    public enum VerificationStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2,
        Failed = 3,
        Finalized = 4
    }

    public enum SubscriptionPackageTier
    {
        Basic = 1,
        Standard = 2,
        Premium = 3,
        Enterprise = 4
    }

    public enum SubscriptionStatus
    {
        Inactive = 0,
        Active = 1,
        Expired = 2,
        Cancelled = 3
    }
}



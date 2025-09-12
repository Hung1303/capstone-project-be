namespace Core.Base
{
    public enum UserRole
    {
        Admin = 1,
        Teacher = 2,
        Student = 3,
        Parent = 4
    }

    public enum TutoringType
    {
        InSchool = 1,    // No fees, specific student groups only
        OutOfSchool = 2  // With tuition fees, public information required
    }

    public enum StudentGroup
    {
        FailingSubjects = 1,           // Students failing subjects in latest semester
        AdvancedTraining = 2,          // Students selected for advanced/gifted training
        FinalYearPreparation = 3,      // Final-year students preparing for exams
        OutOfSchool = 4                // Students for out-of-school tutoring
    }

    public enum RegistrationStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }

    public enum ClassStatus
    {
        Planning = 1,
        Active = 2,
        Completed = 3,
        Cancelled = 4,
        Suspended = 5
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Paid = 2,
        Overdue = 3,
        Refunded = 4,
        Cancelled = 5
    }

    public enum TeachingMethod
    {
        InPerson = 1,
        Online = 2,
        Hybrid = 3
    }

    public enum AuditAction
    {
        Create = 1,
        Update = 2,
        Delete = 3,
        Login = 4,
        Logout = 5,
        Approve = 6,
        Reject = 7,
        Enroll = 8,
        Withdraw = 9
    }
}

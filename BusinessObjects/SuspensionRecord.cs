using Core.Base;

namespace BusinessObjects
{
    public class SuspensionRecord : BaseEntity
    {
        public Guid? UserId { get; set; }
        public Guid? CourseId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTimeOffset SuspendedFrom { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? SuspendedTo { get; set; }
        public Guid ActionByUserId { get; set; }


        public virtual User? User { get; set; }
        public virtual Course? Course { get; set; }
        public virtual User ActionByUser { get; set; } = null!;
    }
}



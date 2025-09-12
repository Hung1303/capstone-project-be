using System.ComponentModel.DataAnnotations;

namespace Core.Base
{
    public abstract class BaseEntity
    {
        [Key]
        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        protected BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = LastUpdatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }
    }
}

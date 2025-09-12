using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class Parent : BaseEntity
    {
        // Foreign Keys and Navigation Properties
        [Required]
        public string UserId { get; set; }
        public virtual User User { get; set; } = null!;

        // Business Properties
        [Required]
        [MaxLength(100)]
        public string ParentId { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Occupation { get; set; }

        [MaxLength(500)]
        public string? EmergencyContact { get; set; }

        // Collection Navigation Properties
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
        //public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}

using Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Payment : BaseEntity
    {
        [Range(10000.00, 100000000, ErrorMessage = "Ammount must be at between 10.000 and 100.000.000")]
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string status { get; set; }
        public DateTime PaymentDate {  get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public Guid? CenterSubscriptionId { get; set; }
        public Guid? EnrollmentId { get; set; }
        public virtual User User { get; set; }
        public virtual CenterSubscription CenterSubscription { get; set; }
        public virtual Enrollment Enrollment { get; set; }
    }
}

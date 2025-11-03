using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO.Payment
{
    public class CreatePaymentRequest
    {
        [Range(10000.00, 1000000000, ErrorMessage = "Ammount must be at between 10.000 and 1.000.000.000")]
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
    }
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string status { get; set; }
        public DateTime PaymentDate;
        public Guid UserId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Services.DTO.Payment
{
    public class CreatePaymentRequest
    {
        [Range(10000.00, 100000000, ErrorMessage = "Số lượng phải giữa 10.000 và 100.000.000")]
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public Guid? CenterSubscriptionId { get; set; }
        public Guid? EnrollmentId { get; set; }
    }
    public class VnPayRefundRequest
    {
        [JsonPropertyName("vnp_RequestId")]
        public string RequestId { get; set; } = string.Empty;

        [JsonPropertyName("vnp_Version")]
        public string Version { get; set; } = "2.1.0";

        [JsonPropertyName("vnp_Command")]
        public string Command { get; set; } = "refund";

        [JsonPropertyName("vnp_TmnCode")]
        public string TmnCode { get; set; } = string.Empty;

        [JsonPropertyName("vnp_TransactionType")]
        public string TransactionType { get; set; } = "02"; // 02: Full, 03: Partial

        [JsonPropertyName("vnp_TxnRef")]
        public string TxnRef { get; set; } = string.Empty;

        [JsonPropertyName("vnp_Amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("vnp_TransactionNo")]
        public string? TransactionNo { get; set; } // Có thể null

        [JsonPropertyName("vnp_TransactionDate")]
        public string TransactionDate { get; set; } = string.Empty; // yyyyMMddHHmmss

        [JsonPropertyName("vnp_CreateBy")]
        public string CreateBy { get; set; } = string.Empty;

        [JsonPropertyName("vnp_CreateDate")]
        public string CreateDate { get; set; } = string.Empty; // yyyyMMddHHmmss

        [JsonPropertyName("vnp_IpAddr")]
        public string IpAddr { get; set; } = string.Empty;

        [JsonPropertyName("vnp_OrderInfo")]
        public string OrderInfo { get; set; } = string.Empty;

        [JsonPropertyName("vnp_SecureHash")]
        public string SecureHash { get; set; } = string.Empty;
    }

    public class VnPayRefundResponse
    {
        [JsonPropertyName("vnp_ResponseId")]
        public string ResponseId { get; set; }

        [JsonPropertyName("vnp_Command")]
        public string Command { get; set; }

        [JsonPropertyName("vnp_ResponseCode")]
        public string ResponseCode { get; set; } 

        [JsonPropertyName("vnp_Message")]
        public string Message { get; set; }

        [JsonPropertyName("vnp_TmnCode")]
        public string TmnCode { get; set; }

        [JsonPropertyName("vnp_TxnRef")]
        public string TxnRef { get; set; }

        [JsonPropertyName("vnp_Amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("vnp_BankCode")]
        public string BankCode { get; set; }

        [JsonPropertyName("vnp_PayDate")]
        public string PayDate { get; set; }

        [JsonPropertyName("vnp_TransactionNo")]
        public string TransactionNo { get; set; }

        [JsonPropertyName("vnp_TransactionType")]
        public string TransactionType { get; set; }

        [JsonPropertyName("vnp_TransactionStatus")]
        public string TransactionStatus { get; set; }

        [JsonPropertyName("vnp_OrderInfo")]
        public string OrderInfo { get; set; }

        [JsonPropertyName("vnp_SecureHash")]
        public string SecureHash { get; set; }
    }
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string status { get; set; }
        public DateTime PaymentDate;
        public Guid UserId { get; set; }
        public Guid? CenterSubscriptionId { get; set; }
        public Guid? EnrollmentId { get; set; }
    }
}

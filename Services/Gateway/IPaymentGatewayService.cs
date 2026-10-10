using System;
using System.Threading.Tasks;

namespace RealEstatePortal.Web.Services.Gateway
{
    public class PaymentProcessRequest
    {
        public int PropertyId { get; set; }
        public decimal Amount { get; set; }
        public string CardHolderName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpirationMonthYear { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "CreditCard";
    }

    public class PaymentProcessResult
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string ReceiptNumber { get; set; } = string.Empty;
        public string GatewayReference { get; set; } = string.Empty;
        public string CardLast4 { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }

    public interface IPaymentGatewayService
    {
        Task<PaymentProcessResult> ProcessTokenPaymentAsync(PaymentProcessRequest request);
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RealEstatePortal.Web.Services.Gateway
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly ILogger<PaymentGatewayService> _logger;

        public PaymentGatewayService(ILogger<PaymentGatewayService> logger)
        {
            _logger = logger;
        }

        public async Task<PaymentProcessResult> ProcessTokenPaymentAsync(PaymentProcessRequest request)
        {
            // Simulate payment gateway latency
            await Task.Delay(300);

            _logger.LogInformation("Processing mock token payment for Property #{PropertyId} for Amount {Amount:C}", request.PropertyId, request.Amount);

            // Basic validation
            if (string.IsNullOrWhiteSpace(request.CardHolderName) || request.Amount <= 0)
            {
                return new PaymentProcessResult
                {
                    IsSuccess = false,
                    Message = "Invalid card holder name or transaction amount."
                };
            }

            var cleanCard = (request.CardNumber ?? string.Empty).Replace(" ", "").Replace("-", "");
            string last4 = cleanCard.Length >= 4 ? cleanCard.Substring(cleanCard.Length - 4) : "4242";

            var guidStr = Guid.NewGuid().ToString("N");
            var txnId = "TXN-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-" + (guidStr.Length >= 8 ? guidStr.Substring(0, 8) : guidStr).ToUpper();
            var receiptNo = "REC-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999);
            var gatewayRef = "PG-AUTH-" + (guidStr.Length >= 12 ? guidStr.Substring(0, 12) : guidStr).ToUpper();

            return new PaymentProcessResult
            {
                IsSuccess = true,
                TransactionId = txnId,
                ReceiptNumber = receiptNo,
                GatewayReference = gatewayRef,
                CardLast4 = last4,
                Message = "Payment authorized and token escrow confirmed successfully.",
                ProcessedAt = DateTime.UtcNow
            };
        }
    }
}

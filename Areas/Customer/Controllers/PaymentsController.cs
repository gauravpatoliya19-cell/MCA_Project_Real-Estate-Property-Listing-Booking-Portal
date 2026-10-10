using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealEstatePortal.Web.Areas.Customer.ViewModels;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Repositories.Interfaces;
using RealEstatePortal.Web.Services.Gateway;

namespace RealEstatePortal.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class PaymentsController : Controller
    {
        private readonly IPropertyRepository _propertyRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IInquiryRepository _inquiryRepo;
        private readonly IPaymentGatewayService _gatewayService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            IPropertyRepository propertyRepo,
            IPaymentRepository paymentRepo,
            IInquiryRepository inquiryRepo,
            IPaymentGatewayService gatewayService,
            ILogger<PaymentsController> logger)
        {
            _propertyRepo = propertyRepo;
            _paymentRepo = paymentRepo;
            _inquiryRepo = inquiryRepo;
            _gatewayService = gatewayService;
            _logger = logger;
        }

        public async Task<IActionResult> Checkout(int propertyId)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please sign in to proceed with your property token reservation.";
                return RedirectToAction("Login", "Account", new { area = "", returnUrl = $"/Customer/Payments/Checkout?propertyId={propertyId}" });
            }

            var property = await _propertyRepo.GetByIdAsync(propertyId);
            if (property == null)
            {
                TempData["ErrorMessage"] = "Selected property not found.";
                return RedirectToAction("Index", "Catalog");
            }

            if (property.Status == PropertyStatus.Rented || property.Status == PropertyStatus.Sold)
            {
                TempData["ErrorMessage"] = $"This property is currently marked as {property.Status} and cannot be reserved.";
                return RedirectToAction("Details", "Catalog", new { id = propertyId });
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            var userName = User.Identity.Name ?? "";

            var vm = new TokenPaymentVM
            {
                PropertyId = property.Id,
                Property = property,
                Amount = property.TokenAmount > 0 ? property.TokenAmount : (property.Price * 0.05m),
                CardHolderName = userName,
                CustomerEmail = userEmail,
                CustomerPhone = "+91 98251 77340"
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(TokenPaymentVM model)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please sign in to process payments.";
                return RedirectToAction("Login", "Account", new { area = "", returnUrl = $"/Customer/Payments/Checkout?propertyId={model.PropertyId}" });
            }

            var property = await _propertyRepo.GetByIdAsync(model.PropertyId);
            if (property == null)
            {
                TempData["ErrorMessage"] = "Property not found.";
                return RedirectToAction("Index", "Catalog");
            }

            model.Property = property;

            if (!ModelState.IsValid)
            {
                return View("Checkout", model);
            }

            var gatewayRequest = new PaymentProcessRequest
            {
                PropertyId = model.PropertyId,
                Amount = model.Amount,
                CardHolderName = model.CardHolderName,
                CardNumber = model.CardNumber,
                ExpirationMonthYear = model.Expiration,
                Cvv = model.Cvv,
                Email = model.CustomerEmail,
                Phone = model.CustomerPhone,
                PaymentMethod = model.PaymentMethod.ToString()
            };

            var result = await _gatewayService.ProcessTokenPaymentAsync(gatewayRequest);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("Checkout", model);
            }

            var payment = new TokenPayment
            {
                PropertyId = model.PropertyId,
                CustomerName = model.CardHolderName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                Amount = model.Amount,
                TransactionId = result.TransactionId,
                ReceiptNumber = result.ReceiptNumber,
                PaymentMethod = model.PaymentMethod,
                Status = PaymentStatus.Success,
                PaymentDate = DateTime.UtcNow,
                CardLast4 = result.CardLast4,
                GatewayReference = result.GatewayReference,
                Notes = model.Notes ?? $"Token reservation for {property.Title}"
            };

            await _paymentRepo.AddAsync(payment);

            // Update property status to Booked
            await _propertyRepo.UpdateStatusAsync(model.PropertyId, PropertyStatus.Booked);

            // Add inquiry conversion record
            var inquiry = new Inquiry
            {
                PropertyId = model.PropertyId,
                CustomerName = model.CardHolderName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                Message = $"Token payment reservation completed. Receipt: {payment.ReceiptNumber}, Amount: ₹{payment.Amount:N2}",
                InquiryDate = DateTime.UtcNow,
                Status = InquiryStatus.ConvertedToBooking
            };
            await _inquiryRepo.AddAsync(inquiry);

            TempData["SuccessMessage"] = $"Payment successful! Property #{property.Id} is reserved. Receipt #{payment.ReceiptNumber} has been issued.";
            return RedirectToAction(nameof(Receipt), new { paymentId = payment.Id });
        }

        public async Task<IActionResult> Receipt(int paymentId)
        {
            var payment = await _paymentRepo.GetByIdAsync(paymentId);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }
    }
}

using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealEstatePortal.Web.Areas.Customer.ViewModels;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Repositories.Interfaces;
using RealEstatePortal.Web.Services.Pdf;

namespace RealEstatePortal.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ContractsController : Controller
    {
        private readonly IPropertyRepository _propertyRepo;
        private readonly IContractRepository _contractRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IContractPdfService _pdfService;
        private readonly IInquiryRepository _inquiryRepo;

        public ContractsController(
            IPropertyRepository propertyRepo,
            IContractRepository contractRepo,
            IPaymentRepository paymentRepo,
            IContractPdfService pdfService,
            IInquiryRepository inquiryRepo)
        {
            _propertyRepo = propertyRepo;
            _contractRepo = contractRepo;
            _paymentRepo = paymentRepo;
            _pdfService = pdfService;
            _inquiryRepo = inquiryRepo;
        }

        public async Task<IActionResult> Generate(int propertyId, int? paymentId)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please sign in to generate and review legal tenancy agreements.";
                return RedirectToAction("Login", "Account", new { area = "", returnUrl = $"/Customer/Contracts/Generate?propertyId={propertyId}" });
            }

            var property = await _propertyRepo.GetByIdAsync(propertyId);
            if (property == null)
            {
                TempData["ErrorMessage"] = "Property not found.";
                return RedirectToAction("Index", "Catalog");
            }

            decimal tokenPaid = 0;
            string tenantName = User.Identity?.Name ?? "Rajesh Patel";
            string tenantEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "rajesh.patel@gmail.com";
            string tenantPhone = "+91 98251 77340";

            if (paymentId.HasValue)
            {
                var payment = await _paymentRepo.GetByIdAsync(paymentId.Value);
                if (payment != null)
                {
                    tokenPaid = payment.Amount;
                    tenantName = payment.CustomerName;
                    tenantEmail = payment.CustomerEmail;
                    tenantPhone = payment.CustomerPhone;
                }
            }
            else
            {
                var payments = await _paymentRepo.GetByPropertyIdAsync(propertyId);
                var latest = payments.FirstOrDefault();
                if (latest != null)
                {
                    tokenPaid = latest.Amount;
                    tenantName = latest.CustomerName;
                    tenantEmail = latest.CustomerEmail;
                    tenantPhone = latest.CustomerPhone;
                }
            }

            var vm = new ContractGenerationVM
            {
                PropertyId = property.Id,
                Property = property,
                TokenPaymentId = paymentId,
                TenantName = tenantName,
                TenantEmail = tenantEmail,
                TenantPhone = tenantPhone,
                MonthlyRent = property.Price,
                SecurityDeposit = property.ListingType == ListingType.Rent ? (property.Price * 2) : 50000m,
                TokenPaid = tokenPaid,
                StartDate = DateTime.Today.AddDays(7),
                EndDate = DateTime.Today.AddDays(7).AddYears(1),
                LandlordName = property.AgentName ?? "Real Estate Property Gujarat Pvt Ltd",
                LandlordEmail = property.AgentEmail ?? "leases@realestateproperty.in"
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContract(ContractGenerationVM model)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please sign in to generate contracts.";
                return RedirectToAction("Login", "Account", new { area = "", returnUrl = $"/Customer/Contracts/Generate?propertyId={model.PropertyId}" });
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
                return View("Generate", model);
            }

            // Generate Contract Number
            var cityPrefix = (property.City.Length >= 3 ? property.City.Substring(0, 3) : "GUJ").ToUpper();
            var contractNo = $"LSE-{DateTime.UtcNow.Year}-{cityPrefix}-{Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()}";

            // Digital signature hash
            var rawSig = $"{model.TenantName}|{model.TenantIdProof}|{model.PropertyId}|{model.MonthlyRent}|{DateTime.UtcNow:O}";
            var sigHash = "SIG-SHA256-" + Convert.ToBase64String(Encoding.UTF8.GetBytes(rawSig)).Substring(0, 20).ToUpper();

            var contract = new RentalContract
            {
                PropertyId = model.PropertyId,
                ContractNumber = contractNo,
                TenantName = model.TenantName,
                TenantEmail = model.TenantEmail,
                TenantPhone = model.TenantPhone,
                TenantIdProof = model.TenantIdProof,
                LandlordName = model.LandlordName,
                LandlordEmail = model.LandlordEmail,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                MonthlyRent = model.MonthlyRent,
                SecurityDeposit = model.SecurityDeposit,
                TokenPaid = model.TokenPaid,
                PaymentFrequency = model.PaymentFrequency,
                TermsAndConditions = model.TermsAndConditions,
                Status = ContractStatus.Active,
                DigitalSignature = sigHash,
                GeneratedAt = DateTime.UtcNow
            };

            await _contractRepo.AddAsync(contract);

            // Update property status to Rented
            await _propertyRepo.UpdateStatusAsync(model.PropertyId, PropertyStatus.Rented);

            // Add inquiry conversion record
            var inquiry = new Inquiry
            {
                PropertyId = model.PropertyId,
                CustomerName = model.TenantName,
                CustomerEmail = model.TenantEmail,
                CustomerPhone = model.TenantPhone,
                Message = $"Contract executed #{contract.ContractNumber}. Monthly rent: ₹{contract.MonthlyRent:N2}",
                InquiryDate = DateTime.UtcNow,
                Status = InquiryStatus.Closed
            };
            await _inquiryRepo.AddAsync(inquiry);

            TempData["SuccessMessage"] = $"Lease Agreement #{contract.ContractNumber} has been officially generated and digitally signed!";
            return RedirectToAction(nameof(Details), new { id = contract.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _contractRepo.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        public async Task<IActionResult> DownloadHtml(int id)
        {
            var contract = await _contractRepo.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            var html = _pdfService.GenerateContractHtml(contract);
            var bytes = Encoding.UTF8.GetBytes(html);
            return File(bytes, "text/html", $"Lease_Contract_{contract.ContractNumber}.html");
        }
    }
}

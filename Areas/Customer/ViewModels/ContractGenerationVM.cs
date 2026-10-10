using System;
using System.ComponentModel.DataAnnotations;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Areas.Customer.ViewModels
{
    public class ContractGenerationVM
    {
        [Required]
        public int PropertyId { get; set; }
        public Property? Property { get; set; }

        public int? TokenPaymentId { get; set; }

        // Tenant Info
        [Required(ErrorMessage = "Tenant full legal name is required.")]
        [Display(Name = "Tenant Full Name")]
        public string TenantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tenant email is required.")]
        [EmailAddress]
        [Display(Name = "Tenant Email")]
        public string TenantEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tenant phone is required.")]
        [Phone]
        [Display(Name = "Tenant Phone")]
        public string TenantPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Govt ID or Passport number is required.")]
        [Display(Name = "Govt ID / SSN / Passport No")]
        public string TenantIdProof { get; set; } = "ID-US-982144";

        // Landlord Info
        [Required]
        [Display(Name = "Lessor / Landlord Name")]
        public string LandlordName { get; set; } = "Real Estate Property Gujarat Pvt Ltd";

        [Required, EmailAddress]
        [Display(Name = "Landlord Email")]
        public string LandlordEmail { get; set; } = "leases@realestateproperty.in";

        // Lease Financials & Dates
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Lease Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Lease End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddYears(1);

        [Required]
        [Display(Name = "Monthly Rent (₹)")]
        public decimal MonthlyRent { get; set; }

        [Required]
        [Display(Name = "Security Deposit (₹)")]
        public decimal SecurityDeposit { get; set; }

        [Display(Name = "Token Deposit Credited (₹)")]
        public decimal TokenPaid { get; set; }

        [Display(Name = "Payment Frequency")]
        public PaymentFrequency PaymentFrequency { get; set; } = PaymentFrequency.Monthly;

        [Required]
        [Display(Name = "Terms & Conditions")]
        public string TermsAndConditions { get; set; } = 
            "1. RENT OBLIGATION: The Tenant agrees to pay the specified monthly rent on or before the 5th of each calendar month.\n" +
            "2. SECURITY DEPOSIT: The security deposit shall be held in escrow and refunded within 30 days of lease expiration less any legitimate damage deductions.\n" +
            "3. MAINTENANCE: Tenant shall maintain the property in a clean, sanitary condition and society rules must be strictly respected.\n" +
            "4. UTILITIES & OCCUPANCY: Tenant is responsible for electricity and society maintenance charges. Property shall be used solely for lawful residential purposes.\n" +
            "5. DEFAULT & GOVERNING LAW: In case of non-payment or breach, tenancy is subject to the Model Tenancy Act as enacted in Gujarat State jurisdiction.";
    }
}

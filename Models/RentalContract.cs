using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstatePortal.Web.Models
{
    public enum ContractStatus
    {
        Draft,
        Active,
        Terminated,
        Expired
    }

    public enum PaymentFrequency
    {
        Monthly,
        Quarterly,
        BiAnnually,
        Annually
    }

    public class RentalContract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Contract Number")]
        public string ContractNumber { get; set; } = string.Empty;

        // Tenant Details
        [Required, StringLength(100)]
        [Display(Name = "Tenant Full Name")]
        public string TenantName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        [Display(Name = "Tenant Email")]
        public string TenantEmail { get; set; } = string.Empty;

        [Required, Phone, StringLength(30)]
        [Display(Name = "Tenant Phone")]
        public string TenantPhone { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Tenant ID / SSN / Passport")]
        public string TenantIdProof { get; set; } = "ID-9823472";

        // Landlord Details
        [Required, StringLength(100)]
        [Display(Name = "Landlord / Lessor Name")]
        public string LandlordName { get; set; } = "Real Estate Property Gujarat Pvt Ltd";

        [Required, EmailAddress, StringLength(150)]
        [Display(Name = "Landlord Email")]
        public string LandlordEmail { get; set; } = "leases@realestateproperty.in";

        // Lease Financials & Dates
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Lease Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Lease End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Monthly Rent Amount (₹)")]
        public decimal MonthlyRent { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Security Deposit (₹)")]
        public decimal SecurityDeposit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Token Deposit Credited (₹)")]
        public decimal TokenPaid { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Net Balance Due Upon Move-in (₹)")]
        public decimal BalanceDue => (MonthlyRent + SecurityDeposit) - TokenPaid;

        [Required]
        [Display(Name = "Payment Frequency")]
        public PaymentFrequency PaymentFrequency { get; set; } = PaymentFrequency.Monthly;

        [Required, StringLength(4000)]
        [Display(Name = "Terms & Conditions")]
        public string TermsAndConditions { get; set; } = string.Empty;

        [Required]
        public ContractStatus Status { get; set; } = ContractStatus.Active;

        [Display(Name = "Digital Signature Code")]
        public string DigitalSignature { get; set; } = string.Empty;

        [Display(Name = "Contract Generated Date")]
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}

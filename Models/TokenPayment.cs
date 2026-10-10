using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstatePortal.Web.Models
{
    public enum PaymentStatus
    {
        Success,
        Pending,
        Failed,
        Refunded
    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        NetBanking,
        UPI,
        WireTransfer
    }

    public class TokenPayment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Payer Full Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        [Display(Name = "Payer Email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required, Phone, StringLength(30)]
        [Display(Name = "Payer Phone")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Token Amount Paid (₹)")]
        public decimal Amount { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Transaction ID")]
        public string TransactionId { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Display(Name = "Receipt Number")]
        public string ReceiptNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Success;

        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [StringLength(10)]
        [Display(Name = "Card Last 4 Digits")]
        public string? CardLast4 { get; set; }

        [StringLength(100)]
        [Display(Name = "Payment Gateway Reference")]
        public string GatewayReference { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}

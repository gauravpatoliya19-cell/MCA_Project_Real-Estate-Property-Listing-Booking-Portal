using System.ComponentModel.DataAnnotations;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Areas.Customer.ViewModels
{
    public class TokenPaymentVM
    {
        public int PropertyId { get; set; }
        public Property? Property { get; set; }

        [Required]
        [Display(Name = "Reservation Token Amount (₹)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Cardholder name is required.")]
        [Display(Name = "Cardholder Name")]
        public string CardHolderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display(Name = "Payer Email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone]
        [Display(Name = "Payer Phone")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Card number is required.")]
        [CreditCard(ErrorMessage = "Please enter a valid credit card number.")]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; } = "4242 4242 4242 4242";

        [Required(ErrorMessage = "Expiry date is required.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Format must be MM/YY")]
        [Display(Name = "Expiration (MM/YY)")]
        public string Expiration { get; set; } = "12/28";

        [Required(ErrorMessage = "CVV is required.")]
        [RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits")]
        [Display(Name = "CVV")]
        public string Cvv { get; set; } = "123";

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;

        [Display(Name = "Additional Notes / Booking Reference")]
        public string? Notes { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstatePortal.Web.Models
{
    public enum InquiryStatus
    {
        New,
        Contacted,
        ConvertedToSiteVisit,
        ConvertedToBooking,
        Closed
    }

    public class Inquiry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Your Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        [Display(Name = "Your Email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required, Phone, StringLength(30)]
        [Display(Name = "Phone Number")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        [Display(Name = "Message / Questions")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Inquiry Date")]
        public DateTime InquiryDate { get; set; } = DateTime.UtcNow;

        [Required]
        public InquiryStatus Status { get; set; } = InquiryStatus.New;
    }
}

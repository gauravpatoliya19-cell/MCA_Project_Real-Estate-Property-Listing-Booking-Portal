using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstatePortal.Web.Models
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }

    public class SiteVisitBooking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Visitor Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        [Display(Name = "Visitor Email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required, Phone, StringLength(30)]
        [Display(Name = "Visitor Phone")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Scheduled Date")]
        public DateTime VisitDate { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Time Slot")]
        public string TimeSlot { get; set; } = "10:00 AM - 11:30 AM";

        [StringLength(500)]
        [Display(Name = "Special Notes / Requirements")]
        public string? Notes { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Display(Name = "Booking Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstatePortal.Web.Models
{
    public class ApplicationUser
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required, StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(255)]
        public string PasswordSalt { get; set; } = string.Empty;

        [Phone, StringLength(30)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Role { get; set; } = "Customer"; // Admin, Agent, Customer

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Registered Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

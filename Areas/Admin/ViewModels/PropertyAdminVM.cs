using System;
using System.ComponentModel.DataAnnotations;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Areas.Admin.ViewModels
{
    public class PropertyAdminVM
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Property Title")]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Property Type")]
        public PropertyType PropertyType { get; set; } = PropertyType.Apartment;

        [Required]
        [Display(Name = "Listing Type")]
        public ListingType ListingType { get; set; } = ListingType.Sale;

        [Required]
        public PropertyStatus Status { get; set; } = PropertyStatus.Available;

        [Required]
        [Range(0, 1000000000)]
        [Display(Name = "Price (₹)")]
        public decimal Price { get; set; }

        [Display(Name = "Token Booking Amount (₹)")]
        public decimal TokenAmount { get; set; }

        [Required, StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string State { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; } = string.Empty;

        [Range(0, 50)]
        public int Bedrooms { get; set; }

        [Range(0, 50)]
        public int Bathrooms { get; set; }

        [Display(Name = "Area (Sq Ft)")]
        public double AreaSqFt { get; set; }

        [Display(Name = "Year Built")]
        public int YearBuilt { get; set; } = DateTime.UtcNow.Year;

        [Display(Name = "Featured Property")]
        public bool IsFeatured { get; set; }

        [StringLength(100)]
        [Display(Name = "Assigned Agent Name")]
        public string AgentName { get; set; } = "Chirag Patel";

        [StringLength(150)]
        [Display(Name = "Agent Email")]
        public string AgentEmail { get; set; } = "chirag.patel@realestateportal.com";

        [StringLength(50)]
        [Display(Name = "Agent Phone")]
        public string AgentPhone { get; set; } = "+91 98250 12345";

        [Range(0, 100)]
        [Display(Name = "Commission Rate (%)")]
        public decimal CommissionRate { get; set; } = 3.0m;

        [Display(Name = "Primary Image URL")]
        public string? PrimaryImageUrl { get; set; }

        [Display(Name = "Additional Image URLs (one per line)")]
        public string? AdditionalImageUrls { get; set; }

        [Display(Name = "Amenities & Features (comma-separated)")]
        public string? SelectedFeaturesComma { get; set; }
    }
}

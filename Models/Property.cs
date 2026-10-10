using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace RealEstatePortal.Web.Models
{
    public enum PropertyType
    {
        Apartment,
        Villa,
        Penthouse,
        Commercial,
        Studio,
        Townhouse
    }

    public enum ListingType
    {
        Rent,
        Sale
    }

    public enum PropertyStatus
    {
        Available,
        Booked,
        Rented,
        Sold
    }

    public class Property
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Property Title")]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Property Type")]
        public PropertyType PropertyType { get; set; }

        [Required]
        [Display(Name = "Listing Type")]
        public ListingType ListingType { get; set; }

        [Required]
        public PropertyStatus Status { get; set; } = PropertyStatus.Available;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1000000000)]
        [Display(Name = "Price / Monthly Rent (₹)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
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
        [Display(Name = "Agent Name")]
        public string AgentName { get; set; } = "Chirag Patel";

        [StringLength(150)]
        [Display(Name = "Agent Email")]
        public string AgentEmail { get; set; } = "chirag.patel@realestateportal.com";

        [StringLength(50)]
        [Display(Name = "Agent Phone")]
        public string AgentPhone { get; set; } = "+91 98250 12345";

        [Range(0, 100)]
        [Display(Name = "Commission Rate (%)")]
        public decimal CommissionRate { get; set; } = 3.5m;

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public virtual ICollection<PropertyFeature> Features { get; set; } = new List<PropertyFeature>();
        public virtual ICollection<SiteVisitBooking> SiteVisits { get; set; } = new List<SiteVisitBooking>();
        public virtual ICollection<TokenPayment> TokenPayments { get; set; } = new List<TokenPayment>();
        public virtual ICollection<RentalContract> RentalContracts { get; set; } = new List<RentalContract>();
        public virtual ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();

        [NotMapped]
        public string PrimaryImageUrl => Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl 
            ?? Images.FirstOrDefault()?.ImageUrl 
            ?? "https://images.unsplash.com/photo-1560518883-ce09059eeffa?auto=format&fit=crop&w=800&q=80";

        [NotMapped]
        public string FormattedPrice
        {
            get
            {
                if (Price >= 10000000m)
                {
                    decimal cr = Price / 10000000m;
                    return $"₹{cr:0.##} Cr";
                }
                else if (Price >= 100000m)
                {
                    decimal lac = Price / 100000m;
                    return $"₹{lac:0.##} Lac";
                }
                else
                {
                    return $"₹{Price:N0}";
                }
            }
        }

        [NotMapped]
        public string FormattedPriceWithPeriod => ListingType == ListingType.Rent 
            ? $"{FormattedPrice} / month" 
            : FormattedPrice;

        [NotMapped]
        public string FormattedTokenAmount
        {
            get
            {
                decimal amount = TokenAmount > 0 ? TokenAmount : (Price * 0.05m);
                if (amount >= 10000000m)
                    return $"₹{(amount / 10000000m):0.##} Cr";
                if (amount >= 100000m)
                    return $"₹{(amount / 100000m):0.##} Lac";
                return $"₹{amount:N0}";
            }
        }
    }
}

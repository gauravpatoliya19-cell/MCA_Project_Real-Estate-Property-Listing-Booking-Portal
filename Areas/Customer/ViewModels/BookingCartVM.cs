using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Areas.Customer.ViewModels
{
    public class BookingCartItemVM
    {
        public int PropertyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ListingType ListingType { get; set; }
        public PropertyType PropertyType { get; set; }

        [Required(ErrorMessage = "Please select a visit date.")]
        [DataType(DataType.Date)]
        public DateTime VisitDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Please select a preferred time slot.")]
        public string TimeSlot { get; set; } = "10:00 AM - 11:30 AM";

        public string? Notes { get; set; }

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

        public string FormattedPriceWithPeriod => ListingType == ListingType.Rent 
            ? $"{FormattedPrice} / month" 
            : FormattedPrice;
    }

    public class BookingCartVM
    {
        public List<BookingCartItemVM> Items { get; set; } = new List<BookingCartItemVM>();

        [Required(ErrorMessage = "Please enter your full name.")]
        [Display(Name = "Your Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Your Email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your phone number.")]
        [Phone(ErrorMessage = "Invalid phone format.")]
        [Display(Name = "Your Phone")]
        public string CustomerPhone { get; set; } = string.Empty;
    }
}

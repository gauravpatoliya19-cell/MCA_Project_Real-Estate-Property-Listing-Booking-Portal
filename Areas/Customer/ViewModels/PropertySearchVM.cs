using System.Collections.Generic;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Areas.Customer.ViewModels
{
    public class PropertySearchVM
    {
        public string? Keyword { get; set; }
        public string? City { get; set; }
        public PropertyType? PropertyType { get; set; }
        public ListingType? ListingType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public PropertyStatus? Status { get; set; }
        public string? SortBy { get; set; } = "newest"; // price_asc, price_desc, newest, area_desc

        public List<string> SelectedFeatures { get; set; } = new List<string>();

        // Result data
        public IEnumerable<Property> Properties { get; set; } = new List<Property>();
        public IEnumerable<string> AvailableCities { get; set; } = new List<string>();
        public int TotalCount { get; set; }
    }
}

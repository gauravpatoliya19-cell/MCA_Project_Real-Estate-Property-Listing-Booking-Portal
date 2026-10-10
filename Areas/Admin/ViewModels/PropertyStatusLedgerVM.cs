using System;
using System.Collections.Generic;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Areas.Admin.ViewModels
{
    public class PropertyStatusLedgerItemVM
    {
        public int PropertyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public PropertyType PropertyType { get; set; }
        public ListingType ListingType { get; set; }
        public PropertyStatus Status { get; set; }
        public decimal Price { get; set; }
        public decimal TokenAmount { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public int TotalSiteVisits { get; set; }
        public decimal TotalTokensCollected { get; set; }
        public int InquiriesCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PropertyStatusLedgerVM
    {
        // Filters
        public PropertyStatus? FilterStatus { get; set; }
        public ListingType? FilterListingType { get; set; }
        public string? FilterCity { get; set; }
        public string? SearchTerm { get; set; }

        // Metrics
        public int TotalProperties { get; set; }
        public int TotalAvailable { get; set; }
        public int TotalBooked { get; set; }
        public int TotalRented { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalPortfolioValue { get; set; }
        public decimal TotalMonthlyRentalYield { get; set; }
        public decimal TotalTokensEscrowed { get; set; }

        // Rows
        public List<PropertyStatusLedgerItemVM> LedgerItems { get; set; } = new List<PropertyStatusLedgerItemVM>();
        public List<string> AvailableCities { get; set; } = new List<string>();
    }
}

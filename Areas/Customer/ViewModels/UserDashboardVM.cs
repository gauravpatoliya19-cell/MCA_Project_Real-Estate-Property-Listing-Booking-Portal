using System.Collections.Generic;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Areas.Customer.ViewModels
{
    public class UserDashboardVM
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        public List<SiteVisitBooking> MyBookings { get; set; } = new List<SiteVisitBooking>();
        public List<TokenPayment> MyPayments { get; set; } = new List<TokenPayment>();
        public List<RentalContract> MyContracts { get; set; } = new List<RentalContract>();
        public List<Inquiry> MyInquiries { get; set; } = new List<Inquiry>();
    }
}

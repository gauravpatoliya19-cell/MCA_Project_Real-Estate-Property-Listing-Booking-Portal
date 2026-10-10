using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Repositories.Interfaces;

namespace RealEstatePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Agent")]
    public class DashboardController : Controller
    {
        private readonly IPropertyRepository _propertyRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IContractRepository _contractRepo;
        private readonly IInquiryRepository _inquiryRepo;

        public DashboardController(
            IPropertyRepository propertyRepo,
            IBookingRepository bookingRepo,
            IPaymentRepository paymentRepo,
            IContractRepository contractRepo,
            IInquiryRepository inquiryRepo)
        {
            _propertyRepo = propertyRepo;
            _bookingRepo = bookingRepo;
            _paymentRepo = paymentRepo;
            _contractRepo = contractRepo;
            _inquiryRepo = inquiryRepo;
        }

        public async Task<IActionResult> Index()
        {
            var properties = (await _propertyRepo.GetAllAsync()).ToList();
            var payments = (await _paymentRepo.GetAllAsync()).ToList();
            var bookings = (await _bookingRepo.GetAllAsync()).ToList();
            var contracts = (await _contractRepo.GetAllAsync()).ToList();
            var inquiries = (await _inquiryRepo.GetAllAsync()).ToList();

            ViewBag.TotalProperties = properties.Count;
            ViewBag.AvailableCount = properties.Count(p => p.Status == PropertyStatus.Available);
            ViewBag.BookedCount = properties.Count(p => p.Status == PropertyStatus.Booked);
            ViewBag.RentedCount = properties.Count(p => p.Status == PropertyStatus.Rented);
            ViewBag.SoldCount = properties.Count(p => p.Status == PropertyStatus.Sold);

            ViewBag.TotalRevenue = payments.Sum(p => p.Amount);
            ViewBag.TotalSiteVisits = bookings.Count;
            ViewBag.TotalContracts = contracts.Count;
            ViewBag.TotalInquiries = inquiries.Count;

            // Calculate total commission earned from Sold and Rented properties
            decimal totalCommission = 0;
            foreach (var prop in properties.Where(p => p.Status == PropertyStatus.Sold || p.Status == PropertyStatus.Rented))
            {
                if (prop.Status == PropertyStatus.Sold)
                {
                    totalCommission += prop.Price * (prop.CommissionRate / 100m);
                }
                else if (prop.Status == PropertyStatus.Rented)
                {
                    totalCommission += prop.Price;
                }
            }
            ViewBag.TotalCommission = totalCommission;

            // Conversion rate
            double conversionRate = inquiries.Count > 0 ? ((double)contracts.Count / inquiries.Count) * 100 : 0;
            ViewBag.ConversionRate = Math.Round(conversionRate, 1);

            ViewBag.RecentPayments = payments.Take(5).ToList();
            ViewBag.RecentBookings = bookings.Take(5).ToList();
            ViewBag.RecentProperties = properties.Take(5).ToList();

            return View();
        }
    }
}

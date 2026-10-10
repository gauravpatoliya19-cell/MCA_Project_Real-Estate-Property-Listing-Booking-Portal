using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstatePortal.Web.Areas.Customer.ViewModels;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Repositories.Interfaces;

namespace RealEstatePortal.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BookingCartController : Controller
    {
        private const string CartSessionKey = "SiteVisitCart_Session";
        private readonly IPropertyRepository _propertyRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IInquiryRepository _inquiryRepo;

        public BookingCartController(
            IPropertyRepository propertyRepo,
            IBookingRepository bookingRepo,
            IInquiryRepository inquiryRepo)
        {
            _propertyRepo = propertyRepo;
            _bookingRepo = bookingRepo;
            _inquiryRepo = inquiryRepo;
        }

        private List<BookingCartItemVM> GetCartFromSession()
        {
            var json = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(json))
            {
                return new List<BookingCartItemVM>();
            }
            try
            {
                return JsonSerializer.Deserialize<List<BookingCartItemVM>>(json) ?? new List<BookingCartItemVM>();
            }
            catch
            {
                return new List<BookingCartItemVM>();
            }
        }

        private void SaveCartToSession(List<BookingCartItemVM> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        public IActionResult Index()
        {
            var cartItems = GetCartFromSession();
            var vm = new BookingCartVM
            {
                Items = cartItems,
                CustomerName = User.Identity?.IsAuthenticated == true ? (User.Identity.Name ?? "") : "",
                CustomerEmail = User.Identity?.IsAuthenticated == true ? (User.FindFirst(ClaimTypes.Email)?.Value ?? "") : ""
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int propertyId)
        {
            var property = await _propertyRepo.GetByIdAsync(propertyId);
            if (property == null)
            {
                TempData["ErrorMessage"] = "Property not found.";
                return RedirectToAction("Index", "Catalog");
            }

            var cart = GetCartFromSession();
            if (!cart.Any(c => c.PropertyId == propertyId))
            {
                cart.Add(new BookingCartItemVM
                {
                    PropertyId = property.Id,
                    Title = property.Title,
                    ImageUrl = property.PrimaryImageUrl,
                    Address = property.Address,
                    City = property.City,
                    Price = property.Price,
                    ListingType = property.ListingType,
                    PropertyType = property.PropertyType,
                    VisitDate = DateTime.Today.AddDays(1),
                    TimeSlot = "10:00 AM - 11:30 AM"
                });
                SaveCartToSession(cart);
                TempData["SuccessMessage"] = $"\"{property.Title}\" has been added to your Site Visit Booking Cart!";
            }
            else
            {
                TempData["InfoMessage"] = "This property is already in your Site Visit schedule cart.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int propertyId)
        {
            var cart = GetCartFromSession();
            cart.RemoveAll(c => c.PropertyId == propertyId);
            SaveCartToSession(cart);
            TempData["SuccessMessage"] = "Property removed from site visit cart.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
            TempData["SuccessMessage"] = "Your site visit booking cart has been cleared.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBooking(BookingCartVM model)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please sign in to schedule and confirm your site visit bookings.";
                return RedirectToAction("Login", "Account", new { area = "", returnUrl = "/Customer/BookingCart" });
            }

            var cart = GetCartFromSession();
            if (cart.Count == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add properties to schedule visits.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.Items = cart;
                return View("Index", model);
            }

            var bookingsToCreate = new List<SiteVisitBooking>();
            for (int i = 0; i < cart.Count; i++)
            {
                var submittedItem = model.Items?.FirstOrDefault(m => m.PropertyId == cart[i].PropertyId) ?? cart[i];

                var booking = new SiteVisitBooking
                {
                    PropertyId = cart[i].PropertyId,
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    VisitDate = submittedItem.VisitDate,
                    TimeSlot = submittedItem.TimeSlot ?? "10:00 AM - 11:30 AM",
                    Notes = submittedItem.Notes,
                    Status = BookingStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow
                };
                bookingsToCreate.Add(booking);

                var inquiry = new Inquiry
                {
                    PropertyId = cart[i].PropertyId,
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    Message = $"Scheduled site visit booking on {booking.VisitDate:yyyy-MM-dd} at {booking.TimeSlot}. Notes: {booking.Notes}",
                    InquiryDate = DateTime.UtcNow,
                    Status = InquiryStatus.ConvertedToSiteVisit
                };
                await _inquiryRepo.AddAsync(inquiry);
            }

            await _bookingRepo.AddRangeAsync(bookingsToCreate);
            HttpContext.Session.Remove(CartSessionKey);

            TempData["SuccessMessage"] = $"Success! {bookingsToCreate.Count} site visit(s) scheduled. View them in My Bookings!";
            return View("Confirmation", bookingsToCreate);
        }
    }
}

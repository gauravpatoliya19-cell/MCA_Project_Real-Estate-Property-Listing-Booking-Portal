using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealEstatePortal.Web.Areas.Customer.ViewModels;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Repositories.Interfaces;

namespace RealEstatePortal.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CatalogController : Controller
    {
        private readonly IPropertyRepository _propertyRepo;
        private readonly IInquiryRepository _inquiryRepo;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(
            IPropertyRepository propertyRepo,
            IInquiryRepository inquiryRepo,
            ILogger<CatalogController> logger)
        {
            _propertyRepo = propertyRepo;
            _inquiryRepo = inquiryRepo;
            _logger = logger;
        }

        public async Task<IActionResult> Index([FromQuery] PropertySearchVM search)
        {
            var allProps = await _propertyRepo.GetAllAsync();
            var query = allProps.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                var kw = search.Keyword.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(kw) ||
                                         p.Description.ToLower().Contains(kw) ||
                                         p.Address.ToLower().Contains(kw) ||
                                         p.City.ToLower().Contains(kw));
            }

            if (!string.IsNullOrWhiteSpace(search.City))
            {
                query = query.Where(p => p.City.Equals(search.City, StringComparison.OrdinalIgnoreCase));
            }

            if (search.PropertyType.HasValue)
            {
                query = query.Where(p => p.PropertyType == search.PropertyType.Value);
            }

            if (search.ListingType.HasValue)
            {
                query = query.Where(p => p.ListingType == search.ListingType.Value);
            }

            if (search.Status.HasValue)
            {
                query = query.Where(p => p.Status == search.Status.Value);
            }

            if (search.MinPrice.HasValue && search.MinPrice.Value > 0)
            {
                query = query.Where(p => p.Price >= search.MinPrice.Value);
            }

            if (search.MaxPrice.HasValue && search.MaxPrice.Value > 0)
            {
                query = query.Where(p => p.Price <= search.MaxPrice.Value);
            }

            if (search.Bedrooms.HasValue && search.Bedrooms.Value > 0)
            {
                query = query.Where(p => p.Bedrooms >= search.Bedrooms.Value);
            }

            if (search.Bathrooms.HasValue && search.Bathrooms.Value > 0)
            {
                query = query.Where(p => p.Bathrooms >= search.Bathrooms.Value);
            }

            // Sorting
            query = search.SortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "area_desc" => query.OrderByDescending(p => p.AreaSqFt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            search.Properties = query.ToList();
            search.TotalCount = search.Properties.Count();
            search.AvailableCities = await _propertyRepo.GetCitiesAsync();

            return View(search);
        }

        public async Task<IActionResult> Details(int id)
        {
            var property = await _propertyRepo.GetDetailsByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            ViewBag.InquirySuccess = TempData["InquirySuccess"];
            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitInquiry([Bind("PropertyId,CustomerName,CustomerEmail,CustomerPhone,Message")] Inquiry inquiry)
        {
            if (ModelState.IsValid)
            {
                inquiry.InquiryDate = DateTime.UtcNow;
                inquiry.Status = InquiryStatus.New;
                await _inquiryRepo.AddAsync(inquiry);

                TempData["SuccessMessage"] = "Your inquiry has been transmitted directly to the listing agent. We will contact you shortly!";
                return RedirectToAction(nameof(Details), new { id = inquiry.PropertyId });
            }

            TempData["ErrorMessage"] = "Please fill all required inquiry fields properly.";
            return RedirectToAction(nameof(Details), new { id = inquiry.PropertyId });
        }
    }
}

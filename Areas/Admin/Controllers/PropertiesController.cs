using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealEstatePortal.Web.Areas.Admin.ViewModels;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Repositories.Interfaces;

namespace RealEstatePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Agent")]
    public class PropertiesController : Controller
    {
        private readonly IPropertyRepository _propertyRepo;
        private readonly ILogger<PropertiesController> _logger;

        public PropertiesController(IPropertyRepository propertyRepo, ILogger<PropertiesController> logger)
        {
            _propertyRepo = propertyRepo;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? search, PropertyStatus? status, ListingType? listingType)
        {
            var properties = await _propertyRepo.GetAllAsync();
            var query = properties.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(s) ||
                                         p.City.ToLower().Contains(s) ||
                                         p.AgentName.ToLower().Contains(s));
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            if (listingType.HasValue)
            {
                query = query.Where(p => p.ListingType == listingType.Value);
            }

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentListingType = listingType;

            return View(query.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var property = await _propertyRepo.GetDetailsByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return View(property);
        }

        public IActionResult Create()
        {
            var vm = new PropertyAdminVM
            {
                Price = 500000m,
                TokenAmount = 10000m,
                CommissionRate = 3.0m,
                PrimaryImageUrl = "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?auto=format&fit=crop&w=1200&q=80",
                SelectedFeaturesComma = "24/7 Security, Swimming Pool, High-Speed Fiber Internet, Covered Parking, Balcony"
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyAdminVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var property = new Property
            {
                Title = vm.Title,
                Description = vm.Description,
                PropertyType = vm.PropertyType,
                ListingType = vm.ListingType,
                Status = vm.Status,
                Price = vm.Price,
                TokenAmount = vm.TokenAmount > 0 ? vm.TokenAmount : (vm.Price * 0.05m),
                Address = vm.Address,
                City = vm.City,
                State = vm.State,
                ZipCode = vm.ZipCode,
                Bedrooms = vm.Bedrooms,
                Bathrooms = vm.Bathrooms,
                AreaSqFt = vm.AreaSqFt,
                YearBuilt = vm.YearBuilt,
                IsFeatured = vm.IsFeatured,
                AgentName = vm.AgentName,
                AgentEmail = vm.AgentEmail,
                AgentPhone = vm.AgentPhone,
                CommissionRate = vm.CommissionRate,
                CreatedAt = DateTime.UtcNow
            };

            // Primary Image
            if (!string.IsNullOrWhiteSpace(vm.PrimaryImageUrl))
            {
                property.Images.Add(new PropertyImage
                {
                    ImageUrl = vm.PrimaryImageUrl.Trim(),
                    IsPrimary = true,
                    Caption = "Main View"
                });
            }

            // Additional Images
            if (!string.IsNullOrWhiteSpace(vm.AdditionalImageUrls))
            {
                var lines = vm.AdditionalImageUrls.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        property.Images.Add(new PropertyImage
                        {
                            ImageUrl = line.Trim(),
                            IsPrimary = false,
                            Caption = "Gallery View"
                        });
                    }
                }
            }

            // Features
            if (!string.IsNullOrWhiteSpace(vm.SelectedFeaturesComma))
            {
                var feats = vm.SelectedFeaturesComma.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var f in feats)
                {
                    property.Features.Add(new PropertyFeature
                    {
                        FeatureName = f.Trim(),
                        IconClass = "bi-check-circle-fill"
                    });
                }
            }

            await _propertyRepo.AddAsync(property);
            TempData["SuccessMessage"] = $"Property \"{property.Title}\" created successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var property = await _propertyRepo.GetByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var primaryImg = property.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl ?? property.Images.FirstOrDefault()?.ImageUrl;
            var additionalImgs = string.Join("\n", property.Images.Where(i => !i.IsPrimary).Select(i => i.ImageUrl));
            var featuresStr = string.Join(", ", property.Features.Select(f => f.FeatureName));

            var vm = new PropertyAdminVM
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                PropertyType = property.PropertyType,
                ListingType = property.ListingType,
                Status = property.Status,
                Price = property.Price,
                TokenAmount = property.TokenAmount,
                Address = property.Address,
                City = property.City,
                State = property.State,
                ZipCode = property.ZipCode,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                AreaSqFt = property.AreaSqFt,
                YearBuilt = property.YearBuilt,
                IsFeatured = property.IsFeatured,
                AgentName = property.AgentName,
                AgentEmail = property.AgentEmail,
                AgentPhone = property.AgentPhone,
                CommissionRate = property.CommissionRate,
                PrimaryImageUrl = primaryImg,
                AdditionalImageUrls = additionalImgs,
                SelectedFeaturesComma = featuresStr
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PropertyAdminVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var property = await _propertyRepo.GetByIdAsync(vm.Id);
            if (property == null)
            {
                return NotFound();
            }

            property.Title = vm.Title;
            property.Description = vm.Description;
            property.PropertyType = vm.PropertyType;
            property.ListingType = vm.ListingType;
            property.Status = vm.Status;
            property.Price = vm.Price;
            property.TokenAmount = vm.TokenAmount;
            property.Address = vm.Address;
            property.City = vm.City;
            property.State = vm.State;
            property.ZipCode = vm.ZipCode;
            property.Bedrooms = vm.Bedrooms;
            property.Bathrooms = vm.Bathrooms;
            property.AreaSqFt = vm.AreaSqFt;
            property.YearBuilt = vm.YearBuilt;
            property.IsFeatured = vm.IsFeatured;
            property.AgentName = vm.AgentName;
            property.AgentEmail = vm.AgentEmail;
            property.AgentPhone = vm.AgentPhone;
            property.CommissionRate = vm.CommissionRate;

            // Update primary image
            if (!string.IsNullOrWhiteSpace(vm.PrimaryImageUrl))
            {
                var primary = property.Images.FirstOrDefault(i => i.IsPrimary);
                if (primary != null)
                {
                    primary.ImageUrl = vm.PrimaryImageUrl.Trim();
                }
                else
                {
                    property.Images.Add(new PropertyImage { ImageUrl = vm.PrimaryImageUrl.Trim(), IsPrimary = true });
                }
            }

            await _propertyRepo.UpdateAsync(property);
            TempData["SuccessMessage"] = $"Property \"{property.Title}\" updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _propertyRepo.DeleteAsync(id);
            TempData["SuccessMessage"] = "Property removed successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, PropertyStatus status)
        {
            await _propertyRepo.UpdateStatusAsync(id, status);
            TempData["SuccessMessage"] = $"Status updated to {status}.";
            return RedirectToAction(nameof(Index));
        }
    }
}

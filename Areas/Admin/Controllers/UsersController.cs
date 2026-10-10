using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Web.Data;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Services.Security;
using RealEstatePortal.Web.ViewModels;

namespace RealEstatePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, string? role)
        {
            var query = _context.ApplicationUsers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(s) || u.Email.ToLower().Contains(s) || u.PhoneNumber.Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => u.Role == role);
            }

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentRole = role;

            var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserAdminVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserAdminVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existing = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower().Trim());
            if (existing != null)
            {
                ModelState.AddModelError("Email", "An account with this email address already exists.");
                return View(model);
            }

            var (hash, salt) = PasswordHasher.HashPassword(model.Password);

            var user = new ApplicationUser
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.ToLower().Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                Role = model.Role,
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.ApplicationUsers.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"User \"{user.FullName}\" created successfully with role \"{user.Role}\".";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var vm = new UserEditVM
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.ApplicationUsers.FindAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName.Trim();
            user.PhoneNumber = model.PhoneNumber.Trim();
            user.Role = model.Role;
            user.IsActive = model.IsActive;

            // Check if email changed and is unique
            if (user.Email.ToLower() != model.Email.ToLower().Trim())
            {
                var existing = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower().Trim() && u.Id != model.Id);
                if (existing != null)
                {
                    ModelState.AddModelError("Email", "Another account with this email already exists.");
                    return View(model);
                }
                user.Email = model.Email.ToLower().Trim();
            }

            // Optional password update
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var (hash, salt) = PasswordHasher.HashPassword(model.NewPassword);
                user.PasswordHash = hash;
                user.PasswordSalt = salt;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"User profile \"{user.FullName}\" updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"User status for {user.FullName} changed to {(user.IsActive ? "Active" : "Disabled")}.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user != null)
            {
                _context.ApplicationUsers.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"User \"{user.FullName}\" removed successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

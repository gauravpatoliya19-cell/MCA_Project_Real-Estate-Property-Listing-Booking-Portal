using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Web.Data;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Services.Security;
using RealEstatePortal.Web.ViewModels;

namespace RealEstatePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin") || User.IsInRole("Agent"))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginVM { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var input = (model.Email ?? string.Empty).Trim();
            var inputLower = input.ToLower();

            // Find matching admin by Email, FullName, or short name/alias
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => 
                u.Email.ToLower() == inputLower ||
                u.FullName.ToLower() == inputLower ||
                (inputLower.Contains("meet") && u.FullName.Contains("Meet")) ||
                (inputLower.Contains("gaurav") && u.FullName.Contains("Gaurav")) ||
                (inputLower.Contains("ronak") && u.FullName.Contains("Ronak")) ||
                (inputLower == "admin" && u.Role == "Admin"));

            // Auto-provision if missing
            if (user == null)
            {
                var (easyHash, easySalt) = PasswordHasher.HashPassword("123456");
                if (inputLower.Contains("meet") || inputLower == "admin@realestateportal.com" || inputLower == "admin")
                {
                    user = new ApplicationUser
                    {
                        FullName = "Chavda Meet",
                        Email = "meet@admin.com",
                        PhoneNumber = "+91 98250 11001",
                        Role = "Admin",
                        PasswordHash = easyHash,
                        PasswordSalt = easySalt,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.ApplicationUsers.Add(user);
                    await _context.SaveChangesAsync();
                }
                else if (inputLower.Contains("gaurav"))
                {
                    user = new ApplicationUser
                    {
                        FullName = "Gaurav Patoliya",
                        Email = "gaurav@admin.com",
                        PhoneNumber = "+91 98250 11002",
                        Role = "Admin",
                        PasswordHash = easyHash,
                        PasswordSalt = easySalt,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.ApplicationUsers.Add(user);
                    await _context.SaveChangesAsync();
                }
                else if (inputLower.Contains("ronak"))
                {
                    user = new ApplicationUser
                    {
                        FullName = "Jatapara Ronak",
                        Email = "ronak@admin.com",
                        PhoneNumber = "+91 98250 11003",
                        Role = "Admin",
                        PasswordHash = easyHash,
                        PasswordSalt = easySalt,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.ApplicationUsers.Add(user);
                    await _context.SaveChangesAsync();
                }
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "No administrative account found with this email or username.");
                return View(model);
            }

            if (user.Role != "Admin" && user.Role != "Agent")
            {
                ModelState.AddModelError(string.Empty, "Access Denied: This login portal is strictly reserved for Platform Administrators and Licensed Agents.");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "This administrator account has been disabled.");
                return View(model);
            }

            bool isValid = PasswordHasher.VerifyPassword(model.Password, user.PasswordHash, user.PasswordSalt);

            // Universal easy password fallback (123456, admin123, admin, 123, 1234, Admin@123, eadly, easily, password)
            var pwdTrimmed = (model.Password ?? string.Empty).Trim();
            var easyPasswords = new HashSet<string>(StringComparer.OrdinalIgnoreCase) 
            { 
                "123456", "admin123", "admin", "Admin@123", "123", "1234", "password", "eadly", "easily" 
            };

            if (!isValid && easyPasswords.Contains(pwdTrimmed))
            {
                var (newHash, newSalt) = PasswordHasher.HashPassword(pwdTrimmed);
                user.PasswordHash = newHash;
                user.PasswordSalt = newSalt;
                await _context.SaveChangesAsync();
                isValid = true;
            }

            if (!isValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid admin credentials. Please check your password.");
                return View(model);
            }

            // Sign in with Admin Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            TempData["SuccessMessage"] = $"Welcome back to Executive Console, {user.FullName} ({user.Role})!";

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Admin session ended successfully.";
            return RedirectToAction(nameof(Login));
        }
    }
}

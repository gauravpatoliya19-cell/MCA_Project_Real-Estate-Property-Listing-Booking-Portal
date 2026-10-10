using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Web.Areas.Customer.ViewModels;
using RealEstatePortal.Web.Data;

namespace RealEstatePortal.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class MyAccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MyAccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            var userName = User.Identity?.Name ?? "Valued Client";

            var userRecord = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == userEmail.ToLower());

            var bookings = await _context.SiteVisitBookings
                .Include(b => b.Property)
                .Where(b => b.CustomerEmail.ToLower() == userEmail.ToLower() || b.CustomerName.ToLower() == userName.ToLower())
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var payments = await _context.TokenPayments
                .Include(p => p.Property)
                .Where(p => p.CustomerEmail.ToLower() == userEmail.ToLower() || p.CustomerName.ToLower() == userName.ToLower())
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            var contracts = await _context.RentalContracts
                .Include(c => c.Property)
                .Where(c => c.TenantEmail.ToLower() == userEmail.ToLower() || c.TenantName.ToLower() == userName.ToLower())
                .OrderByDescending(c => c.GeneratedAt)
                .ToListAsync();

            var inquiries = await _context.Inquiries
                .Include(i => i.Property)
                .Where(i => i.CustomerEmail.ToLower() == userEmail.ToLower() || i.CustomerName.ToLower() == userName.ToLower())
                .OrderByDescending(i => i.InquiryDate)
                .ToListAsync();

            var vm = new UserDashboardVM
            {
                CustomerName = userName,
                CustomerEmail = userEmail,
                CustomerPhone = userRecord?.PhoneNumber ?? "",
                MyBookings = bookings,
                MyPayments = payments,
                MyContracts = contracts,
                MyInquiries = inquiries
            };

            return View(vm);
        }
    }
}

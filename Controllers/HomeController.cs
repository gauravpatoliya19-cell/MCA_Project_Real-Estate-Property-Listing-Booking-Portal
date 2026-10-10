using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Repositories.Interfaces;

namespace RealEstatePortal.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPropertyRepository _propertyRepo;

        public HomeController(IPropertyRepository propertyRepo)
        {
            _propertyRepo = propertyRepo;
        }

        public async Task<IActionResult> Index()
        {
            var featured = await _propertyRepo.GetFeaturedAsync();
            return View(featured);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

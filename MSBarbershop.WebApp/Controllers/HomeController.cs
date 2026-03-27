using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSBarbershop.Core.ViewModels.Home;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.WebApp.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MSBarbershop.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
         public readonly ApplicationDbContext _context;
        private readonly IReviewService _reviewService;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context, IReviewService reviewService)
        {
            _logger = logger;
            _context = context;
            _reviewService = reviewService;
        }
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.ToListAsync();

            var barbers = await _context.Barbers
                .Where(b => b.IsActive)
                .ToListAsync();

            var reviews = await _reviewService.GetApprovedReviews();

            var model = new HomeIndexViewModel
            {
                Services = services,
                Barbers = barbers,
                Reviews = reviews
            };

            return View(model);
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

        //[Authorize(Roles = "Customer")]
        public IActionResult AboutUs()
        {
            return View();
        }


        [Authorize(Roles = "Customer")]
         public async Task<IActionResult> OurBarbers()
            {
             var barbers = await _context.Barbers.ToListAsync();
             return View(barbers);
            }

}
}

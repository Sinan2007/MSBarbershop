using Microsoft.AspNetCore.Mvc;
using MSBarbershop.WebApp.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MSBarbershop.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;

namespace MSBarbershop.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
         public readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
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

        [Authorize(Roles = "Customer")]
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

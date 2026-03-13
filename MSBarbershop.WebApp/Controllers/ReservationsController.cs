using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.Data.Entities.Enums;
using MSBarbershop.WebApp.Services.Reservations;
using MSBarbershop.WebApp.ViewModels.Reservation;
using System.Security.Claims;

namespace MSBarbershop.WebApp.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IReservationService _reservationService;

        public ReservationsController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IReservationService reservationService)
        {
            _context = context;
            _userManager = userManager;
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id = null)
        {
            var model = new CreateReservationViewModel
            {
                Barbers = await _context.Barbers
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.FullName
                    }).ToListAsync(),

                Services = await _context.Services
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToListAsync()
            };

            // Ако Admin може да избира клиент
            if (User.IsInRole("Admin"))
            {
                model.Users = await _userManager.Users
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id,
                        Text = u.Email
                    }).ToListAsync();
            }

            if (id.HasValue)
            {
                model.BarberId = id.Value;
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(model);
                return View(model);
            }

            string userId;

            if (User.IsInRole("Admin") && !string.IsNullOrEmpty(model.UserId))
            {
                userId = model.UserId;
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                userId = user.Id;
            }

            try
            {
                await _reservationService.CreateReservation(userId, model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await LoadDropdowns(model);
                return View(model);
            }

            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(AllReservations));

            return RedirectToAction(nameof(MyReservations));
        }


        private async Task LoadDropdowns(CreateReservationViewModel model)
        {
            model.Barbers = await _context.Barbers
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.FullName
                }).ToListAsync();

            model.Services = await _context.Services
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToListAsync();

            if (User.IsInRole("Admin"))
            {
                model.Users = await _userManager.Users
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id,
                        Text = u.Email
                    }).ToListAsync();
            }
        }

        public async Task<IActionResult> BarberReservations()
        {
            var user = await _userManager.GetUserAsync(User);

            var barber = await _context.Barbers
                .FirstOrDefaultAsync(b => b.UserId == user.Id);

            if (barber == null)
                return NotFound();

            var reservations = await _reservationService
                .GetBarberReservations(barber.Id);

            return View(reservations);
        }




        public async Task<IActionResult> MyReservations()
        {
            var user = await _userManager.GetUserAsync(User);

            var reservations = await _reservationService
    .GetUserReservations(user.Id);


            return View(reservations);
        }


        [Authorize(Roles = "Admin , Barber")]
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
         await _reservationService.CancelReservation(id);

         return RedirectToAction(nameof(AllReservations));
         

        }

        [Authorize(Roles = "Admin , Barber")]
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            await _reservationService.CompleteReservation(id);

            return RedirectToAction(nameof(AllReservations));
        }


        [Authorize(Roles = "Admin,Barber")]
        public async Task<IActionResult> AllReservations()
        {

            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Barber)
                .Include(r => r.Service)
                .ToListAsync();

            return View(reservations);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int barberId, int serviceId, DateOnly date)
        {
            var slots = await _reservationService
                .GetAvailableSlots(barberId, serviceId, date);

            return Json(slots);
        }


    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSBarbershop.Core.IServices;
using MSBarbershop.Core.ViewModels.Reservation;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
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

            TempData["SuccessMessage"] = "Your reservation was created successfully.";

            
                var user = await _userManager.GetUserAsync(User);
                userId = user.Id;
            

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = new MyReservationsPageViewModel
            {
                Upcoming = await _reservationService.GetUpcomingReservations(userId),
                Past = await _reservationService.GetPastReservations(userId)
            };

            return View(model);
        }


        [Authorize(Roles = "Admin,Barber,Customer")]
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reservation = await _context.Reservations
                .Include(r => r.Barber)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            if (User.IsInRole("Customer") && reservation.UserId != userId)
                return Forbid();

            if (User.IsInRole("Barber"))
            {
                var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.UserId == userId);

                if (barber == null || reservation.BarberId != barber.Id)
                    return Forbid();
            }

            TempData["ErrorMessage"] = "Reservation cancelled successfully.";

            await _reservationService.CancelReservation(id);

            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(AllReservations));

            if (User.IsInRole("Barber"))
                return RedirectToAction(nameof(BarberReservations));

            return RedirectToAction(nameof(MyReservations));
        }

        [Authorize(Roles = "Admin , Barber")]
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {

            TempData["SuccessMessage"] = "Reservation completed successfully.";

            await _reservationService.CompleteReservation(id);
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction(nameof(AllReservations));
            }
            else
            {
                return RedirectToAction(nameof(BarberDashboard));
            }


        }


        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Barber")]
        public async Task<IActionResult> BarberDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var barber = await _context.Barbers
                .FirstOrDefaultAsync(b => b.UserId == userId);

            if (barber == null)
                return NotFound();

            var upcoming = await _reservationService
                .GetBarberUpcomingReservations(barber.Id);

            var past = await _reservationService
                .GetBarberPastReservations(barber.Id);

            var model = new MyReservationsPageViewModel
            {
                Upcoming = upcoming,
                Past = past
            };

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _reservationService.GetReservationForEditAsync(id, userId);
            if (model == null)
                return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditReservationViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
            {
                await _reservationService.LoadEditReservationDropdownsAsync(model);
                return View(model);
            }
            var success = await _reservationService.UpdateReservationAsync(model, userId);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to update reservation. The selected time may already be taken.");
                await _reservationService.LoadEditReservationDropdownsAsync(model);
                return View(model);
            }
            return RedirectToAction(nameof(MyReservations));
        }






    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.Data.Entities.Enums;
using MSBarbershop.WebApp.ViewModels.Reservation;
using System.Security.Claims;

namespace MSBarbershop.WebApp.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReservationsController(
            ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
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

            var reservation = new Reservation
            {
                UserId = userId,
                BarberId = model.BarberId,
                ServiceId = model.ServiceId,
                Date = model.Date,
                Time=model.Time,
                Status = ReservationStatus.Active
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

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



        public async Task<IActionResult> MyReservations()
        {
            var user = await _userManager.GetUserAsync(User);

            var reservations = await _context.Reservations
                .Include(r => r.Barber)
                .Include(r => r.Service)
                .Where(r => r.UserId == user.Id)
                .ToListAsync();

            return View(reservations);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
                return NotFound();

            // Проверка дали е неговата резервация
            if (reservation.UserId != user.Id && !User.IsInRole("Admin"))
                return Unauthorized();

            reservation.Status = ReservationStatus.Cancelled;

            await _context.SaveChangesAsync();

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AllReservations");
            }
            return RedirectToAction(nameof(MyReservations));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
                return NotFound();

            reservation.Status = ReservationStatus.Completed;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllReservations));
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

    }
}

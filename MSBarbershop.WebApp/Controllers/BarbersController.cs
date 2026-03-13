using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSBarbershop.WebApp.Services.Barbers;
using MSBarbershop.WebApp.ViewModels.Barber;

namespace MSBarbershop.WebApp.Controllers
{
    public class BarbersController : Controller
    {
        private readonly IBarberService _barberService;

        public BarbersController(IBarberService barberService)
        {
            _barberService = barberService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var barbers = await _barberService.GetAllBarbers();

            return View(barbers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var barber = await _barberService.GetBarberById(id);

            if (barber == null)
                return NotFound();

            var completedReservations =
                await _barberService.GetCompletedReservationsCount(id);

            ViewBag.CompletedReservations = completedReservations;

            return View(barber);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBarberViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _barberService.CreateBarber(model);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var barber = await _barberService.GetBarberById(id);

            if (barber == null)
                return NotFound();

            var model = new EditBarberViewModel
            {
                Id = barber.Id,
                FullName = barber.FullName,
                PhoneNumber = barber.PhoneNumber,
                Description = barber.Description,
                IsActive = barber.IsActive,
                ImageUrl = barber.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditBarberViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            await _barberService.UpdateBarber(model);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var barber = await _barberService.GetBarberById(id);

            if (barber == null)
                return NotFound();

            return View(barber);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _barberService.DeleteBarber(id);

            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.WebApp.ViewModels.Barber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSBarbershop.WebApp.Controllers
{
    public class BarbersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BarbersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Barbers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Barbers.ToListAsync());
        }

        // GET: Barbers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var barber = await _context.Barbers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (barber == null)
            {
                return NotFound();
            }

            return View(barber);
        }

        // GET: Barbers/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBarberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var barber = new Barber
            {
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Description = model.Description,
                IsActive = model.IsActive
            };

            _context.Barbers.Add(barber);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var barber = await _context.Barbers.FindAsync(id);

            if (barber == null)
                return NotFound();

            var model = new EditBarberViewModel
            {
                Id = barber.Id,
                FullName = barber.FullName,
                PhoneNumber = barber.PhoneNumber,
                Description = barber.Description,
                IsActive = barber.IsActive
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

            var barber = await _context.Barbers.FindAsync(id);

            if (barber == null)
                return NotFound();

            barber.FullName = model.FullName;
            barber.PhoneNumber = model.PhoneNumber;
            barber.Description = model.Description;
            barber.IsActive = model.IsActive;

            _context.Update(barber);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Barbers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var barber = await _context.Barbers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (barber == null)
            {
                return NotFound();
            }

            return View(barber);
        }

        // POST: Barbers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var barber = await _context.Barbers.FindAsync(id);
            if (barber != null)
            {
                _context.Barbers.Remove(barber);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BarberExists(int id)
        {
            return _context.Barbers.Any(e => e.Id == id);
        }
    }
}

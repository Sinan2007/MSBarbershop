using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.WebApp.ViewModels.WorkSchedule;

namespace MSBarbershop.WebApp.Controllers
{
    public class WorkSchedulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkSchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkSchedules
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WorkSchedules.Include(w => w.Barber);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: WorkSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSchedule = await _context.WorkSchedules
                .Include(w => w.Barber)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workSchedule == null)
            {
                return NotFound();
            }

            return View(workSchedule);
        }

        // GET: WorkSchedules/Create
        public IActionResult Create()
        {
            var model = new CreateWorkScheduleViewModel
            {
                Barbers = GetBarbers()
            };

            return View(model);
        }

        // POST: WorkSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(CreateWorkScheduleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Barbers = GetBarbers();
                return View(model);
            }

            var schedule = new WorkSchedule
            {
                BarberId = model.BarberId,
                DayOfWeek = model.DayOfWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            };

            _context.WorkSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GetBarbers vzima barberite za dropdown
        private IEnumerable<SelectListItem> GetBarbers()
        {
            return _context.Barbers
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.FullName
                })
                .ToList();
        }


        // GET: WorkSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSchedule = await _context.WorkSchedules.FindAsync(id);
            if (workSchedule == null)
            {
                return NotFound();
            }
            ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "Description", workSchedule.BarberId);
            return View(workSchedule);
        }

        // POST: WorkSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BarberId,DayOfWeek,StartTime,EndTime")] WorkSchedule workSchedule)
        {
            if (id != workSchedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkScheduleExists(workSchedule.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "Description", workSchedule.BarberId);
            return View(workSchedule);
        }

        // GET: WorkSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSchedule = await _context.WorkSchedules
                .Include(w => w.Barber)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workSchedule == null)
            {
                return NotFound();
            }

            return View(workSchedule);
        }

        // POST: WorkSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workSchedule = await _context.WorkSchedules.FindAsync(id);
            if (workSchedule != null)
            {
                _context.WorkSchedules.Remove(workSchedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkScheduleExists(int id)
        {
            return _context.WorkSchedules.Any(e => e.Id == id);
        }
    }
}

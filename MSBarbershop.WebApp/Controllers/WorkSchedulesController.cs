using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSBarbershop.Core.IServices;
using MSBarbershop.Core.ViewModels.WorkSchedule;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.WebApp.ViewModels.WorkSchedule;

public class WorkSchedulesController : Controller
{
    private readonly IWorkScheduleService _service;
    private readonly ApplicationDbContext _context;

    public WorkSchedulesController(IWorkScheduleService service,ApplicationDbContext context)
    {
        _service = service;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var schedules = await _service.GetAll();
        return View(schedules);
    }

    public async Task<IActionResult> Details(int id)
    {
        var schedule = await _service.GetById(id);

        if (schedule == null)
            return NotFound();

        return View(schedule);
    }

    public async Task<IActionResult> Create()
    {
        var model = new CreateWorkScheduleViewModel
        {
            Barbers = await _service.GetBarbersDropdown()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkScheduleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Barbers = await _service.GetBarbersDropdown();
            return View(model);
        }

        await _service.Create(model);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
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

        var model = new EditWorkScheduleViewModel
        {
            Id = workSchedule.Id,
            StartTime = workSchedule.StartTime,
            EndTime = workSchedule.EndTime
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditWorkScheduleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existingSchedule = await _context.WorkSchedules.FindAsync(model.Id);

        if (existingSchedule == null)
        {
            return NotFound();
        }

        existingSchedule.StartTime = model.StartTime;
        existingSchedule.EndTime = model.EndTime;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var schedule = await _service.GetById(id);

        if (schedule == null)
            return NotFound();

        return View(schedule);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.Delete(id);

        return RedirectToAction(nameof(Index));
    }
}

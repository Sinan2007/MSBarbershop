using Microsoft.AspNetCore.Mvc;
using MSBarbershop.Data.Entities;
using MSBarbershop.WebApp.Services.WorkSchedules;
using MSBarbershop.WebApp.ViewModels.WorkSchedule;

public class WorkSchedulesController : Controller
{
    private readonly IWorkScheduleService _service;

    public WorkSchedulesController(IWorkScheduleService service)
    {
        _service = service;
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

    public async Task<IActionResult> Edit(int id)
    {
        var schedule = await _service.GetById(id);

        if (schedule == null)
            return NotFound();

        return View(schedule);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, WorkSchedule model)
    {
        if (id != model.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        await _service.Update(model);

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

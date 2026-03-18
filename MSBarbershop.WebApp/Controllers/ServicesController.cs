using Microsoft.AspNetCore.Mvc;
using MSBarbershop.WebApp.ViewModels.Service;

public class ServicesController : Controller
{
    private readonly IServiceService _service;

    public ServicesController(IServiceService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _service.GetAll());
    }

    public async Task<IActionResult> Details(int id)
    {
        var service = await _service.GetById(id);

        if (service == null)
            return NotFound();

        return View(service);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateServiceViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _service.Create(model);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var service = await _service.GetById(id);

        if (service == null)
            return NotFound();

        var model = new EditServiceViewModel
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            Price = service.Price,
            DurationMinutes = service.DurationMinutes
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, EditServiceViewModel model)
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
        var service = await _service.GetById(id);

        if (service == null)
            return NotFound();

        return View(service);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.Delete(id);

        return RedirectToAction(nameof(Index));
    }
}

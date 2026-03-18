using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.WebApp.Services.WorkSchedules;
using MSBarbershop.WebApp.ViewModels.WorkSchedule;

public class WorkScheduleService : IWorkScheduleService
{
    private readonly ApplicationDbContext _context;

    public WorkScheduleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkSchedule>> GetAll()
    {
        return await _context.WorkSchedules
            .Include(w => w.Barber)
            .ToListAsync();
    }

    public async Task<WorkSchedule?> GetById(int id)
    {
        return await _context.WorkSchedules
            .Include(w => w.Barber)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task Create(CreateWorkScheduleViewModel model)
    {
        var schedule = new WorkSchedule
        {
            BarberId = model.BarberId,
            DayOfWeek = model.DayOfWeek,
            StartTime = model.StartTime,
            EndTime = model.EndTime
        };

        _context.WorkSchedules.Add(schedule);
        await _context.SaveChangesAsync();
    }

    public async Task Update(WorkSchedule schedule)
    {
        _context.WorkSchedules.Update(schedule);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var schedule = await _context.WorkSchedules.FindAsync(id);

        if (schedule == null)
            return;

        _context.WorkSchedules.Remove(schedule);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Exists(int id)
    {
        return await _context.WorkSchedules.AnyAsync(w => w.Id == id);
    }

    public async Task<List<SelectListItem>> GetBarbersDropdown()
    {
        return await _context.Barbers
            .Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.FullName
            })
            .ToListAsync();
    }
}

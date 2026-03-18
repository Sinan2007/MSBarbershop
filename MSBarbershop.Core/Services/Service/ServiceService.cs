using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.WebApp.ViewModels.Service;

public class ServiceService : IServiceService
{
    private readonly ApplicationDbContext _context;

    public ServiceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Service>> GetAll()
    {
        return await _context.Services.ToListAsync();
    }

    public async Task<Service?> GetById(int id)
    {
        return await _context.Services
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task Create(CreateServiceViewModel model)
    {
        var service = new Service
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            DurationMinutes = model.DurationMinutes
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();
    }

    public async Task Update(EditServiceViewModel model)
    {
        var service = await _context.Services.FindAsync(model.Id);

        if (service == null)
            return;

        service.Name = model.Name;
        service.Description = model.Description;
        service.Price = model.Price;
        service.DurationMinutes = model.DurationMinutes;

        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var service = await _context.Services.FindAsync(id);

        if (service == null)
            return;

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Exists(int id)
    {
        return await _context.Services.AnyAsync(s => s.Id == id);
    }
}

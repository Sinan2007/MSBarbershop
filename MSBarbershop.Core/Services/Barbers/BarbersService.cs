using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.Data.Entities.Enums;
using MSBarbershop.WebApp.ViewModels.Barber;


namespace MSBarbershop.Core.Services.Barbers
{
    public class BarbersService : IBarberService
    {
        private readonly ApplicationDbContext _context;

        public BarbersService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Barber>> GetAllBarbers()
        {
            return await _context.Barbers.ToListAsync();
        }

        public async Task<Barber?> GetBarberById(int id)
        {
            return await _context.Barbers
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<int> GetCompletedReservationsCount(int barberId)
        {
            return await _context.Reservations
                .Where(r =>
                    r.BarberId == barberId &&
                    r.Status == ReservationStatus.Completed)
                .CountAsync();
        }

        public async Task CreateBarber(CreateBarberViewModel model)
        {
            var barber = new Barber
            {
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                IsActive = model.IsActive
            };

            _context.Barbers.Add(barber);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateBarber(EditBarberViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            var barber = await _context.Barbers.FindAsync(model.Id);

            if (barber == null)
                return;

            barber.FullName = model.FullName;
            barber.PhoneNumber = model.PhoneNumber;
            barber.Description = model.Description;
            barber.IsActive = model.IsActive;
            barber.ImageUrl = model.ImageUrl;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBarber(int id)
        {
            var barber = await _context.Barbers.FindAsync(id);

            if (barber == null)
                return;

            _context.Barbers.Remove(barber);

            await _context.SaveChangesAsync();
        }
    }
}

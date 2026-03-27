using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.Data.Entities.Enums;
using MSBarbershop.WebApp.ViewModels.Reservation;

namespace MSBarbershop.WebApp.Services.Reservations
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using MSBarbershop.Core.ViewModels.Reservation;
    using MSBarbershop.Data;
    using MSBarbershop.Data.Entities;
    using MSBarbershop.Data.Entities.Enums;
    using MSBarbershop.WebApp.ViewModels.Reservation;

    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MyReservationViewModel>> GetUserReservations(string userId)
        {
            var reservations = await _context.Reservations
    .Include(r => r.Barber)
    .Include(r => r.Service)
    .Where(r => r.UserId == userId)
    .ToListAsync();

            return reservations.Select(r => new MyReservationViewModel
            {
                Id = r.Id,
                BarberName = r.Barber.FullName,
                ServiceName = r.Service.Name,
                Date = r.Date,
                Time = r.Time,
                EndTime = r.Time.AddMinutes(r.Service.DurationMinutes),
                Status = r.Status
            }).ToList();
        }

        public async Task<List<Reservation>> GetBarberReservations(int barberId)
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Service)
                .Where(r => r.BarberId == barberId)

                .ToListAsync();
        }

        public async Task<List<string>> GetAvailableSlots(int barberId, int serviceId, DateOnly date)
        {
            var service = await _context.Services.FindAsync(serviceId);

            var schedule = await _context.WorkSchedules
                .FirstOrDefaultAsync(s =>
                    s.BarberId == barberId &&
                    s.DayOfWeek == date.DayOfWeek);

            if (schedule == null)
                return new List<string>();

            var reservations = await _context.Reservations
                .Include(r => r.Service)
                .Where(r =>
                    r.BarberId == barberId &&
                    r.Date == date &&
                    r.Status == ReservationStatus.Active)
                .ToListAsync();

            var slots = new List<string>();

            var duration = TimeSpan.FromMinutes(service.DurationMinutes);

            var current = schedule.StartTime;

            while (current + duration <= schedule.EndTime)
            {
                var slotStart = current;
                var slotEnd = current + duration;

                bool taken = reservations.Any(r =>
                {
                    var rStart = r.Time.ToTimeSpan();
                    var rEnd = rStart + TimeSpan.FromMinutes(r.Service.DurationMinutes);

                    return slotStart < rEnd && slotEnd > rStart;
                });

                if (!taken)
                {
                    slots.Add(slotStart.ToString(@"hh\:mm"));
                }

                current = current.Add(TimeSpan.FromMinutes(15));
            }

            return slots;
        }

        public async Task CreateReservation(string userId, CreateReservationViewModel model)
        {

            var today = DateOnly.FromDateTime(DateTime.Today);

            if (model.Date < today)
            {
                throw new Exception("You cannot book an appointment for a past date.");
            }
            var service = await _context.Services.FindAsync(model.ServiceId);

            var existing = await _context.Reservations
                .Include(r => r.Service)
                .Where(r =>
                    r.BarberId == model.BarberId &&
                    r.Date == model.Date &&
                    r.Status == ReservationStatus.Active)
                .ToListAsync();

            var newStart = model.Time.ToTimeSpan();
            var newEnd = newStart + TimeSpan.FromMinutes(service.DurationMinutes);

            bool conflict = existing.Any(r =>
            {
                var rStart = r.Time.ToTimeSpan();
                var rEnd = rStart + TimeSpan.FromMinutes(r.Service.DurationMinutes);

                return newStart < rEnd && newEnd > rStart;
            });

            if (conflict)
            {
                throw new Exception("This time slot is already taken.");
            }

            
            var reservation = new Reservation
            {
                UserId = userId,
                BarberId = model.BarberId,
                ServiceId = model.ServiceId,
                Date = model.Date,
                Time = model.Time,
                Status = ReservationStatus.Active
            };

            _context.Reservations.Add(reservation);

            await _context.SaveChangesAsync();
        }

        public async Task CancelReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
                return;

            reservation.Status = ReservationStatus.Cancelled;

            await _context.SaveChangesAsync();
        }

        public async Task CompleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
                return;

            reservation.Status = ReservationStatus.Completed;

            await _context.SaveChangesAsync();
        }

        public async Task<List<MyReservationViewModel>> GetMyReservations(string userId)
        {
            return await _context.Reservations
                .Where(r => r.UserId == userId)
                .Include(r => r.Barber)
                .Include(r => r.Service)
                .Select(r => new MyReservationViewModel
                {
                    Id = r.Id,
                    BarberName = r.Barber.FullName,
                    ServiceName = r.Service.Name,
                    Date = r.Date,
                    Time = r.Time,
                    Status = r.Status
                }).OrderBy(r => r.Status != ReservationStatus.Active)
                  .ThenBy(r => r.Date)
                  .ThenBy(r => r.Time)
                .ToListAsync();
        }

        public async Task<List<MyReservationViewModel>> GetUpcomingReservations(string userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var reservations = await _context.Reservations
                .Where(r => r.UserId == userId &&
                       r.Status == ReservationStatus.Active &&
                       r.Date >= today)
                .Include(r => r.Barber)
                .Include(r => r.Service)
                .ToListAsync();

            return reservations
                .Select(r => new MyReservationViewModel
                {
                    Id = r.Id,
                    BarberName = r.Barber.FullName,
                    ServiceName = r.Service.Name,
                    Date = r.Date,
                    Time = r.Time,
                    EndTime = r.Time.AddMinutes(r.Service.DurationMinutes),
                    Status = r.Status
                })
                .OrderBy(r => r.Date)
                .ThenBy(r => r.Time)
                .ToList();
        }

        public async Task<List<MyReservationViewModel>> GetPastReservations(string userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var reservations = await _context.Reservations
                .Where(r => r.UserId == userId &&
                       (r.Status != ReservationStatus.Active || r.Date < today))
                .Include(r => r.Barber)
                .Include(r => r.Service)
                .ToListAsync();

            return reservations
                .Select(r => new MyReservationViewModel
                {
                    Id = r.Id,
                    BarberName = r.Barber.FullName,
                    ServiceName = r.Service.Name,
                    Date = r.Date,
                    Time = r.Time,
                    EndTime = r.Time.AddMinutes(r.Service.DurationMinutes),
                    Status = r.Status
                })
                .OrderByDescending(r => r.Date)
                .ThenByDescending(r => r.Time)
                .ToList();
        }

        public async Task<List<MyReservationViewModel>> GetBarberUpcomingReservations(int barberId)
        {
            var reservations = await _context.Reservations
                .Where(r => r.BarberId == barberId && r.Status == ReservationStatus.Active)
                .Include(r => r.Service)
                .Include(r => r.Barber)
                .Include(r => r.User)
                .ToListAsync();

            

            return reservations
                .Select(r => new MyReservationViewModel
                {
                    Id = r.Id,
                    BarberName = r.User.FirstName + " " + r.User.LastName,
                    ServiceName = r.Service.Name,
                    Date = r.Date,
                    Time = r.Time,
                    EndTime = r.Time.AddMinutes(r.Service.DurationMinutes),
                    Status = r.Status
                })
                .OrderBy(r => r.Date)
                .ThenBy(r => r.Time)
                .ToList();
        }


        public async Task<List<MyReservationViewModel>> GetBarberPastReservations(int barberId)
        {
            var reservations = await _context.Reservations
                .Where(r => r.BarberId == barberId &&
                       (r.Status == ReservationStatus.Completed ||
                        r.Status == ReservationStatus.Cancelled))
                .Include(r => r.Service)
                .Include(r => r.Barber)
                .Include(r => r.User)
                .ToListAsync();

            return reservations
                .Select(r => new MyReservationViewModel
                {
                    Id = r.Id,
                    BarberName = r.User.FirstName + " " + r.User.LastName,
                    ServiceName = r.Service.Name,
                    Date = r.Date,
                    Time = r.Time,
                    EndTime = r.Time.AddMinutes(r.Service.DurationMinutes),
                    Status = r.Status
                })
                .OrderByDescending(r => r.Date)
                .ThenByDescending(r => r.Time)
                .ToList();
        }

        public async Task<EditReservationViewModel?> GetReservationForEditAsync(int reservationId, string userId)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.UserId == userId);
            if (reservation == null)
                return null;
            var model = new EditReservationViewModel
            {
                Id = reservation.Id,
                BarberId = reservation.BarberId,
                ServiceId = reservation.ServiceId,
                Date = reservation.Date,
                Time = reservation.Time,
                Barbers = await _context.Barbers
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.FullName
                    })
                    .ToListAsync(),
                Services = await _context.Services
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    })
                    .ToListAsync()
            };
            return model;
        }

        public async Task<bool> UpdateReservationAsync(EditReservationViewModel model, string userId)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == model.Id && r.UserId == userId);
            if (reservation == null)
                return false;
            if (reservation.Status != ReservationStatus.Active)
                return false;
            var service = await _context.Services.FindAsync(model.ServiceId);
            if (service == null)
                return false;

            var today = DateOnly.FromDateTime(DateTime.Today);

            if (model.Date < today)
            {
                throw new Exception("You cannot book an appointment for a past date.");
            }
            var existingReservations = await _context.Reservations
                .Include(r => r.Service)
                .Where(r =>
                    r.Id != model.Id &&
                    r.BarberId == model.BarberId &&
                    r.Date == model.Date &&
                    r.Status == ReservationStatus.Active)
                .ToListAsync();
            var newStart = model.Time.ToTimeSpan();
            var newEnd = newStart + TimeSpan.FromMinutes(service.DurationMinutes);
            bool conflict = existingReservations.Any(r =>
            {
                var existingStart = r.Time.ToTimeSpan();
                var existingEnd = existingStart + TimeSpan.FromMinutes(r.Service.DurationMinutes);
                return newStart < existingEnd && newEnd > existingStart;
            });
            if (conflict)
                return false;
            reservation.BarberId = model.BarberId;
            reservation.ServiceId = model.ServiceId;
            reservation.Date = model.Date;
            reservation.Time = model.Time;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task LoadEditReservationDropdownsAsync(EditReservationViewModel model)
        {
            model.Barbers = await _context.Barbers
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.FullName
                })
                .ToListAsync();
            model.Services = await _context.Services
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToListAsync();
        }


    }


}

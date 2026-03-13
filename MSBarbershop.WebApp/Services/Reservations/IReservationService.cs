using MSBarbershop.Data.Entities;
using MSBarbershop.WebApp.ViewModels.Reservation;

namespace MSBarbershop.WebApp.Services.Reservations
{
    public interface IReservationService
    {
        Task<List<Reservation>> GetUserReservations(string userId);

        Task<List<Reservation>> GetBarberReservations(int barberId);

        Task<List<string>> GetAvailableSlots(int barberId, int serviceId, DateOnly date);

        Task CreateReservation(string userId, CreateReservationViewModel model);

        Task CancelReservation(int id);

        Task CompleteReservation(int id);
    }

}

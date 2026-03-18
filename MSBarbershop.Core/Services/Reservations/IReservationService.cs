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

        Task<List<MyReservationViewModel>> GetMyReservations(string userId);

        Task<List<MyReservationViewModel>> GetPastReservations(string userId);
        Task<List<MyReservationViewModel>> GetUpcomingReservations(string userId);

        Task<List<MyReservationViewModel>> GetBarberUpcomingReservations(int barberId);
        Task<List<MyReservationViewModel>> GetBarberPastReservations(int barberId);

    }

}

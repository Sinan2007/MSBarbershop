using MSBarbershop.Data.Entities;
using MSBarbershop.WebApp.ViewModels.Barber;

namespace MSBarbershop.Core.Services.Barbers
{
    public interface IBarberService
    {
        Task<List<Barber>> GetAllBarbers();

        Task<Barber?> GetBarberById(int id);

        Task<int> GetCompletedReservationsCount(int barberId);

        Task CreateBarber(CreateBarberViewModel model);

        Task UpdateBarber(EditBarberViewModel model);

        Task DeleteBarber(int id);
    }
}
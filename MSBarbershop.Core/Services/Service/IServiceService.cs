using MSBarbershop.Data.Entities;
using MSBarbershop.WebApp.ViewModels.Service;

public interface IServiceService
{
    Task<List<Service>> GetAll();

    Task<Service?> GetById(int id);

    Task Create(CreateServiceViewModel model);

    Task Update(EditServiceViewModel model);

    Task Delete(int id);

    Task<bool> Exists(int id);
}

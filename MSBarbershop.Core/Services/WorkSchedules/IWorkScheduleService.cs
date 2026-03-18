namespace MSBarbershop.WebApp.Services.WorkSchedules
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using MSBarbershop.Data.Entities;
    using MSBarbershop.WebApp.ViewModels.WorkSchedule;

    public interface IWorkScheduleService
    {
        Task<List<WorkSchedule>> GetAll();

        Task<WorkSchedule?> GetById(int id);

        Task Create(CreateWorkScheduleViewModel model);

        Task Update(WorkSchedule schedule);

        Task Delete(int id);

        Task<bool> Exists(int id);

        Task<List<SelectListItem>> GetBarbersDropdown();
    }

}

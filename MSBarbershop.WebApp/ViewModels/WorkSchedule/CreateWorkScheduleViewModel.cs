using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MSBarbershop.WebApp.ViewModels.WorkSchedule
{
    public class CreateWorkScheduleViewModel
    {
        [Required]
        public int BarberId { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        public IEnumerable<SelectListItem> Barbers { get; set; } = new List<SelectListItem>();
    }
}

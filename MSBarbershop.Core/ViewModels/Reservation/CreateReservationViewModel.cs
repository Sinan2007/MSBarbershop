using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MSBarbershop.WebApp.ViewModels.Reservation
{
    public class CreateReservationViewModel
    {
        [Required]
        public int BarberId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public TimeOnly Time { get; set; }
        public IEnumerable<SelectListItem> Barbers { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Services { get; set; } = new List<SelectListItem>();

        public string? UserId { get; set; }
        public IEnumerable<SelectListItem>? Users { get; set; } = new List<SelectListItem>();
    }

}

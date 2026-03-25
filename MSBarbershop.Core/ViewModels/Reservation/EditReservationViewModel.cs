using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace MSBarbershop.Core.ViewModels.Reservation
{
    public class EditReservationViewModel
    {
        public int Id { get; set; }
        [Required]
        public int BarberId { get; set; }
        [Required]
        public int ServiceId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateOnly Date { get; set; }
        [Required]
        public TimeOnly Time { get; set; }
        public IEnumerable<SelectListItem> Barbers { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Services { get; set; } = new List<SelectListItem>();
    }
}
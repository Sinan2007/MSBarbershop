using System.ComponentModel.DataAnnotations;

namespace MSBarbershop.WebApp.ViewModels.Service
{
    public class CreateServiceViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int DurationMinutes { get; set; }
    }
}

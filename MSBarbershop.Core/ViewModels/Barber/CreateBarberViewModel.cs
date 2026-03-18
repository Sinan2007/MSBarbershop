using System.ComponentModel.DataAnnotations;

namespace MSBarbershop.WebApp.ViewModels.Barber
{
    public class CreateBarberViewModel
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(12)]
        public string PhoneNumber { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string ImageUrl { get; set; }    
    }
}

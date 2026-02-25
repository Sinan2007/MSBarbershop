namespace MSBarbershop.WebApp.ViewModels.Barber
{
    using System.ComponentModel.DataAnnotations;

    public class EditBarberViewModel
    {
        public int Id { get; set; }

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

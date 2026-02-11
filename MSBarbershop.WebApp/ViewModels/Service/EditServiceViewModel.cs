namespace MSBarbershop.WebApp.ViewModels.Service
{
    using System.ComponentModel.DataAnnotations;

    public class EditServiceViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Range(1, 600)]
        public int DurationMinutes { get; set; }
    }

}

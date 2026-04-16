using System.ComponentModel.DataAnnotations;

namespace MSBarbershop.Core.ViewModels.Profile
{
    public class ProfileViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
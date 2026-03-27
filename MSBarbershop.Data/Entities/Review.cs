using System.ComponentModel.DataAnnotations;

namespace MSBarbershop.Data.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public User User { get; set; }
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        [Required]
        [MaxLength(300)]
        public string Content { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}
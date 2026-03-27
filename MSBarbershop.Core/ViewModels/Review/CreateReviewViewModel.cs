using System.ComponentModel.DataAnnotations;

public class CreateReviewViewModel
{
    public int ReservationId { get; set; }

    [Required]
    [MaxLength(300)]
    public string Content { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }
}
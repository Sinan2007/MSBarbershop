using MSBarbershop.Data.Entities.Enums;
using System.ComponentModel.DataAnnotations;

public class MyReservationViewModel
{
    public int Id { get; set; }
    [Required]
    public string BarberName { get; set; }
    [Required]
    public string ServiceName { get; set; }

    [Required]
    public string CustomerName { get; set; }
    [Required]
    [Phone]
    public string? ClientPhoneNumber { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateOnly Date { get; set; }
    [Required]
    [DataType(DataType.Time)]
    public TimeOnly Time { get; set; }

    public TimeOnly EndTime { get; set; }


    public ReservationStatus Status { get; set; }
}

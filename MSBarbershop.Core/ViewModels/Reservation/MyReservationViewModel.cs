using MSBarbershop.Data.Entities.Enums;

public class MyReservationViewModel
{
    public int Id { get; set; }

    public string BarberName { get; set; }
    public string ServiceName { get; set; }

    public string CustomerName { get; set; }

    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }

    public TimeOnly EndTime { get; set; }


    public ReservationStatus Status { get; set; }
}

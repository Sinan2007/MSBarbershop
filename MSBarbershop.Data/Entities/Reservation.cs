using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSBarbershop.Data.Entities.Enums;

namespace MSBarbershop.Data.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int BarberId { get; set; }
        public Barber Barber { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Active;
    }

}

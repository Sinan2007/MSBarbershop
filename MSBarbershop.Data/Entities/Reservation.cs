using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBarbershop.Data.Entities
{ 
    public class Reservation
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int BarberId { get; set; }
        public Barber Barber { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public ReservationStatus Status { get; set; }
    }

}

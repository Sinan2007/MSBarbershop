using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBarbershop.Data.Entities
{
    public class Barber
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Reservation> Reservations { get; set; }

        public ICollection<BarberService> BarberServices { get; set; }

        public ICollection<WorkSchedule> WorkSchedules { get; set; }
    }

}

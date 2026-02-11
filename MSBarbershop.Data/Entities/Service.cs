using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBarbershop.Data.Entities
{
    public class Service
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int DurationMinutes { get; set; }

        public ICollection<Reservation> Reservations { get; set; }

        public ICollection<BarberService> BarberServices { get; set; }
    }

}

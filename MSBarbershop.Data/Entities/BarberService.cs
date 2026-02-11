using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBarbershop.Data.Entities
{
    public class BarberService
    {
        public int BarberId { get; set; }
        public Barber Barber { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }

}

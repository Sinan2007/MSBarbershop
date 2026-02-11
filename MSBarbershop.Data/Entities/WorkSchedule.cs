using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBarbershop.Data.Entities
{
    public class WorkSchedule
    {
        public int Id { get; set; }

        public int BarberId { get; set; }
        public Barber Barber { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }

}

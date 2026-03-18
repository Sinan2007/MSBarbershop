using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBarbershop.Core.ViewModels.Reservation
{
    public class BarberDashboardViewModel
    {
        public List<MyReservationViewModel> Upcoming { get; set; }
        public List<MyReservationViewModel> Past { get; set; }
    }

}

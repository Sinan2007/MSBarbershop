using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBarbershop.Data.Entities
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;

    public class User : IdentityUser
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }

}

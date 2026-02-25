using MSBarbershop.Data.Entities;
using MSBarbershop.Data;

namespace MSBarbershop.WebApp.Seed
{
    public static class BarberSeeder
    {
        public static async Task Seed(ApplicationDbContext context)
        {
            if (context.Barbers.Any())
                return;

            var barbers = new List<Barber>
        {
            new Barber
            {
                FullName = "Ivan Ivanov",
            PhoneNumber = "0895274137",
            Description = "21 years old, barber from 3 years, great skills, responsible, can work with clients, friendly,taken",
            ImageUrl = "/images/barbers/ivan_ivanov.jpg",
            IsActive = true
            },
            new Barber
        {
            FullName = "Nikola Ivanov",
            PhoneNumber = "0888765432",
            Description = "18 years old, first year as barber, learning, friendly, single",
            ImageUrl = "/images/barbers/nikola_ivanov.jpg",
            IsActive = true
        }
        };

            context.Barbers.AddRange(barbers);
            await context.SaveChangesAsync();
        }
    }
}

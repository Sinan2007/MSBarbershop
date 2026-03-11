using MSBarbershop.Data.Entities;
using MSBarbershop.Data;

namespace MSBarbershop.WebApp.Seed
{
    public class ServicesSeeder
    {
        public static async Task Seed(ApplicationDbContext context)
        {
            if (context.Services.Any())
                return;

            var services = new List<Service>
        {
            new Service
            {
            Name = "man haircut",
            Description = "for men",
            Price = 20,
            DurationMinutes = 30
            },
            new Service
            {
            Name = "child haircut",
            Description = "for children under 10 years old",
            Price = 10,
            DurationMinutes = 30
            },
            new Service
            {
            Name = "skin fade",
            Description = "for people who want some style on their head",
            Price = 25,
            DurationMinutes = 45
            },
            new Service
            {
            Name = "beard shaping",
            Description = "for people with beard",
            Price = 10,
            DurationMinutes = 15
            },
            new Service
            {
            Name = "skin fade + beard shaping",
            Description = "for people with style and beard",
            Price = 40,
            DurationMinutes = 60
            },
            new Service
            {
            Name = "beard/hair dying",
            Description = "for people who want to dye their hair or beard",
            Price = 50,
            DurationMinutes = 60
            },
        };

            context.Services.AddRange(services);
            await context.SaveChangesAsync();
        }
    }
}


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
            Name = "short hair haircut",
            Description = "for people with short hair",
            Price = 20,
            DurationMinutes = 30
            },
            new Service
            {
            Name = "long hair haircut",
            Description = "for people with long hair",
            Price = 30,
            DurationMinutes = 45
            },
            new Service
            {
            Name = "short hair + beard",
            Description = "for people with short hair and beard",
            Price = 35,
            DurationMinutes = 45
            },
            new Service
            {
            Name = "long hair haircut + beard",
            Description = "for people with long hair and beard",
            Price = 40,
            DurationMinutes = 60
            },
        };

            context.Services.AddRange(services);
            await context.SaveChangesAsync();
        }
    }
}


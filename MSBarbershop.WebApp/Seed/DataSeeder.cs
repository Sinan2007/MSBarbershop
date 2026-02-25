using MSBarbershop.Data.Entities;
using MSBarbershop.Data;
using Microsoft.AspNetCore.Identity;

namespace MSBarbershop.WebApp.Seed
{

    public class DataSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<ApplicationDbContext>();

            await BarberSeeder.Seed(context);
            await ServicesSeeder.Seed(context);
        }
    }
}

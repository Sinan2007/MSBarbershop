using MSBarbershop.Data.Entities;
using MSBarbershop.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MSBarbershop.WebApp.Seed
{

    public class DataSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var context = services.GetRequiredService<ApplicationDbContext>();

           //await context.Database.MigrateAsync();


            await BarberSeeder.Seed(context,userManager);
            await ServicesSeeder.Seed(context);
            await SchedulesSeeder.Seed(context);
        }
    }
}

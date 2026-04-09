using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;

namespace MSBarbershop.WebApp.Seed
{
    public static class BarberSeeder
    {
        public static async Task Seed(ApplicationDbContext context, UserManager<User> userManager)
        {
            if (await context.Barbers.AnyAsync())
                return;

            var ivanUser = await userManager.FindByEmailAsync("ivan_ivanoww@gmail.com");
            var nikolaUser = await userManager.FindByEmailAsync("nikola_ivanow@gmail.com");
            var svetoslavUser = await userManager.FindByEmailAsync("svetlio_21@gmail.com");

            var barbers = new List<Barber>();

            if (ivanUser != null)
            {
                barbers.Add(new Barber
                {
                    FullName = "Ivan Ivanov",
                    PhoneNumber= "0866666666",
                    Description = "Professional barber",
                    ImageUrl = "/images/barbers/ivan_ivanov.jpg",
                    IsActive = true,
                    UserId = ivanUser.Id
                });
            }

            if (nikolaUser != null)
            {
                barbers.Add(new Barber
                {
                    FullName = "Nikola Ivanov",
                    PhoneNumber = "0875555555",
                    Description = "Fade specialist",
                    ImageUrl = "/images/barbers/khabib.jpg",
                    IsActive = true,
                    UserId = nikolaUser.Id
                });
            }

            if (svetoslavUser != null)
            {
                barbers.Add(new Barber
                {
                    FullName = "Svetoslav Vutsov",
                    PhoneNumber = "0894444444",
                    Description = "Beard expert",
                    ImageUrl = "/images/barbers/khamzat.jpg",
                    IsActive = true,
                    UserId = svetoslavUser.Id
                });
            }

            await context.Barbers.AddRangeAsync(barbers);
            await context.SaveChangesAsync();
        }
    }
}
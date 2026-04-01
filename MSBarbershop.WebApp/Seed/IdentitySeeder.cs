using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MSBarbershop.Data.Entities;

namespace MSBarbershop.WebApp.Seed
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
        }

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = new[] { "Admin", "Barber", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            await CreateUserIfNotExists(
                userManager,
                "sinan@gmail.com",
                "#Sinan2007bg",
                "Sinan",
                "Admin",
                "Admin");

            await CreateUserIfNotExists(
                userManager,
                "stanisav_dimoww@gmail.com",
                "#Stanislav25",
                "Stanislav",
                "Dimov",
                "Customer");

            await CreateUserIfNotExists(
                userManager,
                "georgi_yordanow@gmail.com",
                "#Joro61",
                "Georgi",
                "Yordanov",
                "Customer");

            await CreateUserIfNotExists(
                userManager,
                "ivan_ivanoww@gmail.com",
                "#Ivan67",
                "Ivan",
                "Ivanov",
                "Barber");

            await CreateUserIfNotExists(
                userManager,
                "nikola_ivanow@gmail.com",
                "#Niko21",
                "Nikola",
                "Ivanov",
                "Barber");

            await CreateUserIfNotExists(
                userManager,
                "svetlio_21@gmail.com",
                "#Vutsov92",
                "Svetoslav",
                "Vutsov",
                "Barber");
        }

        private static async Task CreateUserIfNotExists(
            UserManager<User> userManager,
            string email,
            string password,
            string firstName,
            string lastName,
            string role)
        {
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                if (!await userManager.IsInRoleAsync(existingUser, role))
                {
                    await userManager.AddToRoleAsync(existingUser, role);
                }

                return;
            }

            var user = new User
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
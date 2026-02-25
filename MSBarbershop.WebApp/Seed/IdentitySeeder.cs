using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;

namespace MSBarbershop.WebApp.Seed
{
    public class IdentitySeeder
    {

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
        }

    }


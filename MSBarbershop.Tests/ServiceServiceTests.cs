using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.WebApp.ViewModels.Service;


namespace MSBarbershop.Tests
{
    public class ServiceServiceTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAll_ReturnsAllServices()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Services.Add(new Service { Id = 1, Name = "S1", Description = "D1" });
                context.Services.Add(new Service { Id = 2, Name = "S2", Description = "D2" });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ServiceService(context);
                var result = await service.GetAll();

                Assert.Equal(2, result.Count);
            }
        }

        [Fact]
        public async Task GetById_ReturnsService()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Services.Add(new Service { Id = 1, Name = "S1", Description = "D1" });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ServiceService(context);
                var result = await service.GetById(1);

                Assert.NotNull(result);
                Assert.Equal("S1", result.Name);
            }
        }

        [Fact]
        public async Task Create_AddsService()
        {
            var dbName = Guid.NewGuid().ToString();
            var model = new CreateServiceViewModel
            {
                Name = "S1",
                Description = "D1",
                Price = 10,
                DurationMinutes = 30
            };

            using (var context = GetDbContext(dbName))
            {
                var service = new ServiceService(context);
                await service.Create(model);
            }

            using (var context = GetDbContext(dbName))
            {
                Assert.Equal(1, await context.Services.CountAsync());
                var s = await context.Services.FirstAsync();
                Assert.Equal("S1", s.Name);
                Assert.Equal(10, s.Price);
            }
        }

        [Fact]
        public async Task Update_UpdatesService()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Services.Add(new Service { Id = 1, Name = "S1", Description = "D1" });
                await context.SaveChangesAsync();
            }

            var model = new EditServiceViewModel
            {
                Id = 1,
                Name = "S1-Updated",
                Description = "D1-Updated",
                Price = 20,
                DurationMinutes = 45
            };

            using (var context = GetDbContext(dbName))
            {
                var service = new ServiceService(context);
                await service.Update(model);
            }

            using (var context = GetDbContext(dbName))
            {
                var s = await context.Services.FirstAsync();
                Assert.Equal("S1-Updated", s.Name);
                Assert.Equal(20, s.Price);
                Assert.Equal(45, s.DurationMinutes);
            }
        }

        [Fact]
        public async Task Delete_RemovesService()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Services.Add(new Service { Id = 1, Name = "S1", Description = "D1" });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ServiceService(context);
                await service.Delete(1);
            }

            using (var context = GetDbContext(dbName))
            {
                Assert.Empty(context.Services);
            }
        }

        [Fact]
        public async Task Exists_ReturnsTrueIfExists()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Services.Add(new Service { Id = 1, Name = "S1", Description = "D1" });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ServiceService(context);
                var exists = await service.Exists(1);
                var notExists = await service.Exists(99);

                Assert.True(exists);
                Assert.False(notExists);
            }
        }

        [Fact]
        public async Task Update_WhenNotFound_DoesNothing()
        {
            var dbName = Guid.NewGuid().ToString();
            var model = new EditServiceViewModel { Id = 99 };
            using (var context = GetDbContext(dbName))
            {
                var service = new ServiceService(context);
                await service.Update(model);
            }
        }

        [Fact]
        public async Task Delete_WhenNotFound_DoesNothing()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                var service = new ServiceService(context);
                await service.Delete(99);
            }
        }
    }
}

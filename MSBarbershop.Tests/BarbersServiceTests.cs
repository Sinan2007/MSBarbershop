using Microsoft.EntityFrameworkCore;
using MSBarbershop.Core.Services;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.WebApp.ViewModels.Barber;

namespace MSBarbershop.Tests
{
    public class BarbersServiceTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllBarbers_ReturnsAllBarbers()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Barbers.Add(new Barber { Id = 1, FullName = "Barber 1", Description = "Test", PhoneNumber = "111", UserId = "user1" });
                context.Barbers.Add(new Barber { Id = 2, FullName = "Barber 2", Description = "Test", PhoneNumber = "222", UserId = "user2" });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new BarbersService(context);

                // Act
                var result = await service.GetAllBarbers();

                // Assert
                Assert.Equal(2, result.Count);
                Assert.Contains(result, b => b.FullName == "Barber 1");
                Assert.Contains(result, b => b.FullName == "Barber 2");
            }
        }

        [Fact]
        public async Task GetBarberById_ReturnsCorrectBarber()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Barbers.Add(new Barber { Id = 1, FullName = "Barber 1", Description = "Test", PhoneNumber = "111", UserId = "user1" });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new BarbersService(context);

                // Act
                var result = await service.GetBarberById(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Barber 1", result.FullName);
            }
        }

        [Fact]
        public async Task GetBarberById_WhenNotFound_ReturnsNull()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                var service = new BarbersService(context);

                // Act
                var result = await service.GetBarberById(99);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetCompletedReservationsCount_ReturnsCorrectCount()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Reservations.Add(new Reservation { Id = 1, BarberId = 1, Status = MSBarbershop.Data.Entities.Enums.ReservationStatus.Completed });
                context.Reservations.Add(new Reservation { Id = 2, BarberId = 1, Status = MSBarbershop.Data.Entities.Enums.ReservationStatus.Completed });
                context.Reservations.Add(new Reservation { Id = 3, BarberId = 1, Status = MSBarbershop.Data.Entities.Enums.ReservationStatus.Active });
                context.Reservations.Add(new Reservation { Id = 4, BarberId = 2, Status = MSBarbershop.Data.Entities.Enums.ReservationStatus.Completed });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new BarbersService(context);

                // Act
                var count = await service.GetCompletedReservationsCount(1);

                // Assert
                Assert.Equal(2, count);
            }
        }

        [Fact]
        public async Task CreateBarber_SuccessfullyAddsBarber()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var model = new CreateBarberViewModel
            {
                FullName = "New Barber",
                PhoneNumber = "123456789",
                Description = "New Description",
                IsActive = true
            };

            using (var context = GetDbContext(dbName))
            {
                var service = new BarbersService(context);

                // Act
                await service.CreateBarber(model);
            }

            // Assert
            using (var context = GetDbContext(dbName))
            {
                Assert.Equal(1, await context.Barbers.CountAsync());
                var dbBarber = await context.Barbers.FirstAsync();
                Assert.Equal(model.FullName, dbBarber.FullName);
                Assert.Equal(model.PhoneNumber, dbBarber.PhoneNumber);
                Assert.Equal(model.Description, dbBarber.Description);
                Assert.Equal(model.IsActive, dbBarber.IsActive);
            }
        }

        [Fact]
        public async Task UpdateBarber_SuccessfullyUpdatesBarber()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Barbers.Add(new Barber { Id = 1, FullName = "Old", PhoneNumber = "111", Description = "Old", IsActive = false });
                await context.SaveChangesAsync();
            }

            var updateModel = new EditBarberViewModel
            {
                Id = 1,
                FullName = "New",
                PhoneNumber = "222",
                Description = "New",
                IsActive = true
            };

            using (var context = GetDbContext(dbName))
            {
                var service = new BarbersService(context);

                // Act
                await service.UpdateBarber(updateModel);
            }

            // Assert
            using (var context = GetDbContext(dbName))
            {
                var updatedBarber = await context.Barbers.FirstAsync();
                Assert.Equal("New", updatedBarber.FullName);
                Assert.Equal("222", updatedBarber.PhoneNumber);
                Assert.Equal("New", updatedBarber.Description);
                Assert.True(updatedBarber.IsActive);
            }
        }

        [Fact]
        public async Task DeleteBarber_SuccessfullyRemovesBarber()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Barbers.Add(new Barber { Id = 1, FullName = "To Delete" });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new BarbersService(context);

                // Act
                await service.DeleteBarber(1);
            }

            // Assert
            using (var context = GetDbContext(dbName))
            {
                Assert.Equal(0, await context.Barbers.CountAsync());
            }
        }

        [Fact]
        public async Task UpdateBarber_WhenNotFound_DoesNothing()
        {
            var dbName = Guid.NewGuid().ToString();
            var updateModel = new EditBarberViewModel { Id = 99 };
            using (var context = GetDbContext(dbName))
            {
                var service = new BarbersService(context);
                await service.UpdateBarber(updateModel); // Should not throw
            }
        }

        [Fact]
        public async Task DeleteBarber_WhenNotFound_DoesNothing()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                var service = new BarbersService(context);
                await service.DeleteBarber(99); // Should not throw
            }
        }
    }
}

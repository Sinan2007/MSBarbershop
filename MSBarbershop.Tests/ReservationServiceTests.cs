using Microsoft.EntityFrameworkCore;
using MSBarbershop.Core.Services;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.Data.Entities.Enums;
using MSBarbershop.WebApp.ViewModels.Reservation;
using MSBarbershop.Core.ViewModels.Reservation;

namespace MSBarbershop.Tests
{
    public class ReservationServiceTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetUserReservations_ReturnsCorrectData()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Barbers.Add(new Barber { Id = 1, FullName = "B1" });
                context.Services.Add(new Service { Id = 1, Name = "S1", DurationMinutes = 30 });
                context.Reservations.Add(new Reservation
                {
                    Id = 1,
                    UserId = "user1",
                    BarberId = 1,
                    ServiceId = 1,
                    Date = new DateOnly(2025, 1, 1),
                    Time = new TimeOnly(10, 0),
                    Status = ReservationStatus.Active
                });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                var result = await service.GetUserReservations("user1");

                Assert.Single(result);
                Assert.Equal("B1", result[0].BarberName);
                Assert.Equal("S1", result[0].ServiceName);
            }
        }

        [Fact]
        public async Task CreateReservation_CreatesSuccessfully()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Services.Add(new Service { Id = 1, Name = "S1", DurationMinutes = 30 });
                await context.SaveChangesAsync();
            }

            var model = new CreateReservationViewModel
            {
                BarberId = 1,
                ServiceId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                Time = new TimeOnly(10, 0)
            };

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                await service.CreateReservation("user1", model);
            }

            using (var context = GetDbContext(dbName))
            {
                var count = await context.Reservations.CountAsync();
                Assert.Equal(1, count);
                var res = await context.Reservations.FirstAsync();
                Assert.Equal("user1", res.UserId);
            }
        }

        [Fact]
        public async Task CreateReservation_WhenPastDate_ThrowsException()
        {
            var dbName = Guid.NewGuid().ToString();
            var model = new CreateReservationViewModel
            {
                BarberId = 1,
                ServiceId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
                Time = new TimeOnly(10, 0)
            };

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                await Assert.ThrowsAsync<Exception>(() => service.CreateReservation("user1", model));
            }
        }

        [Fact]
        public async Task CancelReservation_ChangesStatusToCancelled()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Reservations.Add(new Reservation { Id = 1, Status = ReservationStatus.Active });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                await service.CancelReservation(1);
            }

            using (var context = GetDbContext(dbName))
            {
                var res = await context.Reservations.FirstAsync();
                Assert.Equal(ReservationStatus.Cancelled, res.Status);
            }
        }

        [Fact]
        public async Task CompleteReservation_ChangesStatusToCompleted()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Reservations.Add(new Reservation { Id = 1, Status = ReservationStatus.Active });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                await service.CompleteReservation(1);
            }

            using (var context = GetDbContext(dbName))
            {
                var res = await context.Reservations.FirstAsync();
                Assert.Equal(ReservationStatus.Completed, res.Status);
            }
        }

        [Fact]
        public async Task GetBarberReservations_ReturnsCorrectData()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Users.Add(new User { Id = "1", FirstName="A", LastName="B" });
                context.Services.Add(new Service { Id=1, Name="S" });
                context.Reservations.Add(new Reservation { Id = 1, BarberId = 1, UserId = "1", ServiceId = 1 });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                var result = await service.GetBarberReservations(1);
                Assert.Single(result);
            }
        }

        [Fact]
        public async Task GetAvailableSlots_WhenEmptySchedule_ReturnsEmptyList()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Services.Add(new Service { Id = 1, Name="S", DurationMinutes = 30 });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                // Assume no schedule exists
                var result = await service.GetAvailableSlots(1, 1, DateOnly.FromDateTime(DateTime.Today));
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task GetMyReservations_ReturnsOrderedData()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Barbers.Add(new Barber { Id = 1, FullName = "B1" });
                context.Services.Add(new Service { Id = 1, Name = "S1" });
                context.Reservations.Add(new Reservation 
                { 
                    Id = 1, UserId = "user1", BarberId = 1, ServiceId = 1,
                    Status = ReservationStatus.Active, Date = DateOnly.MaxValue, Time = new TimeOnly(12, 0)
                });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                var result = await service.GetMyReservations("user1");
                Assert.Single(result);
            }
        }

        [Fact]
        public async Task GetUpcomingReservations_ReturnsOnlyActiveFuture()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Barbers.Add(new Barber { Id = 1, FullName = "B" });
                context.Services.Add(new Service { Id = 1, Name = "S" });
                context.Reservations.Add(new Reservation { Id = 1, UserId = "user1", Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), Status = ReservationStatus.Active, BarberId = 1, ServiceId = 1 });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                var result = await service.GetUpcomingReservations("user1");
                Assert.Single(result);
            }
        }

        [Fact]
        public async Task GetPastReservations_ReturnsCompletedOrPast()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Barbers.Add(new Barber { Id = 1, FullName = "B" });
                context.Services.Add(new Service { Id = 1, Name = "S" });
                context.Reservations.Add(new Reservation { Id = 1, UserId = "user1", Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), Status = ReservationStatus.Completed, BarberId = 1, ServiceId = 1 });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                var result = await service.GetPastReservations("user1");
                Assert.Single(result);
            }
        }

        [Fact]
        public async Task GetBarberUpcomingReservations_ReturnsCorrectData()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Users.Add(new User { Id = "user1", FirstName = "F", LastName = "L", PhoneNumber="123" });
                context.Services.Add(new Service { Id = 1, Name = "S" });
                context.Barbers.Add(new Barber { Id = 1 });
                context.Reservations.Add(new Reservation { Id = 1, BarberId = 1, UserId = "user1", ServiceId = 1, Status = ReservationStatus.Active, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)) });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                var result = await service.GetBarberUpcomingReservations(1);
                Assert.Single(result);
                Assert.Equal("F L", result[0].BarberName); // Borrowed field name in viewmodel
            }
        }

        [Fact]
        public async Task GetBarberPastReservations_ReturnsCorrectData()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Users.Add(new User { Id = "user1", FirstName = "F", LastName = "L" });
                context.Services.Add(new Service { Id = 1, Name = "S" });
                context.Barbers.Add(new Barber { Id = 1 });
                context.Reservations.Add(new Reservation { Id = 1, BarberId = 1, UserId = "user1", ServiceId = 1, Status = ReservationStatus.Completed });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                var result = await service.GetBarberPastReservations(1);
                Assert.Single(result);
            }
        }

        [Fact]
        public async Task GetReservationForEditAsync_ReturnsModel()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Reservations.Add(new Reservation { Id = 1, UserId = "user1" });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                var result = await service.GetReservationForEditAsync(1, "user1");
                Assert.NotNull(result);
            }
        }

        [Fact]
        public async Task GetReservationForEditAsync_NotFound_ReturnsNull()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                var result = await service.GetReservationForEditAsync(99, "user1");
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task UpdateReservationAsync_NotFound_ReturnsFalse()
        {
            var dbName = Guid.NewGuid().ToString();
            var model = new EditReservationViewModel { Id = 99 };
            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                var result = await service.UpdateReservationAsync(model, "user1");
                Assert.False(result);
            }
        }

        [Fact]
        public async Task UpdateReservationAsync_NotActive_ReturnsFalse()
        {
            var dbName = Guid.NewGuid().ToString();
            var model = new EditReservationViewModel { Id = 1 };
            using (var context = GetDbContext(dbName))
            {
                context.Reservations.Add(new Reservation { Id = 1, UserId = "user1", Status = ReservationStatus.Cancelled });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                var result = await service.UpdateReservationAsync(model, "user1");
                Assert.False(result);
            }
        }

        [Fact]
        public async Task UpdateReservationAsync_Updates()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Services.Add(new Service { Id = 2, Name="S", DurationMinutes = 30 });
                context.Reservations.Add(new Reservation { Id = 1, UserId = "user1", Status = ReservationStatus.Active, BarberId = 1, ServiceId = 1 });
                await context.SaveChangesAsync();
            }

            var model = new EditReservationViewModel
            {
                Id = 1,
                BarberId = 2,
                ServiceId = 2,
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                Time = new TimeOnly(12, 0)
            };

            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                var result = await service.UpdateReservationAsync(model, "user1");
                Assert.True(result);
            }
        }

        [Fact]
        public async Task CancelReservation_WhenNotFound_DoesNothing()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                await service.CancelReservation(99); // should not throw
            }
        }

        [Fact]
        public async Task CompleteReservation_WhenNotFound_DoesNothing()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                await service.CompleteReservation(99); // should not throw
            }
        }

        [Fact]
        public async Task LoadEditReservationDropdownsAsync_FillsDropdowns()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Barbers.Add(new Barber { Id = 1, FullName = "B1" });
                context.Services.Add(new Service { Id = 1, Name = "S1" });
                await context.SaveChangesAsync();
            }

            var model = new EditReservationViewModel();
            using (var context = GetDbContext(dbName))
            {
                var service = new ReservationService(context);
                await service.LoadEditReservationDropdownsAsync(model);
                
                Assert.Single(model.Barbers);
                Assert.Single(model.Services);
            }
        }
    }
}

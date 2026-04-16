using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.WebApp.ViewModels.WorkSchedule;


namespace MSBarbershop.Tests
{
    public class WorkScheduleServiceTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAll_ReturnsAllWorkSchedules()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Barbers.Add(new Barber { Id = 1, FullName = "B1" });
                context.WorkSchedules.Add(new WorkSchedule { Id = 1, BarberId = 1, DayOfWeek = DayOfWeek.Monday });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new WorkScheduleService(context);
                var result = await service.GetAll();

                Assert.Single(result);
                Assert.Equal(DayOfWeek.Monday, result[0].DayOfWeek);
                Assert.Equal("B1", result[0].Barber.FullName);
            }
        }

        [Fact]
public async Task GetById_ReturnsWorkSchedule()
{
    var dbName = Guid.NewGuid().ToString();
    using (var context = GetDbContext(dbName))
    {
        // ADD A BARBER FIRST
        context.Barbers.Add(new Barber { Id = 1 });
        
        // ASSIGN BarberId = 1 TO THE WORK SCHEDULE
        context.WorkSchedules.Add(new WorkSchedule { Id = 1, BarberId = 1, DayOfWeek = DayOfWeek.Tuesday });
        
        await context.SaveChangesAsync();
    }

    using (var context = GetDbContext(dbName))
    {
        var service = new WorkScheduleService(context);
        var result = await service.GetById(1);

        Assert.NotNull(result);
        Assert.Equal(DayOfWeek.Tuesday, result.DayOfWeek);
    }
}


        [Fact]
        public async Task Create_AddsWorkSchedule()
        {
            var dbName = Guid.NewGuid().ToString();
            var model = new CreateWorkScheduleViewModel
            {
                BarberId = 1,
                DayOfWeek = DayOfWeek.Wednesday,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0)
            };

            using (var context = GetDbContext(dbName))
            {
                var service = new WorkScheduleService(context);
                await service.Create(model);
            }

            using (var context = GetDbContext(dbName))
            {
                Assert.Equal(1, await context.WorkSchedules.CountAsync());
                var ws = await context.WorkSchedules.FirstAsync();
                Assert.Equal(DayOfWeek.Wednesday, ws.DayOfWeek);
            }
        }

        [Fact]
        public async Task Update_UpdatesWorkSchedule()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.WorkSchedules.Add(new WorkSchedule { Id = 1, DayOfWeek = DayOfWeek.Monday });
                await context.SaveChangesAsync();
            }

            var schedule = new WorkSchedule
            {
                Id = 1,
                DayOfWeek = DayOfWeek.Friday
            };

            using (var context = GetDbContext(dbName))
            {
                var service = new WorkScheduleService(context);
                await service.Update(schedule);
            }

            using (var context = GetDbContext(dbName))
            {
                var ws = await context.WorkSchedules.FirstAsync();
                Assert.Equal(DayOfWeek.Friday, ws.DayOfWeek);
            }
        }

        [Fact]
        public async Task Delete_RemovesWorkSchedule()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.WorkSchedules.Add(new WorkSchedule { Id = 1 });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new WorkScheduleService(context);
                await service.Delete(1);
            }

            using (var context = GetDbContext(dbName))
            {
                Assert.Empty(context.WorkSchedules);
            }
        }

        [Fact]
        public async Task Exists_ReturnsTrueIfExists()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.WorkSchedules.Add(new WorkSchedule { Id = 1 });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new WorkScheduleService(context);
                var exists = await service.Exists(1);
                var notExists = await service.Exists(99);

                Assert.True(exists);
                Assert.False(notExists);
            }
        }

        [Fact]
        public async Task GetBarbersDropdown_ReturnsSelectListItems()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Barbers.Add(new Barber { Id = 1, FullName = "B1" });
                context.Barbers.Add(new Barber { Id = 2, FullName = "B2" });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new WorkScheduleService(context);
                var result = await service.GetBarbersDropdown();

                Assert.Equal(2, result.Count);
                Assert.Equal("1", result[0].Value);
                Assert.Equal("B1", result[0].Text);
            }
        }

        [Fact]
        public async Task Delete_WhenNotFound_DoesNothing()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                var service = new WorkScheduleService(context);
                await service.Delete(99);
            }
        }
    }
}

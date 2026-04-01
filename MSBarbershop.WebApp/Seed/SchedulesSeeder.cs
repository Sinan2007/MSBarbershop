using MSBarbershop.Data;
using MSBarbershop.Data.Entities;

namespace MSBarbershop.WebApp.Seed
{
    public class SchedulesSeeder
    {
        public static async Task Seed(ApplicationDbContext context)
        {
            if (!context.WorkSchedules.Any())
            {
                var schedules = new List<WorkSchedule>();
                var barbers = context.Barbers.ToList();
                foreach (var barber in barbers)
                {
                    for (int day = 1; day <= 6; day++)
                    {
                        if (day == 6)
                        {
                            schedules.Add(new WorkSchedule
                            {
                                BarberId = barber.Id,
                                DayOfWeek = (DayOfWeek)day,
                                StartTime = new TimeSpan(10, 0, 0),
                                EndTime = new TimeSpan(16, 0, 0)
                            });
                        }
                        else
                        {
                            schedules.Add(new WorkSchedule
                            {
                                BarberId = barber.Id,
                                DayOfWeek = (DayOfWeek)day,
                                StartTime = new TimeSpan(9, 0, 0),
                                EndTime = new TimeSpan(18, 0, 0)
                            });
                        }
                    }
                }

                context.WorkSchedules.AddRange(schedules);
                context.SaveChanges();
            }
        }

    }
}

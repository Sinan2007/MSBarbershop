using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.Data.Entities.Enums;


namespace MSBarbershop.Tests
{
    public class ReviewServiceTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateReview_WithValidReservation_CreatesReview()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Reservations.Add(new Reservation
                {
                    Id = 1,
                    UserId = "user1",
                    Status = ReservationStatus.Completed
                });
                await context.SaveChangesAsync();
            }

            var model = new CreateReviewViewModel
            {
                ReservationId = 1,
                Content = "Great!",
                Rating = 5
            };

            using (var context = GetDbContext(dbName))
            {
                var service = new ReviewService(context);
                await service.CreateReview("user1", model);
            }

            using (var context = GetDbContext(dbName))
            {
                var review = await context.Reviews.FirstAsync();
                Assert.Equal("Great!", review.Content);
                Assert.Equal(5, review.Rating);
                Assert.False(review.IsApproved);
            }
        }

        [Fact]
        public async Task CreateReview_WhenNotAllowed_ThrowsException()
        {
            var dbName = Guid.NewGuid().ToString();
            var model = new CreateReviewViewModel
            {
                ReservationId = 1,
                Content = "Great!",
                Rating = 5
            };

            using (var context = GetDbContext(dbName))
            {
                var service = new ReviewService(context);
                await Assert.ThrowsAsync<Exception>(() => service.CreateReview("user1", model));
            }
        }

        [Fact]
        public async Task GetApprovedReviews_ReturnsOnlyApproved()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Reviews.Add(new Review { Id = 1, Content = "R1", IsApproved = true });
                context.Reviews.Add(new Review { Id = 2, Content = "R2", IsApproved = false });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReviewService(context);
                var result = await service.GetApprovedReviews();

                Assert.Single(result);
                Assert.Equal("R1", result.First().Content);
            }
        }

        [Fact]
        public async Task ApproveReviewAsync_ApprovesReview()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Reviews.Add(new Review { Id = 1, IsApproved = false });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReviewService(context);
                await service.ApproveReviewAsync(1);
            }

            using (var context = GetDbContext(dbName))
            {
                var rev = await context.Reviews.FirstAsync();
                Assert.True(rev.IsApproved);
            }
        }

        [Fact]
        public async Task DeleteReviewAsync_DeletesReview()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Reviews.Add(new Review { Id = 1 });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReviewService(context);
                await service.DeleteReviewAsync(1);
            }

            using (var context = GetDbContext(dbName))
            {
                Assert.Empty(context.Reviews);
            }
        }

        [Fact]
        public async Task ApproveReviewAsync_WhenNotFound_DoesNothing()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                var service = new ReviewService(context);
                await service.ApproveReviewAsync(99);
            }
        }

        [Fact]
        public async Task DeleteReviewAsync_WhenNotFound_DoesNothing()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                var service = new ReviewService(context);
                await service.DeleteReviewAsync(99);
            }
        }

        [Fact]
        public async Task GetAllReviewsAsync_ReturnsData()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Users.Add(new User { Id = "1", FirstName="A", LastName="B" });
                context.Reservations.Add(new Reservation { Id=1 });
                context.Reviews.Add(new Review { Id = 1, UserId="1", ReservationId=1 });
                await context.SaveChangesAsync();
            }

            using (var context = GetDbContext(dbName))
            {
                var service = new ReviewService(context);
                var result = await service.GetAllReviewsAsync();
                Assert.Single(result);
            }
        }
    }
}

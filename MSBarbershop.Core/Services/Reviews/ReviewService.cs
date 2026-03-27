using Microsoft.EntityFrameworkCore;
using MSBarbershop.Data;
using MSBarbershop.Data.Entities;
using MSBarbershop.Data.Entities.Enums;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;

    public ReviewService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateReview(string userId, CreateReviewViewModel model)
    {
        var canReview = await CanUserReviewReservation(model.ReservationId, userId);

        if (!canReview)
            throw new Exception("You cannot review this reservation.");

        var review = new Review
        {
            UserId = userId,
            ReservationId = model.ReservationId,
            Content = model.Content,
            Rating = model.Rating,
            CreatedOn = DateTime.Now,
            IsApproved = false
        };

        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Review>> GetApprovedReviews()
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.CreatedOn)
            .Take(3)
            .ToListAsync();
    }

    public async Task<bool> CanUserReviewReservation(int reservationId, string userId)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r =>
                r.Id == reservationId &&
                r.UserId == userId &&
                r.Status == ReservationStatus.Completed);

        if (reservation == null)
            return false;

        var alreadyReviewed = await _context.Reviews
            .AnyAsync(r => r.ReservationId == reservationId);

        return !alreadyReviewed;
    }

    public async Task<List<Review>> GetAllReviewsAsync()
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Reservation)
            .OrderByDescending(r => r.CreatedOn)
            .ToListAsync();
    }

    public async Task ApproveReviewAsync(int id)
    {
        var review = await _context.Reviews.FindAsync(id);

        if (review == null)
            return;

        review.IsApproved = true;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReviewAsync(int id)
    {
        var review = await _context.Reviews.FindAsync(id);

        if (review == null)
            return;

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
    }
}
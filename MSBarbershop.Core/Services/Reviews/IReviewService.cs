using MSBarbershop.Data.Entities;

public interface IReviewService
{
    Task CreateReview(string userId, CreateReviewViewModel model);
    Task<IEnumerable<Review>> GetApprovedReviews();
    Task<bool> CanUserReviewReservation(int reservationId, string userId);

    Task<List<Review>> GetAllReviewsAsync();
    Task ApproveReviewAsync(int id);
    Task DeleteReviewAsync(int id);
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class ReviewsController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int reservationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var canReview = await _reviewService.CanUserReviewReservation(reservationId, userId);

        if (!canReview)
            return NotFound();

        var model = new CreateReviewViewModel
        {
            ReservationId = reservationId
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReviewViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _reviewService.CreateReview(userId, model);
        TempData["Review"] = "You have succesfully added a review!";


        return RedirectToAction("MyReservations", "Reservations");
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AllReviews()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        return View(reviews);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        await _reviewService.ApproveReviewAsync(id);
        return RedirectToAction(nameof(AllReviews));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _reviewService.DeleteReviewAsync(id);
        return RedirectToAction(nameof(AllReviews));
    }
}
using HandMade_Store_api.DTOs.Review;
using HandMade_Store_api.Models;
using Microsoft.EntityFrameworkCore;

public class ReviewService : IReviewService
{
    private readonly HandmadeStoreContext _context;

    public ReviewService(HandmadeStoreContext context)
    {
        _context = context;
    }

    public async Task<ReviewResponse> CreateAsync(int userId, ReviewRequest request)
    {
        var review = new Review
        {
            ProductId = request.ProductId,
            UserId = userId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.Now
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(userId);

        return new ReviewResponse
        {
            ReviewId = review.ReviewId,
            ProductId = review.ProductId,
            UserId = review.UserId,
            Rating = review.Rating ?? 0,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt ?? DateTime.Now,
            UserName = user?.Name
        };
    }

    public async Task<IEnumerable<ReviewResponse>> GetByProductIdAsync(int productId)
    {
        return await _context.Reviews
            .Where(r => r.ProductId == productId)
            .Include(r => r.User)
            .Select(r => new ReviewResponse
            {
                ReviewId = r.ReviewId,
                ProductId = r.ProductId,
                UserId = r.UserId,
                Rating = r.Rating ?? 0,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt ?? DateTime.Now,
                UserName = r.User.Name
            })
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(int reviewId, int userId)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null || review.UserId != userId) return false;

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }
}

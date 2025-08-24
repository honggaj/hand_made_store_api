using FurnitureStoreAPI.DTOs.Review;
using FurnitureStoreAPI.Interfaces;
using FurnitureStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

public class ReviewService : IReviewService
{
    private readonly FurnitureStoreContext _context;

    public ReviewService(FurnitureStoreContext context)
    {
        _context = context;
    }

    public async Task<ReviewResponse> CreateReviewAsync(ReviewCreateRequest request)
    {
        var review = new Review
        {
            ProductId = request.ProductId,
            UserId = request.UserId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return new ReviewResponse
        {
            ReviewId = review.ReviewId,
            ProductId = review.ProductId,
            UserId = review.UserId,
            Rating = review.Rating ?? 0,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt ?? DateTime.UtcNow
        };
    }

    public async Task<IEnumerable<ReviewResponse>> GetReviewsByProductAsync(int productId)
    {
        return await _context.Reviews
            .Where(r => r.ProductId == productId)
            .Select(r => new ReviewResponse
            {
                ReviewId = r.ReviewId,
                ProductId = r.ProductId,
                ProductName = r.Product.Name,
                UserId = r.UserId,
                UserName = r.User.Name,
                Rating = r.Rating ?? 0,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt ?? DateTime.UtcNow
            })
            .ToListAsync();
    }

    public async Task<bool> DeleteReviewAsync(int reviewId)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null) return false;

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }
}

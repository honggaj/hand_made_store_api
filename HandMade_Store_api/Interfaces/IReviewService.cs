using HandMade_Store_api.DTOs.Review;

public interface IReviewService
{
    Task<ReviewResponse> CreateAsync(int userId, ReviewRequest request);
    Task<IEnumerable<ReviewResponse>> GetByProductIdAsync(int productId);
    Task<bool> DeleteAsync(int reviewId, int userId);
}

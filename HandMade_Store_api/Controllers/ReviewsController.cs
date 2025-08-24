using HandMade_Store_api.DTOs.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HandMade_Store_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // token bắt buộc
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewsController(IReviewService service)
        {
            _service = service;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReviewRequest request)
        {
            // lấy userId từ claim "sub" của JWT token
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userId == 0)
                return Unauthorized("Token invalid");

            var review = await _service.CreateAsync(userId, request);
            return CreatedAtAction(nameof(GetByProduct), new { productId = review.ProductId }, review);
        }


        [HttpGet("product/{productId}")]
        [AllowAnonymous] // ai cũng xem được
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var reviews = await _service.GetByProductIdAsync(productId);
            return Ok(reviews);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _service.DeleteAsync(id, userId);
            if (!result) return NotFound("Cannot delete this review");
            return NoContent();
        }
    }
}

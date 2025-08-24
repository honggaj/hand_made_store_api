namespace HandMade_Store_api.DTOs.Review
{
    public class ReviewRequest
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}

namespace HandMade_Store_api.DTOs.Product
{
    public class ProductUpdateRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsActive { get; set; }

        public bool IsFeatured { get; set; }

        public int CategoryId { get; set; }

        // Màu sắc muốn gắn
        public List<int> ColorIds { get; set; } = new List<int>();

        // Ảnh mới upload
        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();

        // Ảnh cũ muốn xóa
        public List<int> DeleteImageIds { get; set; } = new List<int>(); // 🔥 thêm vào đây
    }
}

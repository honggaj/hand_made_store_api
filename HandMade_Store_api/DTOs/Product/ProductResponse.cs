using HandMade_Store_api.DTOs.Color;

namespace HandMade_Store_api.DTOs.Product
{
    public class ProductResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }

        public List<string> Images { get; set; } = new();

        // Category
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";

        // Colors - dùng DTO có id + name
        public List<ColorResponse> Colors { get; set; } = new();
    }
}

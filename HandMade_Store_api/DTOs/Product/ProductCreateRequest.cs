namespace HandMade_Store_api.DTOs.Product
{
    // Thêm / Update sản phẩm
    public class ProductCreateRequest
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }

        // Màu sắc (list id màu có sẵn)
        public List<int> ColorIds { get; set; } = new();

        // Ảnh mới (upload file)
        public List<IFormFile> NewImages { get; set; } = new();

        // Id ảnh muốn xoá
        public List<int> DeleteImageIds { get; set; } = new();
    }
}

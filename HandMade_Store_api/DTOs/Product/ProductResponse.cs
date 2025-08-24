namespace HandMade_Store_api.DTOs.Product
{
    // Response
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
        public List<string> Colors { get; set; } = new();
    }
}

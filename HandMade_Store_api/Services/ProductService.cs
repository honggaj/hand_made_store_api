using HandMade_Store_api.DTOs.Product;
using HandMade_Store_api.Models;
using HandMade_Store_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HandMade_Store_api.Services
{
    public class ProductService : IProductService
    {
        private readonly HandmadeStoreContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductService(HandmadeStoreContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<ProductResponse> CreateAsync(ProductCreateRequest request)
        {
            var product = new Product
            {
                CategoryId = request.CategoryId,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                IsActive = request.IsActive,
                IsFeatured = request.IsFeatured,
                CreatedAt = DateTime.Now
            };

            // Màu sắc
            if (request.ColorIds != null && request.ColorIds.Any())
            {
                var colors = await _context.Colors
                    .Where(c => request.ColorIds.Contains(c.ColorId))
                    .ToListAsync();
                product.Colors = colors;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Upload ảnh
            if (request.NewImages != null)
            {
                foreach (var file in request.NewImages)
                {
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(_env.WebRootPath, "upload", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var img = new ProductImage
                    {
                        ProductId = product.ProductId,
                        Url = $"/upload/{fileName}",
                        CreatedAt = DateTime.Now
                    };

                    _context.ProductImages.Add(img);
                }
                await _context.SaveChangesAsync();
            }

            return await GetByIdAsync(product.ProductId);
        }

        public async Task<ProductResponse?> UpdateAsync(int id, ProductUpdateRequest request)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Colors)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return null;

            product.CategoryId = request.CategoryId;
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.IsActive = request.IsActive;
            product.IsFeatured = request.IsFeatured;

            // Xoá ảnh
            if (request.DeleteImageIds != null && request.DeleteImageIds.Any())
            {
                var imgs = product.ProductImages.Where(i => request.DeleteImageIds.Contains(i.ImageId)).ToList();
                foreach (var img in imgs)
                {
                    var filePath = Path.Combine(_env.WebRootPath, img.Url.TrimStart('/'));
                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    _context.ProductImages.Remove(img);
                }
            }

            // Upload ảnh mới
            if (request.NewImages != null)
            {
                foreach (var file in request.NewImages)
                {
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(_env.WebRootPath, "upload", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var img = new ProductImage
                    {
                        ProductId = product.ProductId,
                        Url = $"/upload/{fileName}",
                        CreatedAt = DateTime.Now
                    };

                    _context.ProductImages.Add(img);
                }
            }

            // Update màu
            product.Colors.Clear();
            if (request.ColorIds != null && request.ColorIds.Any())
            {
                var colors = await _context.Colors
                    .Where(c => request.ColorIds.Contains(c.ColorId))
                    .ToListAsync();
                product.Colors = colors;
            }

            await _context.SaveChangesAsync();

            return await GetByIdAsync(product.ProductId);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return false;

            // Xoá ảnh trong wwwroot
            foreach (var img in product.ProductImages)
            {
                var filePath = Path.Combine(_env.WebRootPath, img.Url.TrimStart('/'));
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ProductResponse?> GetByIdAsync(int id)
        {
            var p = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Colors)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (p == null) return null;

            return new ProductResponse
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity ?? 0,
                IsActive = p.IsActive ?? false,
                IsFeatured = p.IsFeatured ?? false,
                Images = p.ProductImages.Select(i => i.Url).ToList(),
                Colors = p.Colors.Select(c => c.Name).ToList()
            };
        }

        public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Colors)
                .Select(p => new ProductResponse
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity ?? 0,
                    IsActive = p.IsActive ?? false,
                    IsFeatured = p.IsFeatured ?? false,
                    Images = p.ProductImages.Select(i => i.Url).ToList(),
                    Colors = p.Colors.Select(c => c.Name).ToList()
                })
                .ToListAsync();
        }
    }
}

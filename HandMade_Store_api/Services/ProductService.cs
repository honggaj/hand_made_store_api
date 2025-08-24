using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FurnitureStoreAPI.DTOs.Product;
using FurnitureStoreAPI.DTOs.Review;
using FurnitureStoreAPI.Interfaces;
using FurnitureStoreAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStoreAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly FurnitureStoreContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductService(FurnitureStoreContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _context.Products
     .Include(p => p.ProductImages)
     .Include(p => p.Category)
     .Include(p => p.Reviews)  // để tính trung bình
     .ToListAsync();


            return products.Select(p => MapToResponse(p));
        }


        public async Task<ProductResponse> CreateProductAsync(ProductCreateRequest request)
        {
            var product = new Product
            {
                CategoryId = request.CategoryId,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Material = request.Material,
                Color = request.Color,
                StockQuantity = request.StockQuantity,
                WarrantyMonths = request.WarrantyMonths,
                OriginCountry = request.OriginCountry,
                CreatedAt = DateTime.UtcNow,
                IsFeatured = true,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Upload images
            if (request.Images != null && request.Images.Any())
            {
                var uploadPath = Path.Combine(_env.WebRootPath, "upload");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                foreach (var file in request.Images)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    product.ProductImages.Add(new ProductImage
                    {
                        Url = $"/upload/{fileName}",
                        CreatedAt = DateTime.UtcNow
                    });
                }
                await _context.SaveChangesAsync();
            }

            return MapToResponse(product);
        }

        public async Task<ProductResponse> UpdateProductAsync(int id, ProductUpdateRequest request)
        {
            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return null;

            product.CategoryId = request.CategoryId;
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Material = request.Material;
            product.Color = request.Color;
            product.StockQuantity = request.StockQuantity;
            product.WarrantyMonths = request.WarrantyMonths;
            product.OriginCountry = request.OriginCountry;

            // Remove old images
            if (request.RemoveImageIds != null)
            {
                foreach (var imgId in request.RemoveImageIds)
                {
                    var img = product.ProductImages.FirstOrDefault(i => i.ImageId == imgId);
                    if (img != null)
                    {
                        var path = Path.Combine(_env.WebRootPath, img.Url.TrimStart('/'));
                        if (File.Exists(path)) File.Delete(path);
                        _context.ProductImages.Remove(img);
                    }
                }
            }

            // Add new images
            if (request.AddImages != null && request.AddImages.Any())
            {
                var uploadPath = Path.Combine(_env.WebRootPath, "upload");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                foreach (var file in request.AddImages)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    product.ProductImages.Add(new ProductImage
                    {
                        Url = $"/upload/{fileName}",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return MapToResponse(product);
        }

        public async Task<ProductResponse> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
      .Include(p => p.ProductImages)
      .Include(p => p.Category)
      .Include(p => p.Reviews)
          .ThenInclude(r => r.User)
      .FirstOrDefaultAsync(p => p.ProductId == id);


            if (product == null) return null;

            return MapToResponse(product);
        }


        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return false;

            // Xóa file ảnh trên ổ cứng
            foreach (var img in product.ProductImages)
            {
                var path = Path.Combine(_env.WebRootPath, img.Url.TrimStart('/'));
                if (File.Exists(path)) File.Delete(path);
            }

            // Xóa ảnh trong DB trước
            _context.ProductImages.RemoveRange(product.ProductImages);

            // Xóa product
            _context.Products.Remove(product);

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<ProductResponse>> SearchProductsAsync(string name)
        {
            var products = await _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.Name.Contains(name))
                .ToListAsync();

            return products.Select(p => MapToResponse(p));
        }

        public async Task<bool> ChangeFeaturedStatusAsync(int id, bool status)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;
            product.IsFeatured = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeActiveStatusAsync(int id, bool status)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;
            product.IsActive = status;
            await _context.SaveChangesAsync();
            return true;
        }
        private ProductResponse MapToResponse(Product p) => new ProductResponse
        {
            ProductId = p.ProductId,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Material = p.Material,
            Color = p.Color,
            StockQuantity = p.StockQuantity,
            WarrantyMonths = p.WarrantyMonths,
            OriginCountry = p.OriginCountry,
            IsActive = p.IsActive,
            IsFeatured = p.IsFeatured,
            ProductImages = p.ProductImages.Select(i => new ProductImageResponse
            {
                ImageId = i.ImageId,
                Url = i.Url
            }).ToList(),
            AverageRating = p.Reviews != null && p.Reviews.Any() ? p.Reviews.Average(r => r.Rating ?? 0) : 0,
            Reviews = p.Reviews?.Select(r => new ReviewResponse
            {
                ReviewId = r.ReviewId,
                ProductId = r.ProductId,
                ProductName = p.Name,
                UserId = r.UserId,
                UserName = r.User?.Name,
                Rating = r.Rating ?? 0,
                Comment = r.Comment,
            }).ToList() ?? new List<ReviewResponse>()
        };


    }
}

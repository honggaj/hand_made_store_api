using FurnitureStoreAPI.DTOs.Category;
using FurnitureStoreAPI.Interfaces;
using FurnitureStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStoreAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly FurnitureStoreContext _context;

        public CategoryService(FurnitureStoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryResponse
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    ParentId = c.ParentId
                }).ToListAsync();
        }

        public async Task<CategoryResponse> GetCategoryByIdAsync(int id)
        {
            var c = await _context.Categories.FindAsync(id);
            if (c == null) return null;
            return new CategoryResponse
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                ParentId = c.ParentId
            };
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CategoryRequest request)
        {
            var c = new Category
            {
                Name = request.Name,
                Description = request.Description,
                ParentId = request.ParentId
            };
            _context.Categories.Add(c);
            await _context.SaveChangesAsync();
            return await GetCategoryByIdAsync(c.CategoryId);
        }

        public async Task<CategoryResponse> UpdateCategoryAsync(int id, CategoryRequest request)
        {
            var c = await _context.Categories.FindAsync(id);
            if (c == null) return null;

            c.Name = request.Name;
            c.Description = request.Description;
            c.ParentId = request.ParentId;
            await _context.SaveChangesAsync();

            return await GetCategoryByIdAsync(c.CategoryId);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var c = await _context.Categories.FindAsync(id);
            if (c == null) return false;
            _context.Categories.Remove(c);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}

using HandMade_Store_api.DTOs.Category;
using HandMade_Store_api.DTOs.NewFolder;
using HandMade_Store_api.Interfaces;
using HandMade_Store_api.Models;
using Microsoft.EntityFrameworkCore;

namespace HandMade_Store_api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly HandmadeStoreContext _context;

        public CategoryService(HandmadeStoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryResponse
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();
        }

        public async Task<CategoryResponse> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            return new CategoryResponse
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<CategoryResponse> CreateAsync(CategoryRequest request)
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryResponse
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<CategoryResponse> UpdateAsync(int id, CategoryUpdateRequest request)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            category.Name = request.Name;
            category.Description = request.Description;

            await _context.SaveChangesAsync();

            return new CategoryResponse
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

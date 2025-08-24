using HandMade_Store_api.DTOs.Category;
using HandMade_Store_api.DTOs.NewFolder;

namespace HandMade_Store_api.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllAsync();
        Task<CategoryResponse> GetByIdAsync(int id);
        Task<CategoryResponse> CreateAsync(CategoryRequest request);
        Task<CategoryResponse> UpdateAsync(int id, CategoryUpdateRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
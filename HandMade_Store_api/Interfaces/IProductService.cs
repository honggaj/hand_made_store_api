using HandMade_Store_api.DTOs.Product;

namespace HandMade_Store_api.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponse> CreateAsync(ProductCreateRequest request);
        Task<ProductResponse?> UpdateAsync(int id, ProductUpdateRequest request);
        Task<bool> DeleteAsync(int id);
        Task<ProductResponse?> GetByIdAsync(int id);
        Task<IEnumerable<ProductResponse>> GetAllAsync();
        Task<IEnumerable<ProductResponse>> SearchByNameAsync(string name);
        Task<ProductResponse?> ToggleFeaturedAsync(int id);
        Task<ProductResponse?> ToggleActiveAsync(int id);


    }
}

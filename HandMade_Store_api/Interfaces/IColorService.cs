using HandMade_Store_api.DTOs.Color;

namespace HandMade_Store_api.Interfaces
{
    public interface IColorService
    {
        Task<IEnumerable<ColorResponse>> GetAllAsync();
        Task<ColorResponse> GetByIdAsync(int id);
        Task<ColorResponse> CreateAsync(ColorRequest request);
        Task<ColorResponse> UpdateAsync(int id, ColorUpdateRequest request);
        Task<bool> DeleteAsync(int id);
    }
}

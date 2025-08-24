using HandMade_Store_api.DTOs.Color;
using HandMade_Store_api.Interfaces;
using HandMade_Store_api.Models;
using Microsoft.EntityFrameworkCore;

namespace HandMade_Store_api.Services
{
    public class ColorService : IColorService
    {
        private readonly HandmadeStoreContext _context;

        public ColorService(HandmadeStoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ColorResponse>> GetAllAsync()
        {
            return await _context.Colors
                .Select(c => new ColorResponse
                {
                    ColorId = c.ColorId,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<ColorResponse> GetByIdAsync(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return null;

            return new ColorResponse
            {
                ColorId = color.ColorId,
                Name = color.Name
            };
        }

        public async Task<ColorResponse> CreateAsync(ColorRequest request)
        {
            var color = new Color
            {
                Name = request.Name
            };

            _context.Colors.Add(color);
            await _context.SaveChangesAsync();

            return new ColorResponse
            {
                ColorId = color.ColorId,
                Name = color.Name
            };
        }

        public async Task<ColorResponse> UpdateAsync(int id, ColorUpdateRequest request)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return null;

            color.Name = request.Name;
            await _context.SaveChangesAsync();

            return new ColorResponse
            {
                ColorId = color.ColorId,
                Name = color.Name
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return false;

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

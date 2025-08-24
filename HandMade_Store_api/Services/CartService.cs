using FurnitureStoreAPI.DTOs.Cart;
using FurnitureStoreAPI.Interfaces;
using FurnitureStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

public class CartService : ICartService
{
    private readonly FurnitureStoreContext _context;

    public CartService(FurnitureStoreContext context)
    {
        _context = context;
    }

    public async Task<CartResponse> GetCartByUserIdAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.ProductImages)
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Variant)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
            return new CartResponse { UserId = userId, Items = new List<CartItemResponse>() };

        return new CartResponse
        {
            CartId = cart.CartId,
            UserId = cart.UserId,
            Items = cart.CartItems.Select(ci => new CartItemResponse
            {
                CartItemId = ci.CartItemId,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                ImageUrl = ci.Product.ProductImages.FirstOrDefault()?.Url,
                Quantity = ci.Quantity,
                Price = (ci.Variant != null && ci.Variant.PriceAdjustment.HasValue)
                        ? ci.Product.Price + ci.Variant.PriceAdjustment.Value
                        : ci.Product.Price,
                VariantName = ci.Variant?.VariantName
            }).ToList()
        };
    }

    public async Task<CartResponse> AddItemAsync(int userId, CartItemCreateRequest request)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId, CreatedAt = DateTime.Now };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        // Nếu sản phẩm + variant đã có trong giỏ thì tăng số lượng
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId
                                       && ci.ProductId == request.ProductId
                                       && ci.VariantId == request.VariantId);

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            var newItem = new CartItem
            {
                CartId = cart.CartId,
                ProductId = request.ProductId,
                VariantId = request.VariantId,
                Quantity = request.Quantity
            };
            _context.CartItems.Add(newItem);
        }

        await _context.SaveChangesAsync();
        return await GetCartByUserIdAsync(userId);
    }

    public async Task<CartResponse> UpdateItemAsync(int userId, int cartItemId, int quantity)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return null;

        var item = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.CartId == cart.CartId);
        if (item == null) return await GetCartByUserIdAsync(userId);

        if (quantity <= 0)
            _context.CartItems.Remove(item);
        else
            item.Quantity = quantity;

        await _context.SaveChangesAsync();
        return await GetCartByUserIdAsync(userId);
    }

    public async Task<bool> RemoveItemAsync(int userId, int cartItemId)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return false;

        var item = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.CartId == cart.CartId);
        if (item == null) return false;

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null) return false;

        _context.CartItems.RemoveRange(cart.CartItems);
        await _context.SaveChangesAsync();
        return true;
    }
}

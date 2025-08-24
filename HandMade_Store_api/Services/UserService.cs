using HandMade_Store_api.DTOs.User;
using HandMade_Store_api.Interfaces;
using HandMade_Store_api.Models;
using HandMade_Store_api.Interfaces;
using HandMade_Store_api.Models;
using Microsoft.EntityFrameworkCore;

namespace HandMade_Store_api.Services
{
    public class UserService : IUserService
    {
        private readonly HandmadeStoreContext _context;

        public UserService(HandmadeStoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            return await _context.Users
       .Select(u => new UserResponse
       {
           UserId = u.UserId,
           Name = u.Name,
           Email = u.Email,
           Phone = u.Phone,
           Address = u.Address,
           Role = u.Role,
           IsActive = u.IsActive ?? false,
           CreatedAt = u.CreatedAt
       }).ToListAsync();

        }

        public async Task<UserResponse> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            return new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                IsActive = user.IsActive ?? false,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserResponse> CreateUserAsync(UserCreateRequest request)
        {
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                Role = request.Role ?? "employee",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = true,
                CreatedAt = DateTime.Now
            };


            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive ?? true
            };
        }


        public async Task<UserResponse> UpdateUserAsync(int id, UserUpdateRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.Name = request.Name;
            user.Email = request.Email;
            user.Phone = request.Phone;
            user.Address = request.Address;



            await _context.SaveChangesAsync();

            return new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                IsActive = user.IsActive
            };
        }


        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<UserResponse>> SearchUsersAsync(string keyword) // 🔥 Search
        {
            return await _context.Users
                .Where(u => u.Name.Contains(keyword))
                //|| u.Email.Contains(keyword))
                .Select(u => new UserResponse
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.Phone,
                    Address = u.Address,
                    Role = u.Role,
                    IsActive = u.IsActive ?? false,
                    CreatedAt = u.CreatedAt
                }).ToListAsync();
        }
        public async Task<UserResponse> ChangeUserStatusAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            // Toggle trạng thái: true → false, false → true
            user.IsActive = !(user.IsActive ?? true); // null → true → toggle → false

            await _context.SaveChangesAsync();

            return new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                IsActive = user.IsActive.GetValueOrDefault(), // đảm bảo không null
                CreatedAt = user.CreatedAt
            };

        }
    }
}

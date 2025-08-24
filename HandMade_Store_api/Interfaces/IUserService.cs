using HandMade_Store_api.DTOs.User;

namespace HandMade_Store_api.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<UserResponse> GetUserByIdAsync(int id);
        Task<UserResponse> CreateUserAsync(UserCreateRequest request);
        Task<UserResponse> UpdateUserAsync(int id, UserUpdateRequest request);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<UserResponse>> SearchUsersAsync(string keyword); // 🔥 Search
                                                                          // Thêm hàm thay đổi trạng thái Active/Inactive
        Task<UserResponse> ChangeUserStatusAsync(int userId);
    }
}
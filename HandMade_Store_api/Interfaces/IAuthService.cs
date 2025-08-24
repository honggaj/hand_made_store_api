using HandMade_Store_api.DTOs.Auth;

namespace HandMade_Store_api.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);

        // 🔥 Thêm Google login
        Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request);
    }

}
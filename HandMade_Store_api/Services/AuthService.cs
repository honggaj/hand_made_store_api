using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FurnitureStoreAPI.DTOs.Auth;
using FurnitureStoreAPI.Interfaces;
using FurnitureStoreAPI.Models;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FurnitureStoreAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly FurnitureStoreContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthService(FurnitureStoreContext context, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Lưu user vào DB
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "customer",
                IsActive = request.IsActive   // 🔥 set từ DTO

            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Gửi email chào mừng
            var emailSubject = "Chào mừng bạn đến với Furniture Store! 🏠";

            var emailBody = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
            <div style='background: #667eea; color: white; padding: 25px; text-align: center; border-radius: 8px 8px 0 0;'>
                <h1>🏠 Furniture Store</h1>
                <p style='margin: 0;'>Chào mừng bạn đến với gia đình của chúng tôi!</p>
            </div>
            
            <div style='background: #f8f9fa; padding: 25px; border-radius: 0 0 8px 8px;'>
                <h2>Xin chào <span style='color: #667eea;'>{user.Name}</span>!</h2>
                
                <p>Cảm ơn bạn đã đăng ký tài khoản tại <strong>Furniture Store</strong>. 
                Chúng tôi vui mừng chào đón bạn trở thành thành viên của cửa hàng.</p>
                
                <div style='background: white; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                    <p><strong>Thông tin tài khoản:</strong></p>
                    <ul style='list-style: none; padding: 0;'>
                        <li>👤 <strong>Tên:</strong> {user.Name}</li>
                        <li>📧 <strong>Email:</strong> {user.Email}</li>
                        <li>📞 <strong>SĐT:</strong> {user.Phone}</li>
                    </ul>
                </div>
                
                <p><strong>Quyền lợi thành viên:</strong></p>
                <p>🛒 Mua sắm dễ dàng • 📱 Theo dõi đơn hàng • 🎁 Nhận ưu đãi đặc biệt</p>
                
                <div style='text-align: center; margin: 20px 0;'>
                    <a href='#' style='background: #667eea; color: white; padding: 12px 25px; 
                       text-decoration: none; border-radius: 5px; display: inline-block;'>
                       Khám phá ngay →
                    </a>
                </div>
                
                <p style='text-align: center; color: #666; font-size: 14px;'>
                    Liên hệ hỗ trợ: support@furniturestore.com | Hotline: 1900-xxxx
                </p>
            </div>
        </div>";

            // Gửi email
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

            return new AuthResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = "tạm thời chưa generate JWT",

            };
        }


        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Email hoặc mật khẩu không đúng");

            return new AuthResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = GenerateJwtToken(user)
            };
        }
        public async Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

            if (user == null)
            {
                user = new User
                {
                    Name = payload.Name ?? payload.Email.Split('@')[0],
                    Email = payload.Email,
                    IsActive = true,
                    Role = "customer",
                    PasswordHash = Guid.NewGuid().ToString(), // fake password
                    CreatedAt = DateTime.UtcNow,             // nếu DB NOT NULL
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return new AuthResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = GenerateJwtToken(user)
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

namespace HandMade_Store_api.DTOs.User
{
    public class UserCreateRequest
    {
        public string Name { get; set; }          // Tên nhân viên
        public string Email { get; set; }         // Email đăng nhập
        public string Password { get; set; }      // Mật khẩu (admin set)
        public string Phone { get; set; }         // SĐT
        public string Address { get; set; }       // Địa chỉ
        public string Role { get; set; } = "employee";  // Vai trò, mặc định staff
        public bool IsActive { get; set; } = true;   // Kích hoạt luôn
    }
}

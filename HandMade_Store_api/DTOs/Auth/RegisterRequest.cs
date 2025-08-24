namespace HandMade_Store_api.DTOs.Auth
{
    public class RegisterRequest
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public bool IsActive { get; set; } = true; // 🔥 auto active

    }
}

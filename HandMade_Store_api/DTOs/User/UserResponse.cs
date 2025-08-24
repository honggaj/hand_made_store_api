namespace HandMade_Store_api.DTOs.User
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}

namespace HandMade_Store_api.DTOs.User
{
    public class UserRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }   // nếu là update thì có thể bỏ
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}

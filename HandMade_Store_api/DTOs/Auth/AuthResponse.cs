﻿namespace HandMade_Store_api.DTOs.Auth
{
    public class AuthResponse
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public string Token { get; set; }   // JWT trả về khi login
    }
}
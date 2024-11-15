﻿namespace app.auth.DataModels
{
    public class SignUpInput
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class SignInInput
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string? AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiration { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}

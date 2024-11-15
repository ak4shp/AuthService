namespace app.auth.DataModels
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiryTime { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }

        public bool HasValidAccessToken(string? accessToken)
        {
            return AccessToken == accessToken;
        }

        public bool HasValidRefreshToken()
        {
            return RefreshToken != null;
        }

        public bool TokenExpired()
        {
            return AccessTokenExpiryTime < DateTime.UtcNow
        }


    }


    public class LoggedInUser
    {
        public string Email { get; set; } = string.Empty;
        public string? AccessToken { get; set; } = string.Empty;
    }

    public class AuthorisedUserData
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}

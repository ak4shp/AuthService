namespace app.auth.DataModels
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
    }


    public class LoggedInUser
    {
        public string Email { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
    }

    public class AuthenticatedUserData
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}

using app.auth.DataModels;
using app.auth.DataAccess;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace app.auth.Services
{
    public class UserService
    {
        private readonly ILogger<UserService> logger;
        private readonly DbClientContext context;
        private readonly IConfiguration configuration;

        public UserService(ILogger<UserService> logger, DbClientContext context, IConfiguration configuration)
        {
            this.logger = logger;
            this.context = context;
            this.configuration = configuration;
        }


        public async Task<User> RegisterUserAsync(string email, string password)
        {
            try
            {
                logger.LogInformation($"Registration started for User - {email}.");

                double.TryParse(configuration["JwtSettings:RefreshTokenExpirationDays"], out double expiry);
                if (expiry <= 0) expiry = 7;  // Default value if not configured 

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();
                logger.LogInformation($"Registration Successful for User - {email}.");
                return user;
            }
            catch (Exception ex)
            {
                logger.LogError($"Unable to Register User - {email}. Exception - {ex.Message}");
                throw;
            }
        }


        public async Task<User?> SignInUserAsync(string email, string password)
        {
            try
            {
                logger.LogInformation($"SignIn Begin for User - {email}.");

                var user = await AuthenticateUserAsync(email, password);
                if (user == null)
                    return null;

                double.TryParse(configuration["JwtSettings:AccessTokenExpirationMinutes"], out double accessTokenExpiry);
                double.TryParse(configuration["JwtSettings:RefreshTokenExpirationMinutes"], out double refreshTokenExpiry);

                if (accessTokenExpiry <= 0) accessTokenExpiry = 30;  // Default value if not configured
                if (refreshTokenExpiry <= 0) refreshTokenExpiry = 7;  // Default value if not configured  

                user.AccessToken = GenerateJwtToken(user);
                user.AccessTokenExpiryTime = DateTime.UtcNow.AddMinutes(accessTokenExpiry);

                user.RefreshToken = GenerateRefreshToken();
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenExpiry);

                context.Users.Update(user);
                await context.SaveChangesAsync();

                logger.LogInformation($"SignIn Successful for User - {email}.");
                return user;
            }
            catch (Exception ex)
            {
                logger.LogError($"Unable to Register User - {email}. Exception - {ex.Message}");
                throw;
            }
        }


        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                logger.LogInformation($"Authentication started for User - {email}.");

                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                    return null;

                logger.LogInformation($"Authentication Successful for User - {email}.");
                return user;
            }
            catch (Exception ex)
            {
                logger.LogError($"Unable to Authenticate User - {email}. Exception - {ex.Message}");
                throw;
            }
        }


        public async Task<User> GenerateNewAccessTokenAsync(User user)
        {
            try
            {
                logger.LogInformation($"Generating new access token for User - {user.Email}.");

                double.TryParse(configuration["JwtSettings:AccessTokenExpirationMinutes"], out double expiry);
                if (expiry <= 0) expiry = 30;  // Default value if not configured 

                user.AccessToken = GenerateJwtToken(user);
                user.AccessTokenExpiryTime = DateTime.UtcNow.AddMinutes(expiry);

                context.Users.Update(user);
                await context.SaveChangesAsync();

                logger.LogInformation($"New access token generated for User - {user.Email}.");
                return user;
            }
            catch (Exception ex)
            {
                logger.LogError($"Unable to generate new access token for User - {user.Email}. Exception - {ex.Message}");
                throw;
            }
        }


        public async Task RevokeRefreshTokenAsync(User user)
        {
            try
            {
                logger.LogInformation($"Revoking refresh token for User - {user.Email}.");

                user.RefreshToken = null;

                context.Users.Update(user);
                await context.SaveChangesAsync();

                logger.LogInformation($"Refresh token revoked for User - {user.Email}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Unable to revoke refresh token for User - {user.Email}. Exception - {ex.Message}");
                throw;
            }
        }


        public async Task<User?> GetExistingUserByEmail(string email) =>
            await context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetExistingUserByRefreshToken(string refreshToken) =>
            await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        public async Task<User?> GetExistingUserByValidatedRefreshToken(string refreshToken) =>
            await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow);


        #region Utility
        private string GenerateRefreshToken() =>
           Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private string GenerateJwtToken(User user)
        {
            try
            {
                logger.LogInformation($"Generating access token for User - {user.Email}.");

                var jwtSettings = configuration.GetSection("JwtSettings");
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("id", user.Id.ToString())
                };

                double.TryParse(jwtSettings["AccessTokenExpirationMinutes"], out double expiry);
                if (expiry <= 0) expiry = 30;  // Default value if not configured 

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expiry),
                    signingCredentials: credentials
                );

                logger.LogInformation($"Access token generated for User - {user.Email}.");
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                logger.LogError($"Unable to generate access token for User - {user.Email}. Exception - {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
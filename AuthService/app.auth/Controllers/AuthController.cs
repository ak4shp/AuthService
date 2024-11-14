using app.auth.DataAccess;
using app.auth.DataModels;
using app.auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app.auth.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DbClientContext context;
        private readonly UserService userService;
        private readonly IConfiguration configuration;

        public AuthController(DbClientContext context, IConfiguration configuration)
        {
            this.context = context;
            this.userService = new UserService(context, configuration);
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> SignUp(UserSignUp dto)
        {
            var user = await userService.RegisterUserAsync(dto.Email, dto.Password);
            return Ok(new { user.Id, user.Email });
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn(UserSignIn dto)
        {
            var user = await userService.AuthenticateUserAsync(dto.Email, dto.Password);
            if (user == null)
                return Unauthorized(new { Message = "Invalid credentials" });

            var token = userService.GenerateJwtToken(user);
            var refreshToken = await userService.GenerateRefreshTokenAsync(user);
            return Ok(new AuthResponse { Token = token, RefreshToken = refreshToken, Expiration = DateTime.UtcNow.AddMinutes(30) });
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow);
            if (user == null) return Unauthorized(new { Message = "Invalid or expired refresh token" });

            var newToken = userService.GenerateJwtToken(user);
            var newRefreshToken = await userService.GenerateRefreshTokenAsync(user);
            return Ok(new AuthResponse { Token = newToken, RefreshToken = newRefreshToken, Expiration = DateTime.UtcNow.AddMinutes(30) });
        }

        [HttpPost]
        [Route("revoke-token")]
        public async Task<IActionResult> RevokeToken(string refreshToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null) return NotFound(new { Message = "User not found" });

            await userService.RevokeRefreshTokenAsync(user);
            return Ok(new { Message = "Refresh token revoked" });
        }


        [HttpPost]
        [Route("get-user")]
        public async Task<IActionResult> GetUserId(LoggedInUser user)
        {
            var loggedInUser = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);// && u.RefreshToken == user.RefreshToken);

            if (loggedInUser == null) return NotFound(new { Message = "User not found" });
           
            else if(loggedInUser.RefreshToken == null || loggedInUser.RefreshToken != user.RefreshToken) return NotFound(new { Message = "Invalid or expired refresh token" });

            return Ok(new AuthenticatedUserData { Id = loggedInUser.Id, Email = loggedInUser.Email });
        }
    }

}

using app.auth.DataAccess;
using app.auth.DataModels;
using app.auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app.auth.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserService userService;

        public AuthController(UserService userService)
        {
            this.userService = userService;
        }


        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> SignUp(UserSignUp dto)
        {
            try
            {
                var user = await userService.RegisterUserAsync(dto.Email, dto.Password);
                return Ok(new { user.Id, user.Email });
            }
            catch (Exception ex)
            {
                var errorResponse = new { Message = "An error occurred on the server.", Details = ex.Message };
                return new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }


        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn(UserSignIn dto)
        {
            try
            {
                var user = await userService.SignInUserAsync(dto.Email, dto.Password);
                if (user == null)
                    return Unauthorized(new { Message = "Invalid credentials" });

                return Ok(new AuthResponse
                {
                    AccessToken = user.AccessToken,
                    RefreshToken = user.RefreshToken,
                    AccessTokenExpiration = user.AccessTokenExpiryTime,
                    RefreshTokenExpiration = user.RefreshTokenExpiryTime
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new { Message = "An error occurred on the server.", Details = ex.Message };
                return new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }


        [HttpPost]
        [Route("get-user-details")]
        public async Task<IActionResult> GetUserId(LoggedInUser user)
        {
            try
            {
                User? loggedInUser = await userService.GetExistingUserByEmail(user.Email);

                if (loggedInUser == null)
                    return NotFound(new { Message = "User not found!" });

                else if (loggedInUser.RefreshToken == null)
                    return Unauthorized(new { Message = "Refresh token not found!" });

                else if (loggedInUser.AccessToken != user.AccessToken || loggedInUser.AccessTokenExpiryTime < DateTime.UtcNow)
                    return Unauthorized(new { Message = "Invalid or expired access token!" });

                return Ok(new AuthorisedUserData { Id = loggedInUser.Id, Email = loggedInUser.Email });
            }
            catch (Exception ex)
            {
                var errorResponse = new { Message = "An error occurred on the server.", Details = ex.Message };
                return new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

   

        [HttpPost]
        [Route("revoke-token")]
        public async Task<IActionResult> RevokeToken(string refreshToken)
        {
            try
            {
                User? user = await userService.GetExistingUserByRefreshToken(refreshToken);

                if (user == null) return NotFound(new { Message = "User not found" });

                await userService.RevokeRefreshTokenAsync(user);
                return Ok(new { Message = "Refresh token revoked" });
            }
            catch (Exception ex)
            {
                var errorResponse = new { Message = "An error occurred on the server.", Details = ex.Message };
                return new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }



        [HttpPost]
        [Route("refresh-access-token")]
        public async Task<IActionResult> RefreshAccessToken(string refreshToken)
        {
            try
            {
                User? user = await userService.GetExistingUserByValidatedRefreshToken(refreshToken);

                if (user == null) return Unauthorized(new { Message = "Invalid or expired refresh token" });

                var newUser = await userService.GenerateNewAccessTokenAsync(user);

                return Ok(new AuthResponse
                {
                    AccessToken = newUser.AccessToken,
                    RefreshToken = newUser.RefreshToken,
                    AccessTokenExpiration = newUser.AccessTokenExpiryTime,
                    RefreshTokenExpiration = newUser.RefreshTokenExpiryTime
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new { Message = "An error occurred on the server.", Details = ex.Message };
                return new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using PokeAPIService.Models.Auth;
using PokeAPIService.Services;
namespace PokeAPIService.Controllers

{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenStore _refreshStore;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ITokenService tokenService,
            IRefreshTokenStore refreshStore, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _refreshStore = refreshStore;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login()
        {
            // Demo user
            var userId = "test-user";

            var accessToken = _tokenService.CreateAccessToken(userId);
            var refreshToken = _tokenService.CreateRefreshToken();

            _refreshStore.Save(userId, refreshToken);

            Response.Cookies.Append(
                "refreshToken",
                refreshToken.Token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,          // HTTPS only
                    SameSite = SameSiteMode.Strict,
                    Path = "/api/auth/refresh",
                    Expires = refreshToken.ExpiresAt
                });
            _logger.LogInformation("Issued access token for UserId={UserId}", userId);

            return Ok(new AuthToken
            {
                AccessToken = accessToken
            });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh()
        {
            //Demo user
            var userId = "test-user";

            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken is null)
            {
                _logger.LogWarning("Invalid or revoked refresh token attempt for UserId={UserId}", userId);

                return Unauthorized("Invalid refresh token");
            }
            var stored = _refreshStore.Get(userId, refreshToken);
            if (stored == null || stored.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid or revoked refresh token attempt for UserId={UserId}", userId);
                return Unauthorized("Invalid refresh token");
            }

            //Rotate token
            _refreshStore.Revoke(userId, refreshToken);

            var newRefreshToken = _tokenService.CreateRefreshToken();
            _refreshStore.Save(userId, newRefreshToken);

            Response.Cookies.Append(
                "refreshToken",
                newRefreshToken.Token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,          // HTTPS only
                    SameSite = SameSiteMode.Strict,
                    Path = "/api/auth/refresh",
                    Expires = newRefreshToken.ExpiresAt
                });
            _logger.LogInformation("Issued access token for UserId={UserId}", userId);
            return Ok(new AuthToken
            {
                AccessToken = _tokenService.CreateAccessToken(userId)
            });
        }
    }


}


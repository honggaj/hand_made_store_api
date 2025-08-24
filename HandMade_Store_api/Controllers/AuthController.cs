using HandMade_Store_api.DTOs.Auth;
using HandMade_Store_api.DTOs.Auth;
using HandMade_Store_api.DTOs.Auth;
using HandMade_Store_api.Interfaces;
using HandMade_Store_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandMade_Store_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // POST: api/auth/login-google
        [HttpPost("login-google")]
        public async Task<ActionResult<AuthResponse>> LoginWithGoogle([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var authResponse = await _authService.LoginWithGoogleAsync(request);
                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
using AlamandaApi.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace AlamandaApi.Controllers {
  [ApiController]
  [Route("[controller]")]
  public class AuthController : ControllerBase {
    private readonly AuthService _authService;

    public AuthController(AuthService authService) {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request) {
      try {
        var authResponse = await _authService.RegisterUserAsync(request);
        return Ok(authResponse);
      }
      catch (Exception ex) {
        return BadRequest(new { ex.Message });
      }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request) {
      try {
        var authResponse = await _authService.LoginUserAsync(request);

        return Ok(authResponse);
      } catch (Exception ex) {
        return Unauthorized(new { ex.Message });
      }
    }

    [HttpPost("admin")]
    public async Task<IActionResult> AdminLogin([FromBody] LoginRequest request) {
      try {
        var authResponse = await _authService.LoginAdminAsync(request);
        return Ok(authResponse);
      } catch (Exception ex) {
        return Unauthorized(new { ex.Message });
      }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest body) {
      try {
        var authResponse = await _authService.RefreshTokenAsync(body.RefreshToken);
        return Ok(authResponse);
      } catch (Exception ex) {
        return Unauthorized(new { ex.Message });
      }

    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest body) {
      try {
        var success = await _authService.LogoutAsync(body.RefreshToken);
        return Ok(success);
      } catch (Exception ex) {
        return Unauthorized(new { ex.Message });
      }
    }
  }

}

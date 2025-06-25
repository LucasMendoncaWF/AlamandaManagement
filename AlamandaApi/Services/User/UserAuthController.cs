using AlamandaApi.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace AlamandaApi.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
      try
      {
        var authResponse = await _userService.RegisterUserAsync(request);
        return Ok(authResponse);
      }
      catch (Exception ex)
      {
        return BadRequest(new { ex.Message });
      }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
      var authResponse = await _userService.LoginUserAsync(request);
      if (authResponse == null)
        return Unauthorized(new { Message = "Credenciais inválidas" });

      return Ok(authResponse);
    }

    [HttpPost("admin")]
    public async Task<IActionResult> AdminLogin([FromBody] LoginRequest request)
    {
      var authResponse = await _userService.LoginAdminAsync(request);
      if (authResponse == null)
        return Unauthorized(new { Message = "Credenciais inválidas" });

      return Ok(authResponse);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest body)
    {
      var authResponse = await _userService.RefreshTokenAsync(body.RefreshToken);
      if (authResponse == null)
        return Unauthorized(new { Message = "Refresh token inválido ou expirado" });

      return Ok(authResponse);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest body)
    {
      var success = await _userService.LogoutAsync(body.RefreshToken);
      if (!success)
        return Unauthorized(new { Message = "Refresh token inválido" });

      return Ok(new { Message = "Logout efetuado com sucesso" });
    }
  }

}

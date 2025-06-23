using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AlamandaApi.Services.User
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly string _jwtSecret;

        public AuthController(UserService userService)
        {
          _userService = userService;
          _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "";
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest user)
        {
          var existingUser = await _userService.GetByEmail(user.Email);
          if (existingUser != null)
              return BadRequest("Email já cadastrado");

          var newUser = new UserModel
          {
            Username = user.Username,
            Email = user.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
            Permission = "user"
          };
          await _userService.Create(newUser);

          var jwt = GenerateJwtToken(newUser);
          var refreshToken = GenerateRefreshToken();

          await _userService.UpdateRefreshToken(newUser, "", refreshToken);

          return Ok(new { Token = jwt, RefreshToken = refreshToken.Token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
          var user = await _userService.GetByEmail(login.Email);
          if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
              return Unauthorized("Credenciais inválidas");

          var jwt = GenerateJwtToken(user);
          var refreshToken = GenerateRefreshToken();
          await _userService.UpdateRefreshToken(user, "", refreshToken);

          return Ok(new { Token = jwt, RefreshToken = refreshToken.Token });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
          var user = await _userService.GetByRefreshToken(refreshToken);
          if (user == null)
            return Unauthorized("Refresh token inválido");

          var usedToken = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken && !t.IsRevoked && t.Expires > DateTime.UtcNow);
          if (usedToken == null)
            return Unauthorized("Refresh token inválido ou expirado");

          var newJwt = GenerateJwtToken(user);
          var newRefreshToken = GenerateRefreshToken();
          await _userService.UpdateRefreshToken(user, usedToken.Token, newRefreshToken);

          return Ok(new { Token = newJwt, RefreshToken = newRefreshToken.Token });
        }
        
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
          var user = await _userService.GetByRefreshToken(refreshToken);
          if (user == null)
            return Unauthorized("Refresh token inválido");

          await _userService.RemoveRefreshToken(user, refreshToken);
          return Ok("Logout efetuado com sucesso");
        }

        private string GenerateJwtToken(UserModel user)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.UTF8.GetBytes(_jwtSecret);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(
          [
            new Claim(ClaimTypes.Name, user.Id!),
            new Claim(ClaimTypes.Role, user.Permission)
          ]),
        Expires = DateTime.UtcNow.AddDays(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }

    private RefreshTokenModel GenerateRefreshToken()
    {
      return new RefreshTokenModel
      {
        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
        Expires = DateTime.UtcNow.AddDays(10),
        IsRevoked = false
      };
    }
  }
}

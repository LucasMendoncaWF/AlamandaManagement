using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AlamandaApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AlamandaApi.Services.CRUD;

namespace AlamandaApi.Services.User {
  public class AuthService {
    private readonly AppDbContext _context;
    private readonly string _jwtSecret;
    private readonly CRUDService<UserModel> _crudService;

    public AuthService(AppDbContext context, IConfiguration config, CRUDService<UserModel> crudService) {
      _context = context;
      _jwtSecret = config["JWT_SECRET"] ?? throw new ArgumentNullException("JWT_SECRET não configurado");
      _crudService = crudService;
    }

    public async Task<object> RegisterUserAsync(RegisterRequest request) {
      if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        throw new Exception("User already exists");

      using var tx = await _context.Database.BeginTransactionAsync();
      try {
        var user = new UserModel {
          Email = request.Email,
          UserName = request.UserName
        };
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var response = await GenerateAuthResponse(user);
        await tx.CommitAsync();
        _crudService.ClearCache();
        return response;
      } catch {
        await tx.RollbackAsync();
        throw;
      }
    }

    public Task<object> LoginUserAsync(LoginRequest request) => LoginAsync(request);
    public Task<object> LoginAdminAsync(LoginRequest request) => LoginAsync(request, adminOnly: true);

    private async Task<object> LoginAsync(LoginRequest request, bool adminOnly = false) {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
      if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        throw new Exception("Verify your credentials and try again");

      if (adminOnly && user.PermissionId != 1)
        throw new Exception("Unauthorized");

      return await GenerateAuthResponse(user);
    }

    public async Task<object> RefreshTokenAsync(string refreshToken) {
      if (string.IsNullOrWhiteSpace(refreshToken))
        throw new Exception("Invalid Refresh Token");

      var oldToken = await GetRefreshToken(refreshToken);
      var user = await GetByRefreshToken(refreshToken);

      if (oldToken == null || user == null || oldToken.Expires <= DateTime.UtcNow)
        throw new Exception("Expired Refresh Token");

      _context.RefreshTokens.Remove(oldToken);
      var newToken = GenerateRefreshToken(user.Id);
      _context.RefreshTokens.Add(newToken);
      await _context.SaveChangesAsync();

      return new {
        Token = await GenerateJwtToken(user),
        RefreshToken = newToken.Token
      };
    }

    public async Task<object> LogoutAsync(string refreshToken) {
      var token = await GetRefreshToken(refreshToken)
        ?? throw new Exception("Invalid Refresh Token");

      _context.RefreshTokens.Remove(token);
      await _context.SaveChangesAsync();
      return new { message = "Logout succeed" };
    }

    private async Task<object> GenerateAuthResponse(UserModel user) {
      var token = await GenerateJwtToken(user);
      var refreshToken = GenerateRefreshToken(user.Id);

      _context.RefreshTokens.Add(refreshToken);
      await _context.SaveChangesAsync();

      return new { Token = token, RefreshToken = refreshToken.Token };
    }

    private async Task<string> GenerateJwtToken(UserModel user) {
      var key = Encoding.UTF8.GetBytes(_jwtSecret);
      var permission = await _context.Permissions
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == user.PermissionId)
        ?? throw new Exception("Permission not found");

      var claims = new[] {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, permission.Name)
      };

      var tokenDescriptor = new SecurityTokenDescriptor {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddDays(1),
        SigningCredentials = new SigningCredentials(
          new SymmetricSecurityKey(key),
          SecurityAlgorithms.HmacSha256Signature
        )
      };

      return new JwtSecurityTokenHandler().WriteToken(
        new JwtSecurityTokenHandler().CreateToken(tokenDescriptor)
      );
    }

    private RefreshTokenModel GenerateRefreshToken(int userId) => new() {
      UserId = userId,
      Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
      Expires = DateTime.UtcNow.AddDays(10)
    };

    private Task<RefreshTokenModel?> GetRefreshToken(string token) =>
      _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);

    private Task<UserModel?> GetByRefreshToken(string token) =>
      _context.RefreshTokens
        .Include(rt => rt.User)
        .Where(rt => rt.Token == token && rt.Expires > DateTime.UtcNow)
        .Select(rt => rt.User)
        .FirstOrDefaultAsync();
  }
}

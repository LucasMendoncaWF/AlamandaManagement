using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AlamandaApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AlamandaApi.Services.User {
  public class UserService {
    private readonly AppDbContext _context;
    private readonly string _jwtSecret;

    public UserService(AppDbContext context, IConfiguration config) {
      _context = context;
      _jwtSecret = config["JWT_SECRET"] ?? throw new ArgumentNullException("JWT_SECRET não configurado");
    }

    public async Task<object?> RegisterUserAsync(RegisterRequest request) {
      if (await GetByEmail(request.Email) != null) {
        throw new Exception("Usuário ja cadastrado");
      }

      using var transaction = await _context.Database.BeginTransactionAsync();
      try {
        var user = ObjectMapperUtil.CopyWithCapitalization<RegisterRequest, UserModel>(request);
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    
        var authResponse = await GenerateAuthResponse(user);
        await transaction.CommitAsync();
        return authResponse;
      } catch {
        await transaction.RollbackAsync();
        throw;
      }
    }

    public async Task<object?> LoginUserAsync(LoginRequest request) {
      var user = await GetByEmail(request.Email);
      if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password)) {
        throw new Exception("Verifique suas credenciais e tente novamente");
      }
      return await GenerateAuthResponse(user);
    }

    public async Task<object?> LoginAdminAsync(LoginRequest request) {
      var user = await GetByEmail(request.Email);
      if (user == null || user.Permission != "admin" || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password)) {
        throw new Exception("Verifique suas credenciais e tente novamente");
      }
      return await GenerateAuthResponse(user);
    }


    public async Task<object?> RefreshTokenAsync(string refreshToken) {
      if (string.IsNullOrWhiteSpace(refreshToken)) {
        throw new Exception("Refresh Token Inválido");
      }
        
      var user = await GetByRefreshToken(refreshToken);
      var oldToken = await GetRefreshToken(refreshToken);
      if (oldToken == null) {
        throw new Exception("Refresh Token Inválido");
      }
        
      _context.RefreshTokens.Remove(oldToken);
      await _context.SaveChangesAsync();

      if (user == null || oldToken.Expires <= DateTime.UtcNow) {
        throw new Exception("Refresh Token Expirado");
      }

      var newToken = GenerateRefreshToken(user.Id);
      await ReplaceRefreshToken(oldToken, user.Id, newToken);
      return new {
        Token = GenerateJwtToken(user),
        RefreshToken = newToken.Token
      };
    }

    public async Task<object?> LogoutAsync(string refreshToken) {
      if (string.IsNullOrWhiteSpace(refreshToken)) {
        throw new Exception("Refresh Token inválido");
      }
      var tokenEntity = await GetRefreshToken(refreshToken);
      if (tokenEntity == null) {
        throw new Exception("Refresh Token inválido");
      }
      await RemoveRefreshToken(tokenEntity);
      return new {
        message = "Logout Efetuado"
      };
    }

    private string GenerateJwtToken(UserModel user) {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.UTF8.GetBytes(_jwtSecret);

      var claims = new[] {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.Permission)
      };

      var descriptor = new SecurityTokenDescriptor {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddDays(1),
        SigningCredentials = new SigningCredentials(
          new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      return tokenHandler.WriteToken(tokenHandler.CreateToken(descriptor));
    }

    private RefreshTokenModel GenerateRefreshToken(int userId) => new() {
      UserId = userId,
      Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
      Expires = DateTime.UtcNow.AddDays(10)
    };

    private async Task<object> GenerateAuthResponse(UserModel user) {
      var token = GenerateJwtToken(user);
      var refreshToken = GenerateRefreshToken(user.Id);
      await AddRefreshToken(refreshToken);

      return new { Token = token, RefreshToken = refreshToken.Token };
    }

    public async Task<UserModel?> GetByEmail(string email) {
      return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<UserModel?> GetByRefreshToken(string token) {
      return await _context.RefreshTokens
        .Include(rt => rt.User)
        .Where(rt => rt.Token == token && rt.Expires > DateTime.UtcNow)
        .Select(rt => rt.User)
        .FirstOrDefaultAsync();
    }


    public async Task<RefreshTokenModel?> GetRefreshToken(string token) {
      return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }
      

    public async Task AddRefreshToken(RefreshTokenModel token) {
      _context.RefreshTokens.Add(token);
      await _context.SaveChangesAsync();
    }

    public async Task RemoveRefreshToken(RefreshTokenModel token) {
      _context.RefreshTokens.Remove(token);
      await _context.SaveChangesAsync();
    }

    public async Task ReplaceRefreshToken(RefreshTokenModel oldToken, int userId, RefreshTokenModel newToken) {
      var tokenToRemove = await _context.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.Token == oldToken.Token && rt.UserId == userId);

      if (tokenToRemove != null)
        _context.RefreshTokens.Remove(tokenToRemove);

      _context.RefreshTokens.Add(newToken);
      await _context.SaveChangesAsync();
    }
  }
}
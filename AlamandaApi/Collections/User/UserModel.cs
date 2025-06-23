using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AlamandaApi.Services.User
{
  public class UserModel
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("username")]
    public string Username { get; set; } = null!;

    [BsonElement("email")]
    public string Email { get; set; } = null!;

    [BsonElement("password")]
    public string Password { get; set; } = null!;

    [BsonElement("permission")]
    public string Permission { get; set; } = "user";

    [BsonElement("refreshTokens")]
    public List<RefreshTokenModel> RefreshTokens { get; set; } = new List<RefreshTokenModel>();
  }

  public class LoginRequest
  {
    [Required(ErrorMessage = "E-mail é obrigatório")]
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
  }

  public class RegisterRequest
  {
    [Required(ErrorMessage = "Username é obrigatório")]
    [MaxLength(50, ErrorMessage = "Username pode ter no máximo 50 caracteres")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [MaxLength(50, ErrorMessage = "Email pode ter no máximo 50 caracteres")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    [MaxLength(50, ErrorMessage = "Senha pode ter no máximo 50 caracteres")]
    public string Password { get; set; } = null!;
  }

  public class RefreshTokenModel
  {
    public string Token { get; set; } = null!;
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; } = false;
  }
}

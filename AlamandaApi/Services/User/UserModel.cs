using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlamandaApi.Services.User
{
  public class UserModel
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Username é obrigatório")]
    [StringLength(50, ErrorMessage = "Username pode ter no máximo 50 caracteres")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(50, ErrorMessage = "Email pode ter no máximo 50 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Permission { get; set; } = "user";

    public ICollection<RefreshTokenModel> RefreshTokens { get; set; } = new List<RefreshTokenModel>();
  }

  public class LoginRequest
  {
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
  }

  public class RegisterRequest
  {
    [Required(ErrorMessage = "Username é obrigatório")]
    [StringLength(50, ErrorMessage = "Username pode ter no máximo 50 caracteres")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(50, ErrorMessage = "Email pode ter no máximo 50 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 50 caracteres")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
  }

  public class RefreshTokenModel
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Token { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime Expires { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; set; } = null!;
  }

  public class RefreshTokenRequest
  {
    public string RefreshToken { get; set; } = string.Empty;
  }
}

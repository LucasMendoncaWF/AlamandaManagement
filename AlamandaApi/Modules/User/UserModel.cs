using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlamandaApi.Services.User {
  
  public class PermissionModel {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = null!;
  }

  public class UserEdit {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(50)]
    public string Email { get; set; } = string.Empty;

    public string? Picture { get; set; }

    public int? PermissionId { get; set; }
    public PermissionModel? Permission { get; set; }
  }

  public class UserModel : UserEdit {
    [Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public ICollection<RefreshTokenModel> RefreshTokens { get; set; } = new List<RefreshTokenModel>();
  }

  public class RefreshTokenModel {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

  public class LoginRequest {
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
  }

  public class RegisterRequest : LoginRequest {
    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;
  }

  public class RefreshTokenRequest {
    public string RefreshToken { get; set; } = string.Empty;
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AlamandaApi.Services.Cart;
using AlamandaApi.Services.Comics;

namespace AlamandaApi.Services.User {

  public class LoginRequest {
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
  }

  public class RegisterRequest : LoginRequest {
    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;
  }

  // _________________________ ADMIN CRUD __________________________________
  public class UserCreate {
    [Required, StringLength(50)]
    [JsonPropertyName("username")]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(50)]
    public string Email { get; set; } = string.Empty;

    public string? Picture { get; set; }

    public int? PermissionId { get; set; }
    public PermissionModel? Permission { get; set; }
  }

  public class UserEdit : UserCreate {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

  }

  public class UserModel : UserEdit {
    [Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public ICollection<RefreshTokenModel> RefreshTokens { get; set; } = new List<RefreshTokenModel>();
    public CartModel Cart { get; set; } = new CartModel();
    public virtual ICollection<ComicModel> Comics { get; set; } = new List<ComicModel>();
  }

  // _____________________________ REFRESH TOKEN _______________________________
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

  public class RefreshTokenRequest {
    public string RefreshToken { get; set; } = string.Empty;
  }

  // _____________________________ PERMISSION _______________________________________
  public class PermissionModel {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = null!;
  }
  
  //__________________________ RESPONSES _____________________________________________
  public class UserListView {
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Picture { get; set; }
    public int? PermissionId { get; set; }
    public PermissionModel? Permission { get; set; }
  }
}

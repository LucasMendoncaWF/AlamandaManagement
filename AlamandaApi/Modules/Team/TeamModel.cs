using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AlamandaApi.Services.Comics;
using AlamandaApi.Services.Role;

namespace AlamandaApi.Services.Team {

  public class TeamMemberBaseModel {
    [Required]
    [MaxLength(100)]
    [JsonPropertyOrder(1)]
    public string Name { get; set; } = null!;

    [MaxLength(50)]
    [JsonPropertyOrder(2)]
    public string Social { get; set; } = null!;

    public string? Picture { get; set; } = null;
  }
  public class TeamMemberCreationModel : TeamMemberBaseModel {
    public List<int> RolesIds { get; set; } = new List<int>();
    public virtual List<int> ComicsIds { get; set; } = new List<int>();
  }

  public class TeamMemberEditModel : TeamMemberCreationModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
  }

  public class TeamMemberModel : TeamMemberBaseModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [JsonPropertyOrder(4)]
    public virtual ICollection<RoleModel> Roles { get; set; } = new List<RoleModel>();
    [JsonPropertyOrder(5)]
    public virtual ICollection<ComicModel> Comics { get; set; } = new List<ComicModel>();
  }
}

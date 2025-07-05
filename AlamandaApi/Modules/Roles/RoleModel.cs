using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlamandaApi.Services.Team;

namespace AlamandaApi.Services.Role {
  public class RoleCreationModel {
    public virtual ICollection<TeamMemberModel> TeamMembers { get; set; } = new List<TeamMemberModel>();
  }
  public class RoleModel : RoleCreationModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Name_En { get; set; } = null!;
  }

  public class RoleModelDto {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Name_En { get; set; } = null!;
  }
}

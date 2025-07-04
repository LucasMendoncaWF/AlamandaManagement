using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlamandaApi.Services.Comics;

namespace AlamandaApi.Services.Team {

  public class TeamMemberBaseModel {
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(50)]
    public string Social { get; set; } = null!;

    public string? Picture { get; set; } = null;
    public RoleModel? Role { get; set; }
    public virtual int? RoleId { get; set; } = null;
  }
  public class TeamMemberCreationModel : TeamMemberBaseModel {
    public virtual List<string> ComicsIds { get; set; } = new List<string>();
  }
  
  public class TeamMemberEditModel : TeamMemberCreationModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
  }

  public class RoleModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; } = null!;
  }


  public class TeamMemberModel : TeamMemberBaseModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public virtual ICollection<ComicModel> Comics { get; set; } = new List<ComicModel>();
  }
}

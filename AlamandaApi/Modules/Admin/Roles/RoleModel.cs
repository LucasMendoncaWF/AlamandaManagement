using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlamandaApi.Services.Language;
using AlamandaApi.Services.Team;

namespace AlamandaApi.Services.Role {
  public class RoleCreationModel {
    public virtual ICollection<TeamMemberModel> TeamMembers { get; set; } = new List<TeamMemberModel>();
    public virtual ICollection<RoleTranslationModel> Translations { get; set; } = new List<RoleTranslationModel>();
  }
  public class RoleModel : RoleCreationModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
  }

  // __________________ TRANSLATIONS __________________________________
  public class RoleTranslationModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int RoleId { get; set; }
    public int LanguageId { get; set; }

    [ForeignKey("RoleId")]
    public virtual RoleModel? Role { get; set; } = null!;

    [ForeignKey("LanguageId")]
    public virtual LanguageModel? Language { get; set; } = null!;

    public string Name { get; set; } = null!;
  }

  //__________________________ RESPONSES _____________________________________________
  public class RoleModelView {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? Name { get; set; } = null!;
    public List<RoleTranslationModel> Translations { get; set; } = new();
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlamandaApi.Services.Team;

namespace AlamandaApi.Services.Comics {
  public class ComicCreationModel {
    public string Name { get; set; } = null!;
    public virtual ICollection<TeamMemberModel> TeamMembers { get; set; } = new List<TeamMemberModel>();
    public string? Picture { get; set; } = null;
  }

  public class ComicModel : ComicCreationModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
  }
}

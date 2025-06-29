using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlamandaApi.Services.Team;

namespace AlamandaApi.Services.Comics {

  public class ComicModel
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public virtual ICollection<TeamMemberModel> TeamMembers { get; set; } = new List<TeamMemberModel>();
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlamandaApi.Services.Category;
using AlamandaApi.Services.Team;

namespace AlamandaApi.Services.Comics {
  public class ComicBaseModel {
    public string Name { get; set; } = null!;
    public string? Name_En { get; set; } = null!;
    public string? Picture { get; set; } = null;
    public virtual ICollection<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
    public virtual ICollection<TeamMemberModel> TeamMembers { get; set; } = new List<TeamMemberModel>();
  }

  public class ComicCreationModel : ComicBaseModel {
    public List<int> CategoriesIds { get; set; } = new List<int>();
  }

  public class ComicEditModel : ComicCreationModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
  }



  public class ComicModel : ComicBaseModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
  }
}

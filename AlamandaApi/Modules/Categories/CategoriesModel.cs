using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlamandaApi.Services.Comics;

namespace AlamandaApi.Services.Category {
  public class CategoryModelDTO {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Name_En { get; set; } = null!;
  }
  public class CategoryCreationModel {
    public string Name { get; set; } = null!;
    public string? Name_En { get; set; } = null!;
    public virtual ICollection<ComicModel> Comics { get; set; } = new List<ComicModel>();
  }
  public class CategoryModel: CategoryCreationModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
  }
}

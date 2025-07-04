using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using AlamandaApi.Services.Comics;

namespace AlamandaApi.Services.Category {
  public class CategoryCreation {
    public string Name { get; set; } = null!;
    public virtual int ComicId { get; set; } = new int();
    
    //public virtual ICollection<ComicModel> Comics { get; set; } = new List<ComicModel>();
  }
  public class CategoryModel: CategoryCreation {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
  }
}

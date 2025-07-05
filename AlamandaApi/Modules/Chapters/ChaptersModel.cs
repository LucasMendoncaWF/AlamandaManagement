using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlamandaApi.Services.Chapters {
  public class ChapterCreationModel {
    public string Name { get; set; } = null!;
    public string? Name_En { get; set; } = null!;
    public virtual int ComicId { get; set; } = new int();
  }
  public class ChapterModel: ChapterCreationModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
  }
}

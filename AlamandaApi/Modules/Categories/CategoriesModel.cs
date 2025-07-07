using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlamandaApi.Services.Comics;
using AlamandaApi.Services.Language;

namespace AlamandaApi.Services.Category
{
  public class CategoryModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public virtual ICollection<ComicModel> Comics { get; set; } = new List<ComicModel>();
    public virtual ICollection<CategoryTranslationModel> Translations { get; set; } = new List<CategoryTranslationModel>();
  }

  public class CategoryTranslationModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int CategoryId { get; set; }
    public int LanguageId { get; set; }

    [ForeignKey("CategoryId")]
    public virtual CategoryModel? Category { get; set; } = null!;

    [ForeignKey("LanguageId")]
    public virtual LanguageModel? Language { get; set; } = null!;

    public string Name { get; set; } = null!;
  }

  public class CategoryModelDTO {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<CategoryTranslationModel> Translations { get; set; } = new();
  }

  public class CategoryFormDTO {
    public List<CategoryTranslationModel> Translations { get; set; } = new();
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlamandaApi.Services.Category;
using AlamandaApi.Services.Language;
using AlamandaApi.Services.Team;

namespace AlamandaApi.Services.Comics {
  public class ComicBaseModel {
    public virtual ICollection<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
    public virtual ICollection<TeamMemberModel> TeamMembers { get; set; } = new List<TeamMemberModel>();
    [NotMapped]
    public List<int> CategoriesIds { get; set; } = new List<int>();

    [ForeignKey("Color")]
    public int? Color { get; set; }
    public virtual ColorModel? ColorModel { get; set; }
    
    [ForeignKey("Cover")]
    public int? Cover { get; set; }
    public virtual CoverModel? CoverModel { get; set; }

    [ForeignKey("Status")]
    public int? Status { get; set; }
    public virtual StatusModel? StatusModel { get; set; }

    public int? Available_Storage { get; set; }
    public int? Total_Pages { get; set; }
    public DateTime? Publish_Date { get; set; }
  }

  public class ComicModel : ComicBaseModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public virtual ICollection<ComicTranslationsModel> Translations { get; set; } = new List<ComicTranslationsModel>();
  }

  // __________________ TRANSLATIONS __________________________________
  public class ComicTranslationsModel {
    public int Id { get; set; }

    public int? ComicId { get; set; }
    public int? LanguageId { get; set; }

    [ForeignKey("ComicId")]
    public virtual ComicModel? Comic { get; set; } = null!;
    [ForeignKey("LanguageId")]
    public virtual LanguageModel? Language { get; set; } = null!;

    public List<string?>? Pictures { get; set; } = new List<string?>();
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public string? Amazon { get; set; } = null!;
    public string? Catarse { get; set; } = null!;
    public float? Price { get; set; } = 0;
    public int? Discount { get; set; } = 0;
  }

  // _________________ RESPONSES __________________________________

  public class ComicListModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    [ForeignKey("Color")]
    public int? Color { get; set; }
    [ForeignKey("Cover")]
    public int? Cover { get; set; }
    [ForeignKey("Status")]
    public int? Status { get; set; }
    public virtual ICollection<CategoryModel>? Categories { get; set; } = new List<CategoryModel>();
    public virtual ICollection<TeamMemberModel>? TeamMembers { get; set; } = new List<TeamMemberModel>();
    public virtual ICollection<ComicTranslationsModel>? Translations { get; set; } = new List<ComicTranslationsModel>();
    public int? Available_Storage { get; set; }
    public int? Total_Pages { get; set; }
    public DateTime? Publish_Date { get; set; }
  }

  //__________________ COLOR ______________________________________

  public class ColorModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public ColorModelTranslation Translations { get; set; } = new();
  }

  public class ColorModelTranslation {
    public int Id { get; set; }

    public int ColorId { get; set; }
    public int LanguageId { get; set; }

    [ForeignKey("ColorId")]
    public virtual ColorModel? Color { get; set; } = null!;

    [ForeignKey("LanguageId")]
    public virtual LanguageModel? Language { get; set; } = null!;

    public string Name { get; set; } = null!;
  }

  //__________________ COVER ______________________________________

  public class CoverModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public CoverModelTranslation Translations { get; set; } = new();

  }

  public class CoverModelTranslation {
    public int Id { get; set; }

    public int CoverId { get; set; }
    public int LanguageId { get; set; }

    [ForeignKey("CoverId")]
    public virtual CoverModel? Cover { get; set; } = null!;

    [ForeignKey("LanguageId")]
    public virtual LanguageModel? Language { get; set; } = null!;

    public string Name { get; set; } = null!;
  }
  
  //__________________ STATUS ______________________________________

  public class StatusModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public StatusModelTranslation Translations { get; set; } = new();

  }

  public class StatusModelTranslation {
    public int Id { get; set; }

    public int StatusId { get; set; }
    public int LanguageId { get; set; }

    [ForeignKey("StatusId")]
    public virtual StatusModel? Status { get; set; } = null!;

    [ForeignKey("LanguageId")]
    public virtual LanguageModel? Language { get; set; } = null!;

    public string Name { get; set; } = null!;
  }
}

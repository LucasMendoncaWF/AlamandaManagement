using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlamandaApi.Services.Comics;
using AlamandaApi.Services.CRUD;
using AlamandaApi.Services.Language;

namespace AlamandaApi.Services.Chapters {

  public class ChapterModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
    public DateTime? Publish_Date { get; set; }
    public int? Position { get; set; }

    [ForeignKey("Status")]
    public int? Status { get; set; }
    public virtual StatusModel? ChapterStatus { get; set; }

    public int? Original_Language { get; set; }

    [ForeignKey("Original_Language")]
    public virtual LanguageModel? OriginalLanguage { get; set; }

    [ForeignKey("ComicId")]
    public virtual ComicModel? Comic { get; set; } = null!;
    public int ComicId { get; set; }

    public virtual ICollection<ChapterTranslationModel> Translations { get; set; } = new List<ChapterTranslationModel>();
    public virtual List<PageModel> Pages { get; set; } = new();
  }

  //____________________ TRANSLATION _____________________
  public class ChapterTranslationModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public int? ChapterId { get; set; }
    public int? LanguageId { get; set; }

    [ForeignKey("ChapterId")]
    public virtual ChapterModel? Chapter { get; set; } = null!;
    [ForeignKey("LanguageId")]
    public virtual LanguageModel? Language { get; set; } = null!;
  }

  // _________________ PAGES ________________________________

  public class PageModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
    public int? ChapterId { get; set; }
    public string? Picture { get; set; } = null;
    public int? Position { get; set; }

    [ForeignKey("ChapterId")]
    public virtual ChapterModel? Chapter { get; set; } = null!;
  }

  //____________________ LIST _______________________________
  public class ChaptersPagedResult {
    public List<ChapterModel> Items { get; set; } = [];
    public ComicModel Comic { get; set; } = null!;
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
  }

  public class ChaptersListQueryParams : ListQueryParams {
    public int Id { get; set; } = 0;
  }

  //______________________ FORM __________________________
  public class ChapterFormModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
    public List<string?>? Images { get; set; } = new List<string?>();
    public string? File { get; set; } = null;
    public int ComicId { get; set; }
    public int Original_Language { get; set; }
    public virtual ICollection<ChapterTranslationModel> Translations { get; set; } = new List<ChapterTranslationModel>();
  }

  public class ChapterMoveModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
    public int? Position { get; set; }
  }
}


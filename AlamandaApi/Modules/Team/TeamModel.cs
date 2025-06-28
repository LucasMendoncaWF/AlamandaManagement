using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlamandaApi.Services.Team {
  public class TeamMemberCreationModel {
    [Required(ErrorMessage = "Name é obrigatório")]
    [MaxLength(100, ErrorMessage = "Name pode ter no máximo 100 caracteres")]
    public string Name { get; set; } = null!;

    [MaxLength(50, ErrorMessage = "Social pode ter no máximo 50 caracteres")]
    public string Social { get; set; } = null!;

    public string? Picture { get; set; } = "user";
  }

  public class TeamMemberModel : TeamMemberCreationModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
  }

  public class PagedResult<T> {
    public List<T> Items { get; set; } = [];
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
  }
}

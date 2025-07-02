using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlamandaApi.Services.Art {

  public class ArtCreationModel {

    [MaxLength(50)]
    public string Social { get; set; } = null!;

    public string? Picture { get; set; } = "user";
  }
  
  public class ArtModel : ArtCreationModel {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlamandaApi.Services.Team
{
  public class TeamMemberCreationModel {

    [Required(ErrorMessage = "Name é obrigatório")]
    [MaxLength(100, ErrorMessage = "Name pode ter no máximo 50 caracteres")]
    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("projects")]
    public List<string>? Projects { get; set; } = null!;

    [MaxLength(50, ErrorMessage = "Username pode ter no máximo 50 caracteres")]
    [BsonElement("social")]
    public string Social { get; set; } = null!;

    [BsonElement("picture")]
    [BsonRepresentation(BsonType.String)]
    public string? Picture { get; set; } = "user";
  }
  public class TeamMemberModel : TeamMemberCreationModel
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

  }
}

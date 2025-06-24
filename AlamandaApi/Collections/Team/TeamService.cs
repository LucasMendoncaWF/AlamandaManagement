using MongoDB.Driver;

namespace AlamandaApi.Services.Team
{
  public class TeamService
  {
    private readonly IMongoCollection<TeamMemberModel> _teamMember;
    private readonly string imageUrl;

    public TeamService(MongoDbSettings settings)
    {
      imageUrl = Environment.GetEnvironmentVariable("IMAGES_FOLDER") ?? "";
      var client = new MongoClient(settings.ConnectionString);
      var database = client.GetDatabase(settings.DatabaseName);
      _teamMember = database.GetCollection<TeamMemberModel>("Team");
    }

    public async Task Create(TeamMemberCreationModel member)
    {
      var existingMember = await GetBySocial(member.Social);
      if (existingMember != null)
      {
        throw new Exception("Usuário com esta rede social já cadastrado.");
      }
      var memberParsed = ObjectMapperUtil.CopyWithCapitalization<TeamMemberCreationModel, TeamMemberModel>(member);
      await _teamMember.InsertOneAsync(memberParsed);
      if (member.Picture != null && FieldValidator.IsBase64String(member.Picture))
      {

        var newMember = await _teamMember.Find(u => u.Social == member.Social).FirstOrDefaultAsync();
        if (newMember != null && !string.IsNullOrEmpty(newMember.Id))
        {
          var filter = Builders<TeamMemberModel>.Filter.Eq(m => m.Social, member.Social);
          var pictureDirectory = ImageHandler.SaveImage(member.Picture, newMember.Id, "team");
          var update = Builders<TeamMemberModel>.Update.Set(m => m.Picture, pictureDirectory);
          await _teamMember.UpdateOneAsync(filter, update);
        }
      }
     
    }

    public async Task Update(TeamMemberModel member)
    {
      var existingMember = await _teamMember.Find(u => u.Id == member.Id).FirstOrDefaultAsync();

      if (existingMember == null)
      {
          throw new Exception("Membro não encontrado.");
      }

      if (!string.IsNullOrEmpty(member.Picture) && member.Picture.StartsWith("data:image"))
      {
        if (!string.IsNullOrEmpty(existingMember.Picture) && existingMember.Picture.StartsWith(imageUrl))
        {
          var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingMember.Picture.TrimStart('/'));
          if (File.Exists(oldFilePath))
          {
            File.Delete(oldFilePath);
          }
        }

        member.Picture = ImageHandler.SaveImage(member.Picture, member.Id ?? Guid.NewGuid().ToString(), "team");
      }
      else
      {
        member.Picture = existingMember.Picture;
      }

      await _teamMember.ReplaceOneAsync(u => u.Id == member.Id, member);
    }


    public async Task<List<TeamMemberModel>> GetAll()
    {
      return await _teamMember.Find(_ => true).ToListAsync();
    }

    public async Task<TeamMemberModel> GetBySocial(string Social)
    {
      return await _teamMember.Find(u => u.Social == Social).FirstOrDefaultAsync();
    }

    public async Task<TeamMemberModel> GetById(string Id)
    {
      return await _teamMember.Find(u => u.Id == Id).FirstOrDefaultAsync();
    }


  }
}

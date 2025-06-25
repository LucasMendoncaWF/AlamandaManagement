using AlamandaApi.Data;
using Microsoft.EntityFrameworkCore;

namespace AlamandaApi.Services.Team
{
  public class TeamService
  {
      private readonly AppDbContext _context;
      private readonly string _imageBaseUrl;

      public TeamService(AppDbContext context)
      {
        _context = context;
        _imageBaseUrl = Environment.GetEnvironmentVariable("IMAGES_FOLDER") ?? "";
      }

      public async Task Create(TeamMemberCreationModel member)
      {
        var existingMember = await _context.TeamMembers
          .AsNoTracking()
          .FirstOrDefaultAsync(m => m.Social == member.Social);

        if (existingMember != null)
          throw new Exception("Usuário com esta rede social já cadastrado.");

        var newMember = ObjectMapperUtil.CopyWithCapitalization<TeamMemberCreationModel, TeamMemberModel>(member);

        newMember.Picture = "";

        await _context.TeamMembers.AddAsync(newMember);
        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(member.Picture) && FieldValidator.IsBase64String(member.Picture))
        {
          var savedImagePath = ImageHandler.SaveImage(member.Picture, newMember.Id.ToString(), "team");
          newMember.Picture = savedImagePath;

          _context.TeamMembers.Update(newMember);
          await _context.SaveChangesAsync();
        }
      }

    public async Task Update(TeamMemberModel member)
    {
      var existingMember = await _context.TeamMembers
        .FirstOrDefaultAsync(u => u.Id == member.Id);

      if (existingMember == null)
        throw new Exception("Membro não encontrado.");

      if (!string.IsNullOrEmpty(member.Picture) && member.Picture.StartsWith("data:image"))
      {
        if (!string.IsNullOrEmpty(existingMember.Picture) && existingMember.Picture.StartsWith(_imageBaseUrl))
        {
          var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingMember.Picture.TrimStart('/'));
          if (File.Exists(oldFilePath))
          {
              File.Delete(oldFilePath);
          }
        }
        existingMember.Picture = ImageHandler.SaveImage(member.Picture, member.Id.ToString(), "team");
      }

      existingMember.Name = member.Name;
      existingMember.Social = member.Social;

      _context.TeamMembers.Update(existingMember);
      await _context.SaveChangesAsync();
    }

    public async Task<List<TeamMemberModel>> GetAll()
    {
      return await _context.TeamMembers.AsNoTracking().ToListAsync();
    }

    public async Task<TeamMemberModel?> GetBySocial(string social)
    {
      return await _context.TeamMembers.AsNoTracking()
        .FirstOrDefaultAsync(u => u.Social == social);
    }

    public async Task<TeamMemberModel?> GetById(int id)
    {
      return await _context.TeamMembers.AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == id);
    }
  }
}

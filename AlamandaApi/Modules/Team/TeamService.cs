using AlamandaApi.Data;
using AlamandaApi.Services.Comics;
using Microsoft.EntityFrameworkCore;

namespace AlamandaApi.Services.Team {
  public class TeamService {
    private readonly AppDbContext _context;

    public TeamService(AppDbContext context) {
      _context = context;
    }

    public async Task Create(TeamMemberCreationModel member) {
      var existingMember = await _context.TeamMembers
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Social == member.Social);
      if (existingMember != null) {
        throw new Exception("Usuário com esta rede social já cadastrado.");
      }

      var newMember = ObjectMapperUtil.CopyWithCapitalization<TeamMemberCreationModel, TeamMemberModel>(member);
      newMember.Picture = "";

      if (member.ComicsIds != null && member.ComicsIds.Any()) {
        var relatedComics = await _context.Comics
            .Where(c => member.ComicsIds.Contains(c.Id.ToString()))
            .ToListAsync();

        newMember.Comics = relatedComics;
      }

      await _context.TeamMembers.AddAsync(newMember);
      await _context.SaveChangesAsync();

      if (!string.IsNullOrEmpty(member.Picture) && FieldValidator.IsBase64String(member.Picture)) {
        var savedImagePath = await ImageHandler.SaveImage(member.Picture,
          new ImageHandler.ImageSaveOptions {
            Name = newMember.Id.ToString(),
            Folder = "team",
            Quality = 50,
            MaxWidth = 300,
          }
        );


        newMember.Picture = savedImagePath;
        _context.TeamMembers.Update(newMember);
        await _context.SaveChangesAsync();
      }
    }

    public async Task Update(TeamMemberModel member) {
      var existingMember = await _context.TeamMembers
        .FirstOrDefaultAsync(u => u.Id == member.Id);
      if (existingMember == null) {
        throw new Exception("Membro não encontrado.");
      }
        
      if (!string.IsNullOrEmpty(member.Picture) && member.Picture.StartsWith("data:image")) {
        if (!string.IsNullOrEmpty(existingMember.Picture) && File.Exists(existingMember.Picture)) {
          File.Delete(existingMember.Picture);
        }
        existingMember.Picture = await ImageHandler.SaveImage(member.Picture, 
          new ImageHandler.ImageSaveOptions {
            Name = member.Id.ToString(),
            Folder = "team",
            Quality = 50,
            MaxWidth = 300,
          }
        );
      }

      existingMember.Name = member.Name;
      existingMember.Social = member.Social;
      _context.TeamMembers.Update(existingMember);
      await _context.SaveChangesAsync();
    }
    

    public async Task<PagedResult<TeamMemberModel>> GetAll(int page = 1, int pageSize = 10, string queryString = "") {
      var query = _context.TeamMembers
        .Include(t => t.Comics)
        .AsNoTracking();

      if (!string.IsNullOrWhiteSpace(queryString)) {
        queryString = queryString.Trim().ToLower();
        query = query.Where(m =>
          m.Name.ToLower().Contains(queryString) ||
          m.Social.ToLower().Contains(queryString));
      }

      var totalItems = await query.CountAsync();
      var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

      var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(t => new TeamMemberModel {
          Id = t.Id,
          Name = t.Name,
          Social = t.Social,
          Picture = t.Picture,
          Comics = t.Comics.Select(c => new ComicModel {
            Id = c.Id,
            Name = c.Name
          }).ToList()
        })
        .ToListAsync();

      return new PagedResult<TeamMemberModel> {
        Items = items,
        TotalPages = totalPages,
        CurrentPage = page
      };
    }

    public async Task<TeamMemberModel?> GetBySocial(string social) {
      return await _context.TeamMembers.AsNoTracking()
        .FirstOrDefaultAsync(u => u.Social == social);
    }

    public async Task<TeamMemberModel?> GetById(int id) {
      return await _context.TeamMembers.AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == id);
    }
  }
}

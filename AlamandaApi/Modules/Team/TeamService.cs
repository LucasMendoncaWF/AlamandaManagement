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
      var newMember = ObjectMapperUtil.CopyWithCapitalization<TeamMemberCreationModel, TeamMemberModel>(member);
      newMember.Picture = "";

      if (member.ComicsIds?.Any() == true) {
        var relatedComics = await _context.Comics
          .Where(c => member.ComicsIds.Contains(c.Id.ToString()))
          .ToListAsync();
        newMember.Comics = relatedComics;
      }

      await _context.TeamMembers.AddAsync(newMember);
      await _context.SaveChangesAsync();

      if (!string.IsNullOrEmpty(member.Picture) && FieldValidator.IsBase64String(member.Picture)) {
        var savedImage = await ImageHandler.SaveImage(member.Picture, new ImageHandler.ImageSaveOptions {
          Name = newMember.Id.ToString(),
          Folder = "team",
          Quality = 50,
          MaxWidth = 300,
        });
        newMember.Picture = savedImage;
        _context.TeamMembers.Update(newMember);
        await _context.SaveChangesAsync();
      }
    }

    public async Task Update(TeamMemberEditModel member) {
      var existing = await _context.TeamMembers
        .Include(m => m.Comics)
        .FirstOrDefaultAsync(m => m.Id == member.Id);
      if (existing == null) throw new Exception("Member not found.");

      existing.Picture = await ImageHandler.SaveImage(member.Picture, new ImageHandler.ImageSaveOptions {
        Name = member.Id.ToString(),
        Folder = "team",
        Quality = 50,
        MaxWidth = 300,
        PreviousImage = existing.Picture,
      });

      if (member.ComicsIds?.Any() == true) {
        var relatedComics = await _context.Comics
          .Where(c => member.ComicsIds.Contains(c.Id.ToString()))
          .ToListAsync();
        existing.Comics = relatedComics;
      }

      existing.Name = member.Name;
      existing.Social = member.Social;

      _context.TeamMembers.Update(existing);
      await _context.SaveChangesAsync();
    }

    public async Task<PagedResult<TeamMemberModel>> GetAll(int page = 1, int pageSize = 10, string query = "") {
      var teamQuery = _context.TeamMembers
        .Include(t => t.Comics)
        .Include(t => t.Role)
        .AsNoTracking();

      if (!string.IsNullOrWhiteSpace(query)) {
        var q = query.Trim().ToLower();
        teamQuery = teamQuery.Where(m =>
          m.Name.ToLower().Contains(q) ||
          m.Social.ToLower().Contains(q));
      }

      var totalItems = await teamQuery.CountAsync();
      var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

      var items = await teamQuery
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(t => new TeamMemberModel {
          Id = t.Id,
          Name = t.Name,
          Social = t.Social,
          Picture = t.Picture,
          RoleId = t.RoleId,
          Comics = t.Comics.Select(c => new ComicModel {
            Id = c.Id,
            Name = c.Name
          }).ToList(),
          Role = t.Role == null ? null : new RoleModel {
            Id = t.Role.Id,
            Name = t.Role.Name
          }
        })
        .ToListAsync();

      return new PagedResult<TeamMemberModel> {
        Items = items,
        TotalPages = totalPages,
        CurrentPage = page
      };
    }

    public async Task<TeamMemberModel?> GetBySocial(string social) {
      return await _context.TeamMembers
        .Include(t => t.Comics)
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Social == social);
    }

    public async Task<TeamMemberModel?> GetById(int id) {
      return await _context.TeamMembers
        .Include(t => t.Comics)
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == id);
    }
  }
}

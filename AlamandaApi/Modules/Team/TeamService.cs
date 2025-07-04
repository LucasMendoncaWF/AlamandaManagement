using Microsoft.EntityFrameworkCore;
using AlamandaApi.Services.Comics;
using AlamandaApi.Services.CRUD;
using static AlamandaApi.Data.AppDbContext;

namespace AlamandaApi.Services.Team {
  public class TeamService {
    private readonly CRUDService<TeamMemberModel> _crudService;

    public TeamService(CRUDService<TeamMemberModel> crudService) {
      _crudService = crudService;
    }

    public async Task<TeamMemberModel> Create(TeamMemberCreationModel member) {
      var result = await _crudService.CreateEntityAsync(
        new UpdateEntityOptions<TeamMemberModel, TeamMemberCreationModel> {
          UpdatedObject = member,
          Include = q => q.Include(m => m.Comics),
          PropertiesToUpdate = ["Social", "Name"],
          CustomUpdate = async (existing, updated, tableName, _) => {
            existing.Picture = await ImageHandler.SaveImage(updated.Picture, CreateImage(existing, tableName));
            if (updated.ComicsIds?.Any() == true) {
              var relatedComics = await _crudService.Context.Comics
                .Where(c => updated.ComicsIds.Contains(c.Id.ToString()))
                .ToListAsync();
              existing.Comics = relatedComics;
            }
            return existing;
          }
        }
      );
      return result;
    }

    public async Task<TeamMemberModel> Update(TeamMemberEditModel member) {
      var result = await _crudService.UpdateEntityAsync(
        new UpdateEntityOptions<TeamMemberModel, TeamMemberEditModel> {
          UpdatedObject = member,
          Include = q => q.Include(m => m.Comics),
          PropertiesToUpdate = ["Social", "Name", "RoleId"],
          CustomUpdate = async (existing, updated, tableName, context) => {
            existing.Picture = await ImageHandler.SaveImage(updated.Picture, CreateImage(existing, tableName));
            await _crudService.SyncManyToManyRelation(
              currentCollection: existing.Comics,
              newIds: updated.ComicsIds,
              dbSet: context.Comics,
              idSelector: comic => comic.Id.ToString()
            );
            return existing;
          },
        }
      );
      return result;
    }

    public async Task<PagedResult<TeamMemberModel>> GetAll(ListQueryParams query) {
      return await _crudService.GetPagedAsync(
        query,
        new HashSet<string> { "Id", "Social", "Name"},
        u => new TeamMemberModel {
          Id = u.Id,
          Social = u.Social,
          Name = u.Name,
          Picture = u.Picture,
          RoleId = u.RoleId,
          Comics = u.Comics.Select(c => new ComicModel {
            Id = c.Id,
            Name = c.Name
          }).ToList(),
          Role = u.Role == null ? null : new RoleModel {
            Id = u.Role.Id,
            Name = u.Role.Name
          }
        });
    }

    public async Task<TeamMemberModel?> GetById(int id) {
      return await _crudService.GetByPropertyAsync("Id", id);
    }

    public ImageHandler.ImageSaveOptions CreateImage(TeamMemberModel existing, string folderName) {
      return new ImageHandler.ImageSaveOptions {
        Name = existing.Id.ToString(),
        Folder = folderName,
        Quality = 60,
        MaxWidth = 200,
        PreviousImage = existing.Picture
      };
    }
  }
}

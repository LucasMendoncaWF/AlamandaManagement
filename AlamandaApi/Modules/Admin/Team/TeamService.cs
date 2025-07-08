using Microsoft.EntityFrameworkCore;
using AlamandaApi.Services.Comics;
using AlamandaApi.Services.CRUD;
using static AlamandaApi.Data.AppDbContext;
using AlamandaApi.Services.Role;

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
          Include = q => q.Include(m => m.Comics).Include(m => m.Roles),
          PropertiesToUpdate = ["Social", "Name", "Official_Member"],
          CustomUpdate = async (existing, updated, tableName, _) => {
          var rolesIds = updated.RolesIds?.Select(id => Convert.ToInt32(id)).ToList() ?? new List<int>();
          var comicIds = updated.ComicsIds?.Select(id => Convert.ToInt32(id)).ToList() ?? new List<int>();
            existing.Picture = await ImageHandler.SaveImage(updated.Picture, CreateImage(existing, tableName));
            if (comicIds?.Any() == true) {
              var relatedComics = await _crudService.Context.Comics
                .Where(c => comicIds.Contains(c.Id))
                .ToListAsync();
              existing.Comics = relatedComics;
            }
            if (rolesIds?.Any() == true) {
              var relatedRoles = await _crudService.Context.Roles
                .Where(c => rolesIds.Contains(c.Id))
                .ToListAsync();
              existing.Roles = relatedRoles;
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
          Include = q => q.Include(m => m.Comics).Include(m => m.Roles),
          PropertiesToUpdate = ["Social", "Name", "Official_Member"],
          CustomUpdate = async (existing, updated, tableName, context) => {
          var rolesIds = updated.RolesIds?.Select(id => Convert.ToInt32(id)).ToList() ?? new List<int>();
          var comicIds = updated.ComicsIds?.Select(id => Convert.ToInt32(id)).ToList() ?? new List<int>();
            existing.Picture = await ImageHandler.SaveImage(updated.Picture, CreateImage(existing, tableName));
            await _crudService.SyncManyToManyRelation(
              currentCollection: existing.Comics,
              newIds: comicIds,
              dbSet: context.Comics,
              idSelectorExpr: comic => comic.Id
            );
            await _crudService.SyncManyToManyRelation(
              currentCollection: existing.Roles,
              newIds: rolesIds,
              dbSet: context.Roles,
              idSelectorExpr: role => role.Id
            );
            return existing;
          },
        }
      );
      return result;
    }
    
    public async Task Delete(int Id) {
      await _crudService.DeleteWithImage(Id);
    }

    public async Task<PagedResult<TeamMemberListModel>> GetAll(ListQueryParams query) {
      return await _crudService.GetPagedAsync(new ListOptions<TeamMemberModel, TeamMemberListModel> {
        QueryParams = query,
        AllowedSortColumns = new HashSet<string> { "Social", "Name" },
        Selector = u => new TeamMemberListModel {
          Id = u.Id,
          Social = u.Social,
          Name = u.Name,
          Picture = u.Picture,
          Official_Member = u.Official_Member,
          Comics = u.Comics.Select(c => new ComicListModel {
            Id = c.Id,
            Name = c.Translations
            .Where(t => t.LanguageId == 1)
            .Select(t => t.Name)
            .FirstOrDefault() ?? "",
            Translations = c.Translations,
          }).ToList(),
          Roles = u.Roles.Select(c => new RoleModel {
            Id = c.Id,
            Translations = c.Translations
          }).ToList(),
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

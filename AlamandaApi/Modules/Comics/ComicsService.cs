using AlamandaApi.Services.Category;
using AlamandaApi.Services.CRUD;
using AlamandaApi.Services.Team;
using Microsoft.EntityFrameworkCore;
using static AlamandaApi.Data.AppDbContext;

namespace AlamandaApi.Services.Comics {
  public class ComicsService {
    private readonly CRUDService<ComicModel> _crudService;

    public ComicsService(CRUDService<ComicModel> crudService) {
      _crudService = crudService;
    }

    public async Task<ComicModel> Create(ComicCreationModel comic) {
      var result = await _crudService.CreateEntityAsync(
        new UpdateEntityOptions<ComicModel, ComicCreationModel> {
          UpdatedObject = comic,
          Include = q => q.Include(m => m.Categories),
          PropertiesToUpdate = ["Name", "Name_En"],
          CustomUpdate = async (existing, updated, tableName, context) => {
            existing.Picture = await ImageHandler.SaveImage(updated.Picture, CreateImage(existing, tableName));
            var categoriesIds = updated.CategoriesIds?.Select(id => Convert.ToInt32(id)).ToList() ?? new List<int>();
            if (categoriesIds?.Any() == true) {
              var relatedComics = await _crudService.Context.Categories
                .Where(c => categoriesIds.Contains(c.Id))
                .ToListAsync();
              existing.Categories = relatedComics;
            }
            return existing;
          },
        }
      );
      return result;
    }

    public async Task<ComicModel> Update(ComicEditModel comic) {
      var result = await _crudService.UpdateEntityAsync(
        new UpdateEntityOptions<ComicModel, ComicEditModel> {
          UpdatedObject = comic,
          Include = q => q.Include(m => m.Categories),
          PropertiesToUpdate = ["Name", "Name_En"],
          CustomUpdate = async (existing, updated, tableName, context) => {
            existing.Picture = await ImageHandler.SaveImage(updated.Picture, CreateImage(existing, tableName));
            var categoriesIds = updated.CategoriesIds?.Select(id => Convert.ToInt32(id)).ToList() ?? new List<int>();
            await _crudService.SyncManyToManyRelation(
              currentCollection: existing.Categories,
              newIds: categoriesIds,
              dbSet: context.Categories,
              idSelectorExpr: category => category.Id
            );
            return existing;
          },
        }
      );
      return result;
    }
    
    public async Task Delete(int Id) {
      await _crudService.DeleteByIdAsync(Id);
    }

    public async Task<PagedResult<ComicModel>> GetAll(ListQueryParams query) {
      return await _crudService.GetPagedAsync(new ListOptions<ComicModel, ComicModel> {
        QueryParams = query,
        AllowedSortColumns = new HashSet<string> { "Name", "Name_En" },
        Selector = u => new ComicModel {
          Id = u.Id,
          Name = u.Name,
          Name_En = u.Name_En,
          Picture = u.Picture,
          Categories = u.Categories.Select(c => new CategoryModel {
            Id = c.Id,
            Translations = c.Translations
          }).ToList(),
          TeamMembers = u.TeamMembers.Select(c => new TeamMemberModel {
            Id = c.Id,
            Name = c.Name,
          }).ToList(),
        }
      });
    }

    public async Task<ComicModel?> GetById(int id) {
      return await _crudService.GetByPropertyAsync("Id", id);
    }

    public ImageHandler.ImageSaveOptions CreateImage(ComicModel existing, string folderName) {
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

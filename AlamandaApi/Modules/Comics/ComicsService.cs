using AlamandaApi.Services.CRUD;
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
          PropertiesToUpdate = ["Name"],
          CustomUpdate = async (existing, updated, tableName, _) => {
            existing.Picture = await ImageHandler.SaveImage(updated.Picture, CreateImage(existing, tableName));
            return existing;
          }
        }
      );
      return result;
    }

    public async Task<ComicModel> Update(ComicModel comic) {
      var result = await _crudService.UpdateEntityAsync(
        new UpdateEntityOptions<ComicModel, ComicModel> {
          UpdatedObject = comic,
          PropertiesToUpdate = ["Name"],
          CustomUpdate = async (existing, updated, tableName, context) => {
            existing.Picture = await ImageHandler.SaveImage(updated.Picture, CreateImage(existing, tableName));
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
        AllowedSortColumns = new HashSet<string> { "Name" },
        Selector = u => new ComicModel {
          Id = u.Id,
          Name = u.Name,
          Picture = u.Picture,
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

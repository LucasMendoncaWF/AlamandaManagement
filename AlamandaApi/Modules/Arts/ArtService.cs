using AlamandaApi.Services.CRUD;
using static AlamandaApi.Data.AppDbContext;

namespace AlamandaApi.Services.Art {
  public class ArtService {
    private readonly CRUDService<ArtModel> _crudService;

    public ArtService(CRUDService<ArtModel> crudService) {
      _crudService = crudService;
    }

    public async Task<ArtModel> Create(ArtCreationModel art) {
      var result = await _crudService.CreateEntityAsync(
        new UpdateEntityOptions<ArtModel, ArtCreationModel> {
          PropertiesToUpdate = ["Social"],
          UpdatedObject = art,
          CustomUpdate = async (existing, updated, tableName, _) => {
            existing.Picture = await ImageHandler.SaveImage(updated.Picture, CreateImage(existing, tableName));
            return existing;
          }
        }
      );
      return result;
    }

    public async Task<ArtModel> Update(ArtModel art) {
      var result = await _crudService.UpdateEntityAsync(
        new UpdateEntityOptions<ArtModel, ArtModel> {
          PropertiesToUpdate = ["Social"],
          UpdatedObject = art,
          CustomUpdate = async (existing, updated, tableName, _) => {
            existing.Picture = await ImageHandler.SaveImage(updated.Picture, CreateImage(existing, tableName));
            return existing;
          }
        }
      );
      return result;
    }

    public async Task Delete(int Id) {
      await _crudService.DeleteByIdAsync(Id);
    }

    public async Task<PagedResult<ArtModel>> GetAll(ListQueryParams query) {
      return await _crudService.GetPagedAsync(new ListOptions<ArtModel, ArtModel> {
        QueryParams = query,
        AllowedSortColumns = new HashSet<string> { "Id", "Social" },
        Selector = u => new ArtModel {
          Id = u.Id,
          Social = u.Social,
          Picture = u.Picture
        }
      });
    }

    public async Task<ArtModel?> GetById(int id) {
      return await _crudService.GetByPropertyAsync("Id", id);
    }
    
    public ImageHandler.ImageSaveOptions CreateImage(ArtModel existing, string tableName) {
      return new ImageHandler.ImageSaveOptions {
        Name = existing.Id.ToString(),
        Folder = tableName,
        Quality = 60,
        MaxWidth = 600,
        PreviousImage = existing.Picture
      };
    }
  }
}

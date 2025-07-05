using AlamandaApi.Services.CRUD;
using static AlamandaApi.Data.AppDbContext;

namespace AlamandaApi.Services.Category {
  public class CategoryService {
    private readonly CRUDService<CategoryModel> _crudService;

    public CategoryService(CRUDService<CategoryModel> crudService) {
      _crudService = crudService;
    }

    public async Task<CategoryModel> Create(CategoryCreationModel category) {
      var result = await _crudService.CreateEntityAsync(
        new UpdateEntityOptions<CategoryModel, CategoryCreationModel> {
          PropertiesToUpdate = ["Name", "Name_En"],
          UpdatedObject = category,
        }
      );
      return result;
    }

    public async Task<CategoryModel> Update(CategoryModel category) {
      var result = await _crudService.UpdateEntityAsync(
        new UpdateEntityOptions<CategoryModel, CategoryModel> {
          PropertiesToUpdate = ["Name", "Name_En"],
          UpdatedObject = category,
        }
      );
      return result;
    }

    public async Task Delete(int Id) {
      await _crudService.DeleteByIdAsync(Id);
    }

    public async Task<PagedResult<CategoryModelDTO>> GetAll(ListQueryParams query) {
      return await _crudService.GetPagedAsync(new ListOptions<CategoryModel, CategoryModelDTO> {
        QueryParams = query,
        AllowedSortColumns = new HashSet<string> { "Name", "Name_En" },
        Selector = u => new CategoryModelDTO {
          Id = u.Id,
          Name = u.Name,
          Name_En = u.Name_En!
        }
      });
    }

    public async Task<CategoryModel?> GetById(int id) {
      return await _crudService.GetByPropertyAsync("Id", id);
    }
  }
}

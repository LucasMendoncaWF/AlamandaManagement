using AlamandaApi.Services.CRUD;
using static AlamandaApi.Data.AppDbContext;

namespace AlamandaApi.Services.Role {
  public class RoleService {
    private readonly CRUDService<RoleModel> _crudService;

    public RoleService(CRUDService<RoleModel> crudService) {
      _crudService = crudService;
    }

    public async Task<RoleModel> Create(RoleCreationModel role) {
      var result = await _crudService.CreateEntityAsync(
        new UpdateEntityOptions<RoleModel, RoleCreationModel> {
          PropertiesToUpdate = ["Name"],
          UpdatedObject = role,
        }
      );
      return result;
    }

    public async Task<RoleModel> Update(RoleModel role) {
      var result = await _crudService.UpdateEntityAsync(
        new UpdateEntityOptions<RoleModel, RoleModel> {
          PropertiesToUpdate = ["Name"],
          UpdatedObject = role,
        }
      );
      return result;
    }

    public async Task Delete(int Id) {
      await _crudService.DeleteByIdAsync(Id);
    }

    public async Task<PagedResult<RoleModelDto>> GetAll(ListQueryParams query) {
      return await _crudService.GetPagedAsync(new ListOptions<RoleModel, RoleModelDto> {
        QueryParams = query,
        AllowedSortColumns = new HashSet<string> { "Name" },
        Selector = u => new RoleModelDto {
          Id = u.Id,
          Name = u.Name,
        }
      });
    }

    public async Task<RoleModel?> GetById(int id) {
      return await _crudService.GetByPropertyAsync("Id", id);
    }
  }
}

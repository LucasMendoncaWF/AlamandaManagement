using AlamandaApi.Services.CRUD;
using static AlamandaApi.Data.AppDbContext;

namespace AlamandaApi.Services.User {
  public class UserService {
    private readonly CRUDService<UserModel> _crudService;

    public UserService(CRUDService<UserModel> crudService) {
      _crudService = crudService;
    }

    public async Task<UserModel> AdminUpdateUser(UserEdit user) {
      var result = await _crudService.UpdateEntityAsync(
        new UpdateEntityOptions<UserModel, UserEdit> {
          PropertiesToUpdate = ["Email", "UserName", "PermissionId"],
          UpdatedObject = user,
          CustomUpdate = async (existing, updated, tableName, _) => {
            existing.Picture = await ImageHandler.SaveImage(updated.Picture, CreateImage(existing, tableName));
            return existing;
          }
        }
      );
      return result;
    }

    public async Task<UserModel> AdminCreateUser(UserCreate user) {
      var result = await _crudService.CreateEntityAsync(
        new UpdateEntityOptions<UserModel, UserCreate> {
          PropertiesToUpdate = ["Email", "UserName", "PermissionId"],
          UpdatedObject = user,
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

    public async Task<PagedResult<UserListDto>> GetAll(ListQueryParams query) {
      return await _crudService.GetPagedAsync(new ListOptions<UserModel, UserListDto> {
        QueryParams = query,
        AllowedSortColumns = new HashSet<string> { "UserName", "Email", "PermissionId" },
        Selector = u => new UserListDto {
          Id = u.Id,
          UserName = u.UserName,
          Email = u.Email,
          Picture = u.Picture,
          PermissionId = u.PermissionId,
          Permission = u.Permission == null ? null : new PermissionModel {
            Id = u.Permission.Id,
            Name = u.Permission.Name
          }
        },
      });
    }

    public async Task<UserModel?> GetById(int id) {
      return await _crudService.GetByPropertyAsync("Id", id);
    }
    
    public ImageHandler.ImageSaveOptions CreateImage(UserModel existing, string tableName) {
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
using AlamandaApi.Services.CRUD;
using Microsoft.EntityFrameworkCore;
using static AlamandaApi.Data.AppDbContext;

namespace AlamandaApi.Services.Role {
  public class RoleService {
    private readonly CRUDService<RoleModel> _crudService;

    public RoleService(CRUDService<RoleModel> crudService) {
      _crudService = crudService;
    }

    public async Task<RoleCreationModel> Create(RoleCreationModel role) {
      var result = await _crudService.CreateEntityAsync(
        new UpdateEntityOptions<RoleModel, RoleCreationModel> {
          AfterAll = async (entity, updated, tableName, context) => {
          context.Entry(entity).Collection(e => e.Translations).Load();
          var translationsDict = updated.Translations ?? new List<RoleTranslationModel>();
          foreach (var lang in translationsDict) {
            var langId = lang.LanguageId;
            var translation = new RoleTranslationModel {
              RoleId = entity.Id,
              LanguageId = langId,
              Name = lang.Name
            };
            context.Set<RoleTranslationModel>().Add(translation);
          }
          await context.SaveChangesAsync();
          return entity;
        },
          UpdatedObject = role,
        }
      );
      return result;
    }

    public async Task<RoleModel> Update(RoleModel role) {
      var result = await _crudService.UpdateEntityAsync(
        new UpdateEntityOptions<RoleModel, RoleModel> {
          UpdatedObject = role,
          CustomUpdate = async (entity, updated, tableName, context) => {
            context.Entry(entity).Collection(e => e.Translations).Load();
            var translationsDict = updated.Translations ?? new List<RoleTranslationModel>();

            foreach (var currTranslation in translationsDict) {
              var langId = currTranslation.LanguageId;
              var existingTranslation = entity.Translations
                  .FirstOrDefault(t => t.LanguageId == langId);
              if (existingTranslation != null) {
                existingTranslation.Name = currTranslation.Name;
              }
              else {
                var newTranslation = new RoleTranslationModel {
                  RoleId = entity.Id,
                  LanguageId = langId,
                  Name = currTranslation.Name
                };
                context.Add(newTranslation);
              }
            }
            await context.SaveChangesAsync();
            return entity;
          },
        }
      );
      return result;
    }

    public async Task Delete(int Id) {
      await _crudService.DeleteByIdAsync(Id);
    }

    public async Task<PagedResult<RoleModelView>> GetAll(ListQueryParams query) {
      return await _crudService.GetPagedAsync(new ListOptions<RoleModel, RoleModelView> {
        QueryParams = query,
        IgnoreFields = ["teammembers"],
        Include = q => q.Include(c => c.Translations).ThenInclude(t => t.Language),
        Selector = u => new RoleModelView {
          Id = u.Id,
          Name = u.Translations
            .Where(t => t.LanguageId == 1)
            .Select(t => t.Name)
            .FirstOrDefault() ?? "",
          Translations = u.Translations.Select(r => new RoleTranslationModel {
            Id = r.Id,
            Name = r.Name,
            LanguageId = r.LanguageId,
            RoleId = r.RoleId
          }).ToList(),
        }
      });
    }

    public async Task<RoleModel?> GetById(int id) {
      return await _crudService.GetByPropertyAsync("Id", id);
    }
  }
}

using AlamandaApi.Services.CRUD;
using Microsoft.EntityFrameworkCore;
using static AlamandaApi.Data.AppDbContext;

namespace AlamandaApi.Services.Category {

  public class CategoryService {
    private readonly CRUDService<CategoryModel> _crudService;

    public CategoryService(CRUDService<CategoryModel> crudService) {
      _crudService = crudService;
    }

    public async Task<CategoryModel> Create(CategoryModelView category) {
      var result = await _crudService.CreateEntityAsync(new UpdateEntityOptions<CategoryModel, CategoryModelView> {
        UpdatedObject = category,
        AfterAll = async (entity, updated, tableName, context) => {
          context.Entry(entity).Collection(e => e.Translations).Load();
          var translationsDict = updated.Translations ?? new List<CategoryTranslationModel>();
          foreach (var lang in translationsDict) {
            var langId = lang.LanguageId;
            var translation = new CategoryTranslationModel {
              CategoryId = entity.Id,
              LanguageId = langId,
              Name = lang.Name
            };
            context.Set<CategoryTranslationModel>().Add(translation);
          }
          await context.SaveChangesAsync();
          return entity;
        },
      });

      return result;
    }

    public async Task<CategoryModel> Update(CategoryModel category) {
      var result = await _crudService.UpdateEntityAsync(
        new UpdateEntityOptions<CategoryModel, CategoryModel> {
          UpdatedObject = category,
          CustomUpdate = async (entity, updated, tableName, context) => {
            context.Entry(entity).Collection(e => e.Translations).Load();
            var translationsDict = updated.Translations ?? new List<CategoryTranslationModel>();
            foreach (var currTranslation in translationsDict) {
              var langId = currTranslation.LanguageId;
              var existingTranslation = entity.Translations
                  .FirstOrDefault(t => t.LanguageId == langId);
              if (existingTranslation != null) {
                existingTranslation.Name = currTranslation.Name;
              }
              else {
                var newTranslation = new CategoryTranslationModel {
                  CategoryId = entity.Id,
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

    public async Task<PagedResult<CategoryModelView>> GetAll(ListQueryParams query) {
      return await _crudService.GetPagedAsync(new ListOptions<CategoryModel, CategoryModelView> {
        QueryParams = query,
        Include = q => q.Include(c => c.Translations).ThenInclude(t => t.Language),
        Selector = u => new CategoryModelView {
          Id = u.Id,
          Name = u.Translations
            .Where(t => t.LanguageId == 1)
            .Select(t => t.Name)
            .FirstOrDefault() ?? "",
          Translations = u.Translations.Select(r => new CategoryTranslationModel {
            Id = r.Id,
            Name = r.Name,
            LanguageId = r.LanguageId,
            CategoryId = r.CategoryId
          }).ToList(),
        }
      });
    }

  }
}

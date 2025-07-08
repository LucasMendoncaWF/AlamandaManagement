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

    public async Task<ComicModel> Create(ComicModel comic) {
      var result = await _crudService.CreateEntityAsync(
        new UpdateEntityOptions<ComicModel, ComicModel> {
          UpdatedObject = comic,
          PropertiesToUpdate = ["Color", "Cover", "Status", "Total_Pages", "Available_Storage", "Publish_Date"],
          Include = q => q.Include(m => m.Categories),
          CustomUpdate = async (entity, updated, tableName, context) => {
            var categoriesIds = updated.CategoriesIds?.Select(id => Convert.ToInt32(id)).ToList() ?? new List<int>();
            if (categoriesIds?.Any() == true) {
              var relatedComics = await _crudService.Context.Categories
                .Where(c => categoriesIds.Contains(c.Id))
                .ToListAsync();
              entity.Categories = relatedComics;
            }
            return entity;
          },
          AfterAll = async (entity, updated, tableName, context) => {
            context.Entry(entity).Collection(e => e.Translations).Load();
            var translationsDict = updated.Translations ?? new List<ComicTranslationsModel>();
            foreach (var lang in translationsDict) {
              List<string?>? pictures = null;
              pictures = await ImageHandler.SaveImages(
                createImageType(lang, tableName),
                lang.Pictures
              );
              var langId = lang.LanguageId;
              var translation = new ComicTranslationsModel {
                ComicId = entity.Id,
                LanguageId = langId,
                Name = lang.Name,
                Amazon = lang.Amazon,
                Catarse = lang.Catarse,
                Description = lang.Description,
                Discount = lang.Discount ?? 0,
                Price = lang.Price ?? 0,
                Pictures = pictures,
              };
              context.Set<ComicTranslationsModel>().Add(translation);
            }
            await context.SaveChangesAsync();
            return entity;
          },
        }
      );
      return result;
    }

    public async Task<ComicModel> Update(ComicModel comic) {
      var result = await _crudService.UpdateEntityAsync(
        new UpdateEntityOptions<ComicModel, ComicModel> {
          UpdatedObject = comic,
          PropertiesToUpdate = ["Color", "Cover", "Status", "Total_Pages", "Available_Storage", "Publish_Date"],
          Include = q => q.Include(m => m.Categories),
          CustomUpdate = async (existing, updated, tableName, context) => {
            var categoriesIds = updated.CategoriesIds?.Select(id => Convert.ToInt32(id)).ToList() ?? new List<int>();

            await _crudService.SyncManyToManyRelation(
              currentCollection: existing.Categories,
              newIds: categoriesIds,
              dbSet: context.Categories,
              idSelectorExpr: category => category.Id
            );
            context.Entry(existing).Collection(e => e.Translations).Load();
            var translationsDict = updated.Translations ?? new List<ComicTranslationsModel>();
            foreach (var lang in translationsDict) {
              if (lang == null) { continue; }
              var langId = lang.LanguageId;
              var existingTranslation = existing.Translations
                  .FirstOrDefault(t => t.LanguageId == langId);
              List<string?>? pictures = null;
                pictures = await ImageHandler.SaveImages(
                  createImageType(lang, tableName),
                  lang.Pictures,
                  existingTranslation?.Pictures
                );
              var newTranslation = new ComicTranslationsModel {
                ComicId = existing.Id,
                LanguageId = langId,
                Name = lang.Name,
                Amazon = lang.Amazon,
                Catarse = lang.Catarse,
                Description = lang.Description,
                Discount = lang.Discount ?? 0,
                Price = lang.Price ?? 0,
                Pictures = pictures,
              };

              if (existingTranslation == null) {
                context.Add(newTranslation);
              }
              else {
                existingTranslation.Name = newTranslation.Name;
                existingTranslation.Amazon = newTranslation.Amazon;
                existingTranslation.Catarse = newTranslation.Catarse;
                existingTranslation.Description = newTranslation.Description;
                existingTranslation.Discount = newTranslation.Discount ?? 0;
                existingTranslation.Price = newTranslation.Price ?? 0;
                existingTranslation.Pictures = newTranslation.Pictures ?? null;
              }
            }

            await context.SaveChangesAsync();
            return existing;
          },
        }
      );
      return result;
    }

    public async Task Delete(int id) {
      await _crudService.DeleteWithMultipleImages(id);
    }

    public async Task<PagedResult<ComicListModel>> GetAll(ListQueryParams query) {
      return await _crudService.GetPagedAsync(new ListOptions<ComicModel, ComicListModel> {
        QueryParams = query,
        IgnoreFields = ["teammembers", "cart", "users"],
        Selector = u => new ComicListModel {
          Id = u.Id,
          Available_Storage = u.Available_Storage,
          Color = u.Color,
          Cover = u.Cover,
          Publish_Date = u.Publish_Date,
          Total_Pages = u.Total_Pages,
          Status = u.Status,
          Name = u.Translations
            .Where(t => t.LanguageId == 1)
            .Select(t => t.Name)
            .FirstOrDefault() ?? "",
          Categories = u.Categories.Select(c => new CategoryModel {
            Id = c.Id,
            Translations = c.Translations
          }).ToList(),
          TeamMembers = u.TeamMembers.Select(c => new TeamMemberModel {
            Id = c.Id,
            Name = c.Name,
          }).ToList(),
          Translations = u.Translations.Select(r => new ComicTranslationsModel {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            Amazon = r.Amazon,
            Catarse = r.Catarse,
            Discount = r.Discount,
            Price = r.Price,
            LanguageId = r.LanguageId,
            ComicId = r.ComicId,
            Pictures = r.Pictures,
          }).ToList(),
        }
      });
    }

    public async Task<ComicModel?> GetById(int id) {
      return await _crudService.GetByPropertyAsync("Id", id);
    }

    private ImageHandler.ImageSaveOptions createImageType(ComicTranslationsModel updated, string tableName) {
      return new ImageHandler.ImageSaveOptions {
        Name = updated.LanguageId.ToString() ?? "0",
        Folder = tableName,
        Quality = 60,
        MaxWidth = 200,
        MaxKb = 2000,
      };
    }
  }
}

using System.Security.Claims;
using AlamandaApi.Data;
using AlamandaApi.Services.Category;
using AlamandaApi.Services.CRUD;
using AlamandaApi.Services.Team;
using Microsoft.EntityFrameworkCore;
using static AlamandaApi.Data.AppDbContext;

namespace AlamandaApi.Services.Comics {
  public class ComicsService {
    private readonly CRUDService<ComicModel> _crudService;
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private int maxFileSize = 2000;
    public ComicsService(
      CRUDService<ComicModel> crudService,
      IHttpContextAccessor httpContextAccessor,
      AppDbContext context
    ) {
      _context = context;
      _crudService = crudService;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ComicModel> Create(ComicModel comic, bool? isFromAdmin = false) {
      var httpContext = _httpContextAccessor.HttpContext;
      if (httpContext == null) throw new UnauthorizedAccessException("Missing context.");
      var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0";

      var result = await _crudService.CreateEntityAsync(
        new UpdateEntityOptions<ComicModel, ComicModel> {
          UpdatedObject = comic,
          PropertiesToUpdate = GetFieldsToUpdate(isFromAdmin),
          Include = q => q.Include(m => m.Categories),
          CustomUpdate = async (entity, updated, tableName, context) => {
            entity.OwnerId = int.Parse(userIdStr);
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
                createImageType(lang, entity, tableName),
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

    public async Task<ComicModel> Update(ComicModel comic, bool? isFromAdmin = false) {
      await EnsureUserOwnsComic(comic.Id);
      var result = await _crudService.UpdateEntityAsync(
        new UpdateEntityOptions<ComicModel, ComicModel> {
          UpdatedObject = comic,
          PropertiesToUpdate = GetFieldsToUpdate(isFromAdmin),
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
                  createImageType(lang, updated, tableName),
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

    private string[] GetFieldsToUpdate(bool? isFromAdmin) {
      var fieldsToUpdate = new List<string> { "Color", "Total_Pages" };
      if (isFromAdmin == true) {
        fieldsToUpdate.AddRange([ "Status", "Publish_Date", "Available_Storage", "Cover" ]);
      }
      return fieldsToUpdate.ToArray();
    }

    public async Task Delete(int id) {
      await EnsureUserOwnsComic(id);
      await _crudService.DeleteWithMultipleTranslationImages(id, true);
    }

    public async Task<ComicModel?> GetById(int id, int languageId) {
      var comic = await _context.Comics
        .Include(c => c.ColorModel)
        .Include(c => c.CoverModel)
        .Include(c => c.StatusModel)
        .Include(c => c.Categories)
          .ThenInclude(cat => cat.Translations)
        .Include(c => c.Translations)
        .FirstOrDefaultAsync(c => c.Id == id);

      if (comic == null) return null;
      comic.Translations = comic.Translations
        .Where(t => t.LanguageId == languageId)
        .ToList();

      if (comic.ColorModel?.Translations?.LanguageId != languageId)
        comic.ColorModel = null;

      if (comic.CoverModel?.Translations?.LanguageId != languageId)
        comic.CoverModel = null;

      if (comic.StatusModel != null)
        comic.StatusModel.Translations = comic.StatusModel.Translations
          .Where(t => t.LanguageId == languageId)
          .ToList();

      foreach (var cat in comic.Categories) {
        cat.Translations = cat.Translations
          .Where(t => t.LanguageId == languageId)
          .ToList();
      }
      return comic;
    }

    public async Task<PagedResult<ComicListModel>> GetAll(ListQueryParams query) {
      return await _crudService.GetPagedAsync(new ListOptions<ComicModel, ComicListModel> {
        QueryParams = query,
        MaxFileSize = maxFileSize,
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

    public async Task EnsureUserOwnsComic(int comicId) {
      var httpContext = _httpContextAccessor.HttpContext;
      if (httpContext == null) throw new UnauthorizedAccessException("Missing context.");

      var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
      var permissionIdStr = httpContext.User.FindFirst("PermissionId")?.Value;

      if (!int.TryParse(userIdStr, out var userId))
        throw new UnauthorizedAccessException("Invalid user ID");

      var isAdmin = permissionIdStr == "1";

      if (isAdmin) return;

      var isOwner = await _crudService.Context.Comics
        .AsNoTracking()
        .AnyAsync(c => c.Id == comicId && c.OwnerId == userId);

      if (!isOwner)
        throw new UnauthorizedAccessException("You don't have permission to modify this comic");
    }

    private ImageHandler.ImageSaveOptions createImageType(ComicTranslationsModel updated, ComicModel parent, string tableName) {
      return new ImageHandler.ImageSaveOptions {
        Name = updated.LanguageId.ToString() ?? "0",
        Folder = $"{tableName}/{parent.Id}",
        Quality = 60,
        MaxWidth = 200,
        MaxKb = maxFileSize,
      };
    }
  }
}

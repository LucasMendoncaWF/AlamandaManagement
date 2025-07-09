using System.Security.Claims;
using AlamandaApi.Data;
using AlamandaApi.Services.Comics;
using AlamandaApi.Services.FieldsSchema;
using Microsoft.EntityFrameworkCore;

namespace AlamandaApi.Services.Chapters {
  public class ChaptersService {

    private readonly AppDbContext _context;
    private readonly FieldsSchemaService _fieldsService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ComicsService _comicsService;
    private readonly int maxKb = 5000;
    public ChaptersService(
      AppDbContext context,
      FieldsSchemaService fieldsService,
      IHttpContextAccessor httpContextAccessor,
      ComicsService comicsService
    ) {
      _context = context;
      _fieldsService = fieldsService;
      _httpContextAccessor = httpContextAccessor;
      _comicsService = comicsService;
    }

    public async Task<ChapterModel> Create(ChapterFormModel chapter) {
      await _comicsService.EnsureUserOwnsComic(chapter.ComicId);
      ChapterModel newChapter = new ChapterModel();
      newChapter.Translations = chapter.Translations;
      newChapter.Original_Language = chapter.Original_Language;
      await _context.Chapters.AddAsync(newChapter);
      await _context.SaveChangesAsync();

      var comic = await _context.Comics.FindAsync(newChapter.ComicId);
      if (comic == null) throw new Exception("Comic not found");
      List<string?>? savedImages = new List<string?>();
      if (chapter.Images != null) {
        savedImages = await ImageHandler.SaveImages(GetImagesConfiguration(newChapter), chapter.Images);
      } else if(chapter.File != null) {
        savedImages = new List<string?>();
      }
      if (savedImages != null) {
          for (int i = 0; i < savedImages.Count; i++) {
            newChapter.Pages.Add(new PageModel {
              ChapterId = chapter.Id,
              Position = i,
              Picture = savedImages[i]
            });
          }
          await _context.SaveChangesAsync();
        }

      return newChapter;
    }

    public async Task<ChapterModel> Update(ChapterFormModel chapter) {
      var existing = await _context.Chapters
        .Include(c => c.Pages)
        .Include(c => c.Translations)
        .FirstOrDefaultAsync(c => c.Id == chapter.Id);

      if (existing == null) throw new Exception("Chapter not found");
      await EnsureUserOwnsChapter(existing.Id!.Value);

      existing.Original_Language = chapter.Original_Language;
      existing.Translations = chapter.Translations;

      var folderPath = $"Comics/{chapter.ComicId}/{chapter.Id}";
      var previousImages = existing.Pages.Select(p => p.Picture).ToList();
      List<string?>? newSavedImages = new List<string?>();
      if (chapter.Images != null) {
        newSavedImages = await ImageHandler.SaveImages(GetImagesConfiguration(existing), chapter.Images, previousImages);
      } else if(chapter.File != null) {
        newSavedImages = new List<string?>();
      }
      if (newSavedImages != null) {
        var removedPages = existing.Pages.Where(p => !newSavedImages.Contains(p.Picture)).ToList();
        foreach (var page in removedPages) {
          if (!string.IsNullOrEmpty(page.Picture)) {
            ImageHandler.DeleteImage(folderPath, page.Picture);
          }
        }
        existing.Pages.RemoveAll(removedPages.Contains);

        for (int i = 0; i < newSavedImages.Count; i++) {
          var existingPage = existing.Pages.FirstOrDefault(p => p.Picture == newSavedImages[i]);
          if (existingPage == null) {
            existing.Pages.Add(new PageModel {
              ChapterId = chapter.Id,
              Position = i,
              Picture = newSavedImages[i]
            });
          }
          else {
            existingPage.Position = i;
          }
        }
      }

      await _context.SaveChangesAsync();
      return existing;
    }

    public async Task<ChapterMoveModel> Move(List<ChapterMoveModel> chapters) {
      foreach (var chapter in chapters) {
        var existing = await _context.Chapters.FindAsync(chapter.Id);
        if (existing != null) {
          await EnsureUserOwnsChapter(existing.Id!.Value);
          existing.Position = chapter.Position;
        }
      }
      await _context.SaveChangesAsync();
      return chapters.First();
    }

    public async Task Delete(int id) {
      await EnsureUserOwnsChapter(id);
      var chapter = await _context.Chapters
        .Include(c => c.Pages)
        .FirstOrDefaultAsync(c => c.Id == id);

      if (chapter == null) return;

      var folderPath = $"Comics/{chapter.ComicId}/{chapter.Id}";
      foreach (var page in chapter.Pages) {
        if (!string.IsNullOrEmpty(page.Picture)) {
          ImageHandler.DeleteImage(folderPath, page.Picture);
        }
      }

      _context.Chapters.Remove(chapter);
      await _context.SaveChangesAsync();
    }

    public async Task<ChaptersPagedResult?> GetAllFromComic(ChaptersListQueryParams queryParams) {
      string id = _httpContextAccessor.HttpContext?.Request.Headers["languageId"].ToString() ?? "1";
      if (!int.TryParse(id, out int languageId)) {
          languageId = 1;
      }
      int page = queryParams.Page ?? 1;
      int pageSize = queryParams.PageSize ?? 10;
      var query = _context.Chapters
        .Include(ch => ch.Translations)
        .Where(ch => ch.ComicId == queryParams.Id)
        .OrderBy(ch => ch.Position);
      var comic = await _comicsService.GetById(queryParams.Id, languageId);
      if (comic == null) {
        return null;
      }
      var total = await query.CountAsync();
      var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

      return new ChaptersPagedResult {
        Items = items,
        TotalPages = total,
        CurrentPage = page,
        Comic = comic!,
      };
    }

    public async Task<ChapterModel?> GetById(int id) {
      return await _context.Chapters
        .Include(c => c.Translations)
        .Include(c => c.Pages)
        .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task EnsureUserOwnsChapter(int chapterId) {
      var httpContext = _httpContextAccessor.HttpContext;
      if (httpContext == null) throw new UnauthorizedAccessException("Missing context.");

      var userIdStr = httpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
      var permissionIdStr = httpContext.User?.FindFirst("PermissionId")?.Value;

      if (!int.TryParse(userIdStr, out var userId))
        throw new UnauthorizedAccessException("Invalid user ID");

      var isAdmin = permissionIdStr == "1";
      if (isAdmin) return;

      var chapter = await _context.Chapters
        .AsNoTracking()
        .Where(c => c.Id == chapterId)
        .Select(c => new { c.ComicId })
        .FirstOrDefaultAsync();

      if (chapter == null) {
        throw new Exception("Chapter not found.");
      }

      var isOwner = await _context.Comics
        .AsNoTracking()
        .AnyAsync(c => c.Id == chapter.ComicId && c.OwnerId == userId);

      if (!isOwner)
        throw new UnauthorizedAccessException("You don't have permission to edit this.");
    }

    public async Task<List<FieldInfo>?> GetFields() {
      return await _fieldsService.GetFieldTypes("Chapters", maxKb);
    }

    private ImageHandler.ImageSaveOptions GetImagesConfiguration(ChapterModel chapter) {
      var folderPath = $"Comics/{chapter.ComicId}/{chapter.Id}";
      return new ImageHandler.ImageSaveOptions {
        Folder = folderPath,
        MaxKb = maxKb,
        MaxWidth = 1200,
        Quality = 70,
        Name = "page"
      };
    }
  }
}

using AlamandaApi.Data;
using Microsoft.EntityFrameworkCore;
using static AlamandaApi.Data.AppDbContext;

namespace AlamandaApi.Services.Art {
  public class ArtService {
    private readonly AppDbContext _context;

    public ArtService(AppDbContext context) {
      _context = context;
    }

    public async Task<ArtModel> Create(ArtCreationModel art) {
      var newArt = ObjectMapperUtil.CopyWithCapitalization<ArtCreationModel, ArtModel>(art);
      newArt.Picture = "";

      var result = await _context.FanArts.AddAsync(newArt);
      await _context.SaveChangesAsync();

      if (!string.IsNullOrEmpty(art.Picture) && FieldValidator.IsBase64String(art.Picture)) {
        var savedImage = await ImageHandler.SaveImage(art.Picture, new ImageHandler.ImageSaveOptions {
          Name = newArt.Id.ToString(),
          Folder = "fanArt",
          Quality = 60,
          MaxWidth = 1000,
        });
        newArt.Picture = savedImage;
        var newResult = _context.FanArts.Update(newArt);
        await _context.SaveChangesAsync();
        return newResult.Entity;
      }
      return result.Entity;
    }

    public async Task<ArtModel> Update(ArtModel art) {
      var existing = await _context.FanArts
        .FirstOrDefaultAsync(m => m.Id == art.Id);
      if (existing == null) throw new Exception("Member not found.");

      existing.Picture = await ImageHandler.SaveImage(art.Picture, new ImageHandler.ImageSaveOptions {
        Name = art.Id.ToString(),
        Folder = "fanArt",
        Quality = 60,
        MaxWidth = 1000,
        PreviousImage = existing.Picture,
      });

      existing.Social = art.Social;
      var result = _context.FanArts.Update(existing);
      await _context.SaveChangesAsync();
      return result.Entity;
    }

    public async Task<PagedResult<ArtModel>> GetAll(int page, int pageSize, string query) {
      var artQuery = _context.FanArts
        .AsNoTracking();
      var totalItems = await artQuery.CountAsync();
      var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

      if (!string.IsNullOrWhiteSpace(query)) {
        var q = query.Trim().ToLower();
        artQuery = artQuery.Where(m =>
          m.Social.ToLower().Contains(q));
      }

      var items = await artQuery
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(t => new ArtModel {
          Id = t.Id,
          Social = t.Social,
          Picture = t.Picture,
        })
        .ToListAsync();

      return new PagedResult<ArtModel> {
        Items = items,
        TotalPages = totalPages,
        CurrentPage = page
      };
    }

    public async Task<ArtModel?> GetById(int id) {
      return await _context.FanArts
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == id);
    }
  }
}

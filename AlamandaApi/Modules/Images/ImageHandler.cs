using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

public static class ImageHandler {
  private static readonly string IMAGES_STORAGE = Environment.GetEnvironmentVariable("IMAGES_STORAGE") ?? "";
  private static readonly string IMAGES_FOLDER = Environment.GetEnvironmentVariable("IMAGES_FOLDER") ?? "";
  public class ImageSaveOptions {
    public string Name { get; set; } = "image";
    public string Folder { get; set; } = "default";
    public int Quality { get; set; } = 75;
    public int MaxWidth { get; set; } = 1200;
  }

  public static async Task<string> SaveImage(string base64Image, ImageSaveOptions options) {
    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", IMAGES_FOLDER, options.Folder);
    Directory.CreateDirectory(uploadsFolder);

    var fileName = $"{options.Name}.webp";
    var filePath = Path.Combine(uploadsFolder, fileName);
    var base64Data = base64Image.Contains(",") ? base64Image.Split(",")[1] : base64Image;
    var imageBytes = Convert.FromBase64String(base64Data);

    using var inputStream = new MemoryStream(imageBytes);
    using var image = await Image.LoadAsync(inputStream);

    if (image.Width > options.MaxWidth) {
      var ratio = (float)options.MaxWidth / image.Width;
      image.Mutate(x => x.Resize(options.MaxWidth, (int)(image.Height * ratio)));
    }

    image.Metadata.ExifProfile = null;
    var encoder = new WebpEncoder { Quality = options.Quality };

    await using var outputStream = new FileStream(filePath, FileMode.Create);
    await image.SaveAsync(outputStream, encoder);

    return $"{IMAGES_STORAGE}/{IMAGES_FOLDER}/{options.Folder}/{fileName}";
  }
}

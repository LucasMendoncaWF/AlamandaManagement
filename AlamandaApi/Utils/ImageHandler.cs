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
    public string? PreviousImage { get; set; } = null;
  }

  public static async Task<string?> SaveImage(string? base64Image, ImageSaveOptions options) {
    var folder = options.Folder.Replace("Model", "");
    if (!string.IsNullOrEmpty(base64Image) && base64Image.StartsWith("data:image")) {
      if (!string.IsNullOrEmpty(options.PreviousImage)) {
        var previousFileName = Path.GetFileName(options.PreviousImage);
        var previousFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", IMAGES_FOLDER, folder, previousFileName);

        if (File.Exists(previousFilePath)) {
          File.Delete(previousFilePath);
        }
      }
      var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", IMAGES_FOLDER, folder);
      Directory.CreateDirectory(uploadsFolder);

      var fileName = $"{options.Name}_{DateTime.Now:dd-MM-yy-HH-mm-ss}.webp";
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

      return $"{IMAGES_STORAGE}/{IMAGES_FOLDER}/{folder}/{fileName}";
    }
    else if (!string.IsNullOrEmpty(options.PreviousImage) && string.IsNullOrEmpty(base64Image)) {
      var previousFileName = Path.GetFileName(options.PreviousImage);
      var previousFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", IMAGES_FOLDER, folder, previousFileName);

      if (File.Exists(previousFilePath)) {
        File.Delete(previousFilePath);
      }
      return null;
    }
    else {
      return options.PreviousImage;
    }
  }
}

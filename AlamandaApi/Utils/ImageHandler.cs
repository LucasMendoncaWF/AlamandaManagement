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
    public int? MaxKb { get; set; } = 500;
  }

  public static async Task<string?> SaveImage(string? base64Image, ImageSaveOptions options) {
    var folder = options.Folder.Replace("Model", "");
    if (!string.IsNullOrEmpty(base64Image) && base64Image.StartsWith("data:image")) {
      if (!string.IsNullOrEmpty(options.PreviousImage)) {
        DeleteImage(folder, options.PreviousImage);
      }
      var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", IMAGES_FOLDER, folder);
      Directory.CreateDirectory(uploadsFolder);

      var fileName = $"{options.Name}_{DateTime.Now:dd-MM-yy-HH-mm-ss}.webp";
      var filePath = Path.Combine(uploadsFolder, fileName);
      var base64Data = base64Image.Contains(",") ? base64Image.Split(",")[1] : base64Image;
      var imageBytes = Convert.FromBase64String(base64Data);

      using var inputStream = new MemoryStream(imageBytes);
      using var image = await Image.LoadAsync(inputStream);

      Console.WriteLine(options.MaxKb);
      if (options.MaxKb.HasValue) {
        var maxBytes = options.MaxKb.Value * 1024;
        if (imageBytes.Length > maxBytes) {
          throw new Exception($"Image exceeds the {options.MaxKb}KB size.");
        }
      }

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
      DeleteImage(folder, options.PreviousImage);
      return null;
    }
    else {
      return options.PreviousImage;
    }
  }

  public static async Task<List<string?>?> SaveImages(
    ImageSaveOptions baseOptions,
    List<string?>? newImages,
    List<string?>? previousImages = null
  ) {
    List<string?> newSavedImages = [];
    if (previousImages != null) {
      foreach (var prevImage in previousImages) {
        if (!(newImages ?? new List<string?>()).Contains(prevImage)) {
          DeleteImage(baseOptions.Folder, prevImage);
        }
      }
    }

    if (newImages == null) {
      return null;
    }

    for (int i = 0; i < newImages?.Count; i++) {
      string? image = newImages[i];
      if (previousImages != null && previousImages.Contains(image)) {
        newSavedImages.Add(image);
        continue;
      }
      if (!string.IsNullOrEmpty(image) && image.StartsWith("data:image")) {
        ImageSaveOptions options = new ImageSaveOptions {
          Folder = baseOptions.Folder,
          MaxWidth = baseOptions.MaxWidth,
          Name = baseOptions.Name + "_multi_" + i,
          MaxKb = baseOptions.MaxKb,
        };
        string? newImageUrl = await SaveImage(image, options);
        if (newImageUrl != null) {
          newSavedImages.Add(newImageUrl);
        }
      }
    }
    return newSavedImages;
  }

  public static void DeleteImage(string folder, string? image) {
    if (string.IsNullOrEmpty(image)) return;
    folder = folder.Replace("/", Path.DirectorySeparatorChar.ToString());

    var previousFileName = Path.GetFileName(image);
    if (string.IsNullOrEmpty(previousFileName)) return;
    var fullFolderPath = Path.Combine(
      Directory.GetCurrentDirectory(),
      "wwwroot",
      IMAGES_FOLDER,
      folder
    );
    var fullImagePath = Path.Combine(fullFolderPath, previousFileName);
    Console.WriteLine($"[DeleteImage] {fullImagePath}");

    if (File.Exists(fullImagePath)) {
      File.Delete(fullImagePath);
      if (Directory.Exists(fullFolderPath) && !Directory.EnumerateFileSystemEntries(fullFolderPath).Any()) {
        Directory.Delete(fullFolderPath, recursive: true);
      }
    }
  }
}

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

public static class ImageHandler
{
  private static readonly string backendBaseUrl = Environment.GetEnvironmentVariable("BACKEND_URL") ?? "";
  
  public class ImageSaveOptions
  {
      public string Folder { get; set; } = "default";
      public int Quality { get; set; } = 75;
      public int MaxWidth { get; set; } = 1200;
  }
  public static string SaveImage(string base64Image, string imageName, string folder)
  {
    var imageUrl = Environment.GetEnvironmentVariable("IMAGES_FOLDER");
    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folder);
    Directory.CreateDirectory(uploadsFolder);

    var fileName = $"{imageName}.jpg";
    var filePath = Path.Combine(uploadsFolder, fileName);

    var base64Data = base64Image.Contains(",") ? base64Image.Split(",")[1] : base64Image;
    var imageBytes = Convert.FromBase64String(base64Data);

    using var inputStream = new MemoryStream(imageBytes);
    using var image = Image.Load(inputStream);

    if (image.Width > 1200)
    {
      var ratio = 1200f / image.Width;
      image.Mutate(x => x.Resize(1200, (int)(image.Height * ratio)));
    }
    image.Metadata.ExifProfile = null;

    var encoder = new JpegEncoder
    {
      Quality = 75
    };

    image.Save(filePath, encoder);

    var baseUrl = backendBaseUrl.EndsWith("/") ? backendBaseUrl : backendBaseUrl + "/";
    var imagePath = imageUrl.EndsWith("/") ? imageUrl : imageUrl + "/";

    return $"{baseUrl}{imagePath}{folder}/{fileName}";
  }
}

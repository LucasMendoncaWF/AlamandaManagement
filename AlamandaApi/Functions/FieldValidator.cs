public static class FieldValidator {
  public static bool IsBase64String(string base64) {
    if (string.IsNullOrEmpty(base64)) {
      return false;
    }
    var data = base64.Contains(",") ? base64.Substring(base64.IndexOf(",") + 1) : base64;

    try {
      int maxByteLength = data.Length * 3 / 4;
      Span<byte> buffer = new Span<byte>(new byte[maxByteLength]);
      return Convert.TryFromBase64String(data, buffer, out int bytesParsed);
    }
    catch {
      return false;
    }
  }
}

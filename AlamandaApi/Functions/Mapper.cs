public static class ObjectMapperUtil
{
  public static TTarget CopyWithCapitalization<TSource, TTarget>(TSource source)
    where TTarget : new()
  {
    var target = new TTarget();

    var sourceProps = typeof(TSource).GetProperties();
    var targetProps = typeof(TTarget).GetProperties();

    foreach (var sourceProp in sourceProps)
    {
      if (!sourceProp.CanRead) continue;

      var capitalizedName = char.ToUpper(sourceProp.Name[0]) + sourceProp.Name.Substring(1);
      var targetProp = targetProps.FirstOrDefault(p => p.Name == capitalizedName && p.CanWrite);

      if (targetProp != null && targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
      {
        var value = sourceProp.GetValue(source);
        targetProp.SetValue(target, value);
      }
    }

    return target;
  }
}

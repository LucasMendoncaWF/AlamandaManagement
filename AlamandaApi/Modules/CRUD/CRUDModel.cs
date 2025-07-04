using AlamandaApi.Data;

namespace AlamandaApi.Services.CRUD {
  public class ListQueryParams {
    public int? Page { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? QueryString { get; set; } = "";
    public string? SortBy { get; set; } = "Id";
    public string? SortDirection { get; set; } = "descending";
  }

  public class UpdateEntityOptions<T, X> where T : class where X : class {
    public X UpdatedObject { get; set; } = null!;
    public string[] PropertiesToUpdate { get; set; } = Array.Empty<string>();
    public Func<T, X, string, AppDbContext, Task<T>>? CustomUpdate { get; set; }
    public Func<IQueryable<T>, IQueryable<T>>? Include { get; set; }
  }
}
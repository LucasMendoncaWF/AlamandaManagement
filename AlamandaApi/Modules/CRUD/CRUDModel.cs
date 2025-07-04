using System.Linq.Expressions;
using AlamandaApi.Data;

namespace AlamandaApi.Services.CRUD {
  public class ListQueryParams {
    public int? Page { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? QueryString { get; set; } = "";
    public string? SortBy { get; set; } = "Id";
    public string? SortDirection { get; set; } = "descending";
  }

  public class ListOptions<T, TResult> {
     public ListQueryParams QueryParams { get; set; } = new();
    public HashSet<string> AllowedSortColumns { get; set; } = new();
    public Expression<Func<T, TResult>> Selector { get; set; } = default!;
  }

  public class UpdateEntityOptions<T, X> where T : class where X : class {
    public X UpdatedObject { get; set; } = null!;
    public string[] PropertiesToUpdate { get; set; } = Array.Empty<string>();
    public Func<T, X, string, AppDbContext, Task<T>>? CustomUpdate { get; set; }
    public Func<IQueryable<T>, IQueryable<T>>? Include { get; set; }
  }
}
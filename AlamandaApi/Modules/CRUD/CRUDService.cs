using AlamandaApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using static AlamandaApi.Data.AppDbContext;

namespace AlamandaApi.Services.CRUD {

  public class CRUDService<T> where T : class, new() {
    private readonly AppDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly string _tableName;

    // Cache keys tracking for cleanup
    private static readonly HashSet<string> _knownCacheKeys = new();
    private static readonly object _cacheLock = new();

    public AppDbContext Context => _context;

    public CRUDService(AppDbContext context, IMemoryCache memoryCache) {
      _context = context;
      _memoryCache = memoryCache;
      _tableName = typeof(T).Name;
    }

    /// Returns paged, filtered, sorted list with caching support.
    public async Task<PagedResult<TResult>> GetPagedAsync<TResult>(
      ListQueryParams queryParams,
      HashSet<string> allowedSortColumns,
      Expression<Func<T, TResult>> selector
    ) {
      var dbSet = _context.Set<T>().AsNoTracking();

      int pageSize = queryParams.PageSize ?? 10;
      int page = queryParams.Page ?? 1;

      string sortBy = allowedSortColumns.Contains(queryParams.SortBy ?? "") ? queryParams.SortBy! : "Id";
      string sortDir = (queryParams.SortDirection?.ToLower() == "descending") ? "descending" : "ascending";

      string query = queryParams.QueryString?.Trim() ?? "";
      var filter = CRUDServiceHelper.BuildStringContainsFilter<T>(query, allowedSortColumns);

      string cacheKey = $"{_tableName}_GetAll_{page}_{pageSize}_{query}_{sortBy}_{sortDir}";

      if (_memoryCache.TryGetValue(cacheKey, out PagedResult<TResult>? cached))
        return cached!;

      // Apply filtering if query present
      var filtered = !string.IsNullOrEmpty(query) && filter != null
        ? dbSet.Where(filter)
        : dbSet;

      // Apply sorting
      filtered = sortDir == "descending"
        ? filtered.OrderByDescending(x => EF.Property<object>(x, sortBy))
        : filtered.OrderBy(x => EF.Property<object>(x, sortBy));

      // Get counts and page items
      var totalItems = await filtered.CountAsync();
      var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
      var items = await filtered.Skip((page - 1) * pageSize).Take(pageSize).Select(selector).ToListAsync();

      var result = new PagedResult<TResult> {
        Items = items,
        TotalPages = totalPages,
        CurrentPage = page
      };

      SetCache(cacheKey, result);
      return result;
    }

    /// Gets an entity by arbitrary property, with caching.
    public async Task<T?> GetByPropertyAsync(string propName, object value) {
      var key = $"{_tableName}_GetBy_{propName}_{value?.ToString()?.ToLower() ?? "null"}";

      if (_memoryCache.TryGetValue(key, out T? cached))
        return cached;

      var param = Expression.Parameter(typeof(T), "x");
      var prop = Expression.Property(param, propName);
      var constant = Expression.Constant(value);
      var predicate = Expression.Lambda<Func<T, bool>>(Expression.Equal(prop, constant), param);

      var entity = await _context.Set<T>().FirstOrDefaultAsync(predicate);

      if (entity != null)
        SetCache(key, entity);

      return entity;
    }

    /// Creates a new entity with specified properties and optional custom creation logic.
    public async Task<T> CreateEntityAsync<X>(UpdateEntityOptions<T, X> options) where X : class {
      var newEntity = new T();

      if (options.CustomUpdate != null) {
        newEntity = await options.CustomUpdate(newEntity, options.UpdatedObject, _tableName, _context);
      }

      foreach (var propName in options.PropertiesToUpdate) {
        var prop = typeof(T).GetProperty(propName);
        if (prop == null) continue;

        var inputValue = typeof(X).GetProperty(propName)?.GetValue(options.UpdatedObject);
        prop.SetValue(newEntity, inputValue);
      }

      _context.Set<T>().Add(newEntity);
      await _context.SaveChangesAsync();
      ClearCache();

      return newEntity;
    }

    /// Updates an existing entity by its key with specified properties and optional custom update logic.
    public async Task<T> UpdateEntityAsync<X>(UpdateEntityOptions<T, X> options) where X : class {
      var dbSet = _context.Set<T>();
      IQueryable<T> query = dbSet;

      var keyName = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.FirstOrDefault()?.Name;
      if (keyName == null) throw new Exception("Primary key not found");

      var id = typeof(X).GetProperty(keyName)?.GetValue(options.UpdatedObject);
      if (id == null) throw new Exception("Id not found");

      if (options.Include != null) {
        query = options.Include(query);
      }

      var existing = await query.FirstOrDefaultAsync(e =>
        EF.Property<object>(e, keyName!).Equals(id)
      ) ?? throw new Exception($"{typeof(T).Name} not found.");

      if (options.CustomUpdate != null) {
        existing = await options.CustomUpdate(existing, options.UpdatedObject, _tableName, _context);
      }

      foreach (var propName in options.PropertiesToUpdate) {
        var targetProp = typeof(T).GetProperty(propName);
        var sourceProp = typeof(X).GetProperty(propName);
        if (targetProp == null || sourceProp == null) continue;

        var value = sourceProp.GetValue(options.UpdatedObject);
        targetProp.SetValue(existing, value);
      }
      _context.Update(existing);
      await _context.SaveChangesAsync();
      ClearCache();

      return existing;
    }

    /// Synchronizes a many-to-many relation by adding/removing from currentCollection based on newIds.
    public async Task SyncManyToManyRelation<TJoin>(
      ICollection<TJoin> currentCollection,
      IEnumerable<string> newIds,
      IQueryable<TJoin> dbSet,
      Func<TJoin, string> idSelector
    ) where TJoin : class {
      var currentIds = currentCollection.Select(idSelector).ToHashSet();
      var newIdsSet = newIds.ToHashSet();

      if (currentIds.SetEquals(newIdsSet)) return;

      var allPossibleItems = await dbSet.ToListAsync();

      var toAdd = allPossibleItems
          .Where(item => newIdsSet.Contains(idSelector(item)) && !currentIds.Contains(idSelector(item)))
          .ToList();

      var toRemove = currentCollection
          .Where(item => !newIdsSet.Contains(idSelector(item)))
          .ToList();

      foreach (var item in toRemove)
        currentCollection.Remove(item);

      foreach (var item in toAdd)
        currentCollection.Add(item);
    }

    // cache helpers
    private void SetCache(string key, object value) {
      var options = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
      _memoryCache.Set(key, value, options);

      lock (_cacheLock) {
        _knownCacheKeys.Add(key);
      }
    }

    public void ClearCache() {
      lock (_cacheLock) {
        var prefix = _tableName + "_";
        var keysToRemove = _knownCacheKeys.Where(k => k.StartsWith(prefix)).ToList();

        foreach (var key in keysToRemove) {
          _memoryCache.Remove(key);
          _knownCacheKeys.Remove(key);
        }
      }
    }
  }

  public static class CRUDServiceHelper {
    public static Expression<Func<T, bool>> BuildStringContainsFilter<T>(string? query, IEnumerable<string> props) {
      if (string.IsNullOrWhiteSpace(query) || !props.Any()) return x => true;

      var param = Expression.Parameter(typeof(T), "x");
      var loweredQuery = query.ToLower();
      Expression? combined = null;

      foreach (var propName in props) {
        var prop = typeof(T).GetProperty(propName);
        if (prop?.PropertyType != typeof(string)) continue;

        var access = Expression.Property(param, prop);
        var notNull = Expression.NotEqual(access, Expression.Constant(null, typeof(string)));
        var toLower = Expression.Call(access, nameof(string.ToLower), Type.EmptyTypes);
        var contains = Expression.Call(toLower, nameof(string.Contains), Type.EmptyTypes, Expression.Constant(loweredQuery));
        var condition = Expression.AndAlso(notNull, contains);

        combined = combined == null ? condition : Expression.OrElse(combined, condition);
      }

      return combined != null ? Expression.Lambda<Func<T, bool>>(combined, param) : x => true;
    }
  }
}

using AlamandaApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using static AlamandaApi.Data.AppDbContext;

namespace AlamandaApi.Services.CRUD {

  public class CRUDService<T> where T : class, new() {
    private readonly AppDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly string _tableName;
    private static readonly HashSet<string> _knownCacheKeys = new();
    private static readonly object _cacheLock = new();
    private static readonly ConcurrentDictionary<string, PropertyInfo> _propertyCache = new();

    public AppDbContext Context => _context;

    public CRUDService(AppDbContext context, IMemoryCache memoryCache) {
      _context = context;
      _memoryCache = memoryCache;
      _tableName = typeof(T).Name;
    }

    public async Task<PagedResult<TResult>> GetPagedAsync<TResult>(ListOptions<T, TResult> options) {
      var dbSet = _context.Set<T>().AsNoTracking();
      int pageSize = options.QueryParams.PageSize ?? 10;
      int page = options.QueryParams.Page ?? 1;
      string sortBy = options.AllowedSortColumns.Contains(options.QueryParams.SortBy ?? "") ? options.QueryParams.SortBy! : "Id";
      bool descending = options.QueryParams.SortDirection == "descending";
      string query = options.QueryParams.QueryString?.Trim() ?? "";
      var filter = CRUDServiceHelper.BuildStringContainsFilter<T>(query, options.AllowedSortColumns);
      string cacheKey = HashCacheKey($"{_tableName}_GetAll_{page}_{pageSize}_{query}_{sortBy}_{(descending ? "desc" : "asc")}");

      if (_memoryCache.TryGetValue(cacheKey, out PagedResult<TResult>? cached))
        return cached!;

      IQueryable<T> filtered = !string.IsNullOrEmpty(query) && filter != null ? dbSet.Where(filter) : dbSet;

      var param = Expression.Parameter(typeof(T), "x");
      var property = Expression.Property(param, sortBy);
      var lambda = Expression.Lambda(property, param);
      filtered = filtered.Provider.CreateQuery<T>(
        Expression.Call(
          typeof(Queryable),
          descending ? "OrderByDescending" : "OrderBy",
          [ typeof(T), property.Type ],
          filtered.Expression,
          Expression.Quote(lambda)
        )
      );

      var totalItems = await filtered.CountAsync();
      var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
      var items = await filtered.Skip((page - 1) * pageSize).Take(pageSize).Select(options.Selector).ToListAsync();

      var result = new PagedResult<TResult> {
        Items = items,
        TotalPages = totalPages,
        CurrentPage = page
      };

      SetCache(cacheKey, result);
      return result;
    }

    public async Task<T?> GetByPropertyAsync(string propName, object value) {
      var key = HashCacheKey($"{_tableName}_GetBy_{propName}_{value?.ToString() ?? "null"}");

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

    public async Task<T> CreateEntityAsync<X>(UpdateEntityOptions<T, X> options) where X : class {
      var newEntity = new T();
      if (options.CustomUpdate != null)
        newEntity = await options.CustomUpdate(newEntity, options.UpdatedObject, _tableName, _context);

      var updatedTypeProps = typeof(X).GetProperties().ToDictionary(p => p.Name);
      foreach (var propName in options.PropertiesToUpdate) {
        if (!_propertyCache.TryGetValue(propName, out var targetProp)) {
          targetProp = typeof(T).GetProperty(propName)!;
          _propertyCache[propName] = targetProp;
        }
        if (!updatedTypeProps.TryGetValue(propName, out var sourceProp)) continue;
        targetProp?.SetValue(newEntity, sourceProp.GetValue(options.UpdatedObject));
      }

      _context.Set<T>().Add(newEntity);
      await _context.SaveChangesAsync();
      ClearCache();
      return newEntity;
    }

    public async Task<T> UpdateEntityAsync<X>(UpdateEntityOptions<T, X> options) where X : class {
      var dbSet = _context.Set<T>();
      IQueryable<T> query = dbSet;

      var keyName = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.FirstOrDefault()?.Name ?? throw new Exception("Primary key not found");
      var id = typeof(X).GetProperty(keyName)?.GetValue(options.UpdatedObject) ?? throw new Exception("Id not found");

      if (options.Include != null)
        query = options.Include(query);

      var existing = await query.FirstOrDefaultAsync(e => EF.Property<object>(e, keyName!).Equals(id)) ?? throw new Exception($"{typeof(T).Name} not found.");

      if (options.CustomUpdate != null)
        existing = await options.CustomUpdate(existing, options.UpdatedObject, _tableName, _context);

      var updatedTypeProps = typeof(X).GetProperties().ToDictionary(p => p.Name);
      foreach (var propName in options.PropertiesToUpdate) {
        if (!_propertyCache.TryGetValue(propName, out var targetProp)) {
          targetProp = typeof(T).GetProperty(propName)!;
          _propertyCache[propName] = targetProp;
        }
        if (!updatedTypeProps.TryGetValue(propName, out var sourceProp)) continue;
        targetProp?.SetValue(existing, sourceProp.GetValue(options.UpdatedObject));
      }

      _context.Update(existing);
      await _context.SaveChangesAsync();
      ClearCache();
      return existing;
    }

    public async Task DeleteByIdAsync(object id) {
      var dbSet = _context.Set<T>();
      var keyName = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.FirstOrDefault()?.Name
          ?? throw new Exception("Primary key not found");
      var param = Expression.Parameter(typeof(T), "x");
      var prop = Expression.Property(param, keyName);
      var constant = Expression.Constant(id);
      var predicate = Expression.Lambda<Func<T, bool>>(Expression.Equal(prop, constant), param);

      var entity = await dbSet.FirstOrDefaultAsync(predicate);

      if (entity == null) {
        throw new Exception("Item not found");
      }

      dbSet.Remove(entity);
      await _context.SaveChangesAsync();
      ClearCache();
    }

    public async Task SyncManyToManyRelation<TJoin, TKey>(
      ICollection<TJoin> currentCollection,
      IEnumerable<string> newIds,
      IQueryable<TJoin> dbSet,
      Expression<Func<TJoin, TKey>> idSelectorExpr
    ) where TJoin : class {
      var idSelectorCompiled = idSelectorExpr.Compile();
      var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(TKey));
      var newIdsConverted = newIds.Select(id => (TKey)converter.ConvertFromString(id)!).ToHashSet();
      var currentIdsConverted = currentCollection.Select(idSelectorCompiled).ToHashSet();

      if (newIdsConverted.SetEquals(currentIdsConverted)) return;

      var relevantIds = newIdsConverted.Union(currentIdsConverted).ToList();

      var param = Expression.Parameter(typeof(TJoin), "x");
      var prop = (idSelectorExpr.Body as MemberExpression)?.Member as PropertyInfo ?? throw new Exception("Invalid expression");
      var propAccess = Expression.Property(param, prop.Name);
      var containsMethod = typeof(List<TKey>).GetMethod("Contains", new[] { typeof(TKey) })!;
      var body = Expression.Call(Expression.Constant(relevantIds), containsMethod, propAccess);
      var lambda = Expression.Lambda<Func<TJoin, bool>>(body, param);

      var allPossibleItems = await dbSet.Where(lambda).ToListAsync();

      var toAdd = allPossibleItems.Where(item => newIdsConverted.Contains(idSelectorCompiled(item)) && !currentIdsConverted.Contains(idSelectorCompiled(item))).ToList();
      var toRemove = currentCollection.Where(item => !newIdsConverted.Contains(idSelectorCompiled(item))).ToList();

      foreach (var item in toRemove)
        currentCollection.Remove(item);

      foreach (var item in toAdd)
        currentCollection.Add(item);
    }

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

    private static string HashCacheKey(string key) => key.Length > 100 ? key.GetHashCode().ToString() : key;
  }

  public static class CRUDServiceHelper {
    public static Expression<Func<T, bool>> BuildStringContainsFilter<T>(string? query, IEnumerable<string> props) {
      if (string.IsNullOrWhiteSpace(query) || !props.Any()) return x => true;

      var param = Expression.Parameter(typeof(T), "x");
      Expression? combined = null;
      var efFunctionsProperty = Expression.Property(null, typeof(EF).GetProperty(nameof(EF.Functions))!);
      var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
        nameof(DbFunctionsExtensions.Like),
        [ typeof(DbFunctions), typeof(string), typeof(string) ]
      )!;

      var patternConstant = Expression.Constant($"%{query}%");

      foreach (var propName in props) {
        var prop = typeof(T).GetProperty(propName);
        if (prop?.PropertyType != typeof(string)) continue;

        var access = Expression.Property(param, prop);
        var notNull = Expression.NotEqual(access, Expression.Constant(null, typeof(string)));
        var likeCall = Expression.Call(likeMethod, efFunctionsProperty, access, patternConstant);
        var condition = Expression.AndAlso(notNull, likeCall);

        combined = combined == null ? condition : Expression.OrElse(combined, condition);
      }

      return combined != null ? Expression.Lambda<Func<T, bool>>(combined, param) : x => true;
    }
  }
} 
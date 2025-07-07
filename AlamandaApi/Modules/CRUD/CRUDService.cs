using AlamandaApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;
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
      IQueryable<T> dbSet = _context.Set<T>().AsNoTracking();

      if (options.Include != null) {
        dbSet = options.Include(dbSet);
      }

      int pageSize = options.QueryParams.PageSize ?? 10;
      int page = options.QueryParams.Page ?? 1;
      string? requestedSortBy = options.QueryParams.SortBy;
      string sortBy = requestedSortBy ?? "Id";

      var propertyName = typeof(T).GetProperties()
        .FirstOrDefault(p => string.Equals(p.Name, sortBy, StringComparison.OrdinalIgnoreCase))
        ?.Name ?? "Id";

      bool descending = options.QueryParams.SortDirection == "descending";
      string query = options.QueryParams.QueryString?.Trim() ?? "";
      var filter = CRUDServiceHelper.BuildStringContainsFilter<T>(query, options.AllowedSortColumns);
      string cacheKey = HashCacheKey($"{_tableName}_GetAll_{page}_{pageSize}_{query}_{sortBy}_{(descending ? "desc" : "asc")}");

      if (_memoryCache.TryGetValue(cacheKey, out PagedResult<TResult>? cached))
        return cached!;

      IQueryable<T> filtered = !string.IsNullOrEmpty(query) && filter != null ? dbSet.Where(filter) : dbSet;

      var lambda = GetOrderByExpression<T>(propertyName);
      filtered = filtered.Provider.CreateQuery<T>(
        Expression.Call(
          typeof(Queryable),
          descending ? "OrderByDescending" : "OrderBy",
          [ typeof(T), lambda.Body.Type ],
          filtered.Expression,
          Expression.Quote(lambda)
        )
      );

      var totalItems = await filtered.CountAsync();
      var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
      var items = await filtered
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(options.Selector)
        .ToListAsync();

      var result = new PagedResult<TResult> {
        Items = items,
        TotalPages = totalPages,
        CurrentPage = page
      };

      SetCache(cacheKey, result);
      return result;
    }

    private static LambdaExpression GetOrderByExpression<E>(string propertyName) {
      var param = Expression.Parameter(typeof(E), "x");
      var property = Expression.Property(param, propertyName);
      var delegateType = typeof(Func<,>).MakeGenericType(typeof(E), property.Type);
      return Expression.Lambda(delegateType, property, param);
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
      if (options.CustomUpdate != null) {
        newEntity = await options.CustomUpdate(newEntity, options.UpdatedObject, _tableName, _context);
      }

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
      try {
        await _context.SaveChangesAsync();
      } catch (DbUpdateException dbEx) when (dbEx.InnerException != null) {
        ErrorHandler.GetFriendlyError(dbEx.InnerException as PostgresException);
      }
      if (options.AfterAll != null) {
        await options.AfterAll(newEntity, options.UpdatedObject, _tableName, _context);
      }
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
      try {
        await _context.SaveChangesAsync();
      } catch (DbUpdateException dbEx) when (dbEx.InnerException != null) {
        ErrorHandler.GetFriendlyError(dbEx.InnerException as PostgresException);
      }
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
        IEnumerable<TKey> newIds,
        IQueryable<TJoin> dbSet,
        Expression<Func<TJoin, TKey>> idSelectorExpr
    ) where TJoin : class {
      var idSelectorCompiled = idSelectorExpr.Compile();
      var newIdsSet = new HashSet<TKey>(newIds);
      var currentIdsSet = new HashSet<TKey>(currentCollection.Select(idSelectorCompiled));

      if (newIdsSet.SetEquals(currentIdsSet)) return;

      var relevantIds = newIdsSet.Union(currentIdsSet).ToHashSet();
      var param = Expression.Parameter(typeof(TJoin), "x");
      var idMember = (idSelectorExpr.Body as MemberExpression) ?? throw new Exception("Invalid id selector expression");
      var replacedProperty = Expression.Property(param, idMember.Member.Name);
      var containsMethod = typeof(HashSet<TKey>).GetMethod(nameof(HashSet<TKey>.Contains), new[] { typeof(TKey) })!;
      var relevantIdsConst = Expression.Constant(relevantIds);
      var body = Expression.Call(relevantIdsConst, containsMethod, replacedProperty);
      var lambda = Expression.Lambda<Func<TJoin, bool>>(body, param);
      var allPossibleItems = await dbSet.Where(lambda).ToListAsync();

      var toAdd = allPossibleItems
        .Where(item => newIdsSet.Contains(idSelectorCompiled(item)) && !currentIdsSet.Contains(idSelectorCompiled(item)))
        .ToList();

      var toRemove = currentCollection
        .Where(item => !newIdsSet.Contains(idSelectorCompiled(item)))
        .ToList();

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
      var likeMethod = typeof(NpgsqlDbFunctionsExtensions).GetMethod(
          nameof(NpgsqlDbFunctionsExtensions.ILike),
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
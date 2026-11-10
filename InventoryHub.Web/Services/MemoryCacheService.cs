using Microsoft.Extensions.Caching.Memory;

namespace InventoryHub.Web.Services;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly HashSet<string> _keys = new();
    private readonly object _lock = new();

    public MemoryCacheService(IMemoryCache cache) => _cache = cache;

    public bool TryGet<T>(string key, out T? value) => _cache.TryGetValue(key, out value);

    public void Set<T>(string key, T value, TimeSpan ttl)
    {
        _cache.Set(key, value, ttl);
        lock (_lock) _keys.Add(key);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        lock (_lock) _keys.Remove(key);
    }

    public void RemoveByPrefix(string prefix)
    {
        List<string> remove;
        lock (_lock) remove = _keys.Where(k => k.StartsWith(prefix, StringComparison.Ordinal)).ToList();
        foreach (var k in remove) Remove(k);
    }
}
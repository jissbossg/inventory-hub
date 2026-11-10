namespace InventoryHub.Web.Services;

public interface ICacheService
{
    bool TryGet<T>(string key, out T? value);
    void Set<T>(string key, T value, TimeSpan ttl);
    void Remove(string key);
    void RemoveByPrefix(string prefix);
}
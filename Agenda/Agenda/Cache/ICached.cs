namespace Vehiculos.Cache;

public interface ICached<in TKey, TValue> where TKey : notnull {
    void Add(TKey key, TValue value);
    TValue? Get(TKey key);
    bool Remove(TKey key);
}
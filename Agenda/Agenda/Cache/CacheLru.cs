using Serilog;

namespace Vehiculos.Cache;

public class CacheLru<TKey, TValue> : ICached<TKey, TValue>
    where TKey : notnull {
    private readonly int _capacity = 3;
    private readonly Dictionary<TKey, TValue> _dataContacto = new();
    private readonly LinkedList<TKey> _set = new();
    private readonly ILogger _logger = Log.ForContext<CacheLru<TKey, TValue>>();

    public CacheLru(int capacity) {
        if (capacity <= 0)
            throw new ArgumentException("La capacidad no puede ser menor o igual que 0", nameof(capacity));
        _capacity = capacity;
    }
    
    public void Add(TKey key, TValue value) {
        _logger.Debug("[LRU-ADD] Intentando añadir clave: {Key}", key);
        if (_dataContacto.TryGetValue(key, out var existingValue)) {
            _logger.Debug("[LRU-ADD] Clave {Key} ya existe. Actualizando valor: {Old} -> {New}",
                key, existingValue, value);
            _dataContacto[key] = value;
            RefreshUsage(key);
            return;
        }
        _logger.Debug("[LRU-ADD] Clave {Key} es nueva. Capacidad actual: {Used}/{Total}",
            key, _dataContacto.Count, _capacity);
        if (_dataContacto.Count >= _capacity) {
            var oldestKey = _set.First!.Value;
            _logger.Debug("[LRU-EVICT] Cache llena. Desalojando elemento más antiguo: {Key} = {Value}",
                oldestKey);
            _set.RemoveFirst();
            _dataContacto.Remove(oldestKey);
            
        }
        _dataContacto.Add(key, value);
        _set.AddLast(key);
        _logger.Debug("[LRU-ADD] Elemento añadido. Nueva lista de uso: {Order}",
            string.Join(" -> ", _set));
    }
    public TValue? Get(TKey key) {
        _logger.Debug("[LRU-GET] Buscando clave: {Key}", key);
        if (!_dataContacto.TryGetValue(key, out var value)) {
            _logger.Debug("[LRU-GET] Clave {Key} NO encontrada en cache", key);
            return default;
        }
        _logger.Debug("[LRU-GET] Clave {Key} encontrada con valor: {Value}. Rejuveneciendo...",
            key, value);
        RefreshUsage(key);
        _logger.Debug("[LRU-GET] Lista tras rejuvenecimiento: {Order}",
            string.Join(" -> ", _set));
        return value;
    }
    public bool Remove(TKey key) {
        _logger.Debug("[LRU-REMOVE] Intentando eliminar clave: {Key}", key);
        if (!_dataContacto.Remove(key)) {
            _logger.Debug("[LRU-REMOVE] Clave {Key} no encontrada", key);
            return false;
        }
        _set.Remove(key);
        _logger.Debug("[LRU-REMOVE] Clave {Key} eliminada correctamente", key);
        return true;
    } 
    private void RefreshUsage(TKey key) {
        _logger.Verbose("[LRU-REFRESH] Moviendo clave {Key} al final de la lista", key);
        _set.Remove(key);
        _set.AddLast(key);
    }
}
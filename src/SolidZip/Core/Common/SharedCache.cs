namespace SolidZip.Core.Common;

public class SharedCache<T> where T : class
{
    private object _lock = new();
    private T? _cache = null;
    private bool _wasChanged = false;

    public T Value
    {
        get
        {
            lock (_lock)
                return _cache ?? throw new NullReferenceException(
                    $"Cache of {typeof(T).FullName} was not added, but tried to get, validate it using Exist() method before loading cache");
        }
        set
        {
            lock (_lock)
            {
                if (_cache is not null)
                    _wasChanged = true;
                _cache = value;
            }
        }
    }

    public bool Exists()
    {
        lock (_lock)
            return _cache is not null;
    }

    public void ExpandChanges(Action<T> expandAction)
    {
        if (Exists() && _wasChanged)
            expandAction(Value);
        _cache = null;
    }
}
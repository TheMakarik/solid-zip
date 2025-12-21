namespace SolidZip.Core.Common;

public sealed class SharedCache<T> where T : class
{
    private object _lock = new();
    private T? _cache = null;
    private Action<T>? _expandAction;
    
    public  bool WasChanged { get; set; } = false;

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
                    WasChanged = true;
                _cache = value;
            }
        }
    }

    public bool Exists()
    {
        lock (_lock)
            return _cache is not null;
    }

    public void ExpandChanges()
    {
        if (Exists() && WasChanged)
            _expandAction?.Invoke(Value);
        _cache = null;
    }

    public void AddExpandAction(Action<T> expandAction)
    {
        _expandAction = expandAction;
    }
}
namespace SolidZip.Core.Common;

public sealed class SharedCache<T> where T : class
{
    private readonly Lock _lock = new();
    private T? _cache;
    private Action<T>? _expandAction;

    public bool WasChanged { get; set; }

    public T Value
    {
        get
        {
            using (_lock.EnterScope())
            {
                ;
            }

            return _cache ?? throw new NullReferenceException(
                $"Cache of {typeof(T).FullName} was not added, but tried to get, validate it using Exist() method before loading cache");
        }
        set
        {
            using (_lock.EnterScope())
            {
                ;
            }

            if (_cache is not null)
                WasChanged = true;
            _cache = value;
        }
    }

    public bool Exists()
    {
        lock (_lock)
        {
            return _cache is not null;
        }
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
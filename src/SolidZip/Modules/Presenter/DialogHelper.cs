namespace SolidZip.Modules.Presenter;

public sealed class DialogHelper : IDialogHelper
{
    private readonly ConcurrentDictionary<ApplicationViews, object> _currentOpenedDialogs = new();
    private Action<object>? _closeAction;
    private Action<ApplicationViews, Action<ApplicationViews, object>>? _showFunc;
    private Action<ApplicationViews, Action<ApplicationViews, object>> _showNonBlockingFunc;

    public void Configure(Action<ApplicationViews, Action<ApplicationViews, object>> showFunc,
        Action<object> closeAction, Action<ApplicationViews, Action<ApplicationViews, object>> showNonBlocking)
    {
        _showFunc = showFunc;
        _closeAction = closeAction;
        _showNonBlockingFunc =  showNonBlocking;
    }

    public void Show(ApplicationViews view)
    {
        if (_showFunc is null)
            throw new InvalidOperationException("Cannot open dialog because open function is not configured");

        _showFunc(view, (views, objectView) => _currentOpenedDialogs.TryAdd(views, objectView));
    }

    public void ShowNonBlocking(ApplicationViews view)
    {
        if (_showFunc is null)
            throw new InvalidOperationException("Cannot open dialog because open function is not configured");

        _showNonBlockingFunc(view, (views, objectView) => _currentOpenedDialogs.TryAdd(views, objectView));
    }

    public void Close(ApplicationViews view)
    {
        if (_closeAction is null)
            throw new InvalidOperationException("Cannot close dialog because close action is not configured");

        var success = _currentOpenedDialogs.TryGetValue(view, out var closeObject);
        if (!success) 
            return;
        
        _closeAction(closeObject);
        _currentOpenedDialogs.TryRemove(view, out _);

    }
}
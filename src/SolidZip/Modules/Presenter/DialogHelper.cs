using SolidZip.Core.Contracts.Presenter;

namespace SolidZip.Modules.Presenter;

public sealed class DialogHelper() : IDialogHelper
{
    private Action<ApplicationViews, Action<ApplicationViews, object>>? _showFunc;
    private Action<object>? _closeAction;
    private readonly ConcurrentDictionary<ApplicationViews, object> _currentOpenedDialogs = new();
    
    public void Configure(Action<ApplicationViews, Action<ApplicationViews, object>> showFunc, Action<object> closeAction)
    {
       _showFunc = showFunc;
       _closeAction = closeAction;
    }

    public void Show(ApplicationViews view)
    {
       if(_showFunc is null)
          throw new InvalidOperationException("Cannot open dialog because open function is not configured");
       
       _showFunc(view, ((views, objectView) =>  _currentOpenedDialogs.TryAdd(views, objectView)));
    }

    public void Close(ApplicationViews view)
    {
       if(_closeAction is null)
          throw new InvalidOperationException("Cannot close dialog because close action is not configured");
       
       _closeAction(_currentOpenedDialogs[view]);
       _currentOpenedDialogs.TryRemove(view, out _);
    }
}
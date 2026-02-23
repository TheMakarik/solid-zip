namespace SolidZip.Core.Contracts.Presenter;

public interface IDialogHelper
{
    public void Configure(Action<ApplicationViews, Action<ApplicationViews, object>> showFunc,
        Action<object> closeActio, Action<ApplicationViews, Action<ApplicationViews, object>> showNonBlocking);

    public void Show(ApplicationViews view);
    public void ShowNonBlocking(ApplicationViews view);
    public void Close(ApplicationViews view);
}
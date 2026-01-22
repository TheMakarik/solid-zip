namespace SolidZip.Core.Contracts.Presenter;

public interface IDialogHelper
{
    public void Configure(Action<ApplicationViews, Action<ApplicationViews, object>> showFunc,
        Action<object> closeAction);

    public void Show(ApplicationViews view);
    public void Close(ApplicationViews view);
}
namespace SolidZip.Modules.Presenter;

public sealed class MessageBoxPresenter : IMessageBox
{
    private Func<string, string, MessageBoxButtonEnum, MessageBoxImageEnum, MessageBoxResultEnum>? _showFunc;

    public void Configure(Func<string, string, MessageBoxButtonEnum, MessageBoxImageEnum, MessageBoxResultEnum> showFunc)
    {
        _showFunc = showFunc;
    }

    public MessageBoxResultEnum Show(string messageBoxText, string caption, MessageBoxButtonEnum button, MessageBoxImageEnum icon)
    {
        if (_showFunc is null)
            throw new InvalidOperationException("MessageBox show function is not configured");
        return _showFunc(messageBoxText, caption, button, icon);
    }
}

namespace SolidZip.Modules.Presenter;

public sealed class RequirePasswordPresenter : IRequirePassword, IRecipient<RequirePasswordReadyMessage>
{
    private readonly IDialogHelper _dialogHelper;
    private readonly IMessenger _messenger;
    private RequestPasswordMessage? _currentMessage;

    public RequirePasswordPresenter(IDialogHelper dialogHelper, IMessenger messenger)
    {
        _dialogHelper = dialogHelper;
        _messenger = messenger;
    }

    public string? RequestPassword()
    {
        _currentMessage = new RequestPasswordMessage();
        _dialogHelper.Show(ApplicationViews.RequirePassword);
        return _currentMessage.Response;
    }

    public void Receive(RequirePasswordReadyMessage message)
    {
        if (_currentMessage is not null)
            _messenger.Send(new RequirePasswordDialogOpenedMessage(_currentMessage));
    }
}
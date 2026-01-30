namespace SolidZip.ViewModels;

public sealed partial class RequirePasswordViewModel : ViewModelBase,
    IRecipient<RequirePasswordDialogOpenedMessage>
{
    private readonly IDialogHelper _dialogHelper;
    private RequestPasswordMessage? _requestMessage;
    private bool _replied;
    
    [ObservableProperty] private string _password = string.Empty;

    public RequirePasswordViewModel(StrongTypedLocalizationManager localization, IMessenger messenger,
        IDialogHelper dialogHelper) : base(localization, messenger)
    {
        _dialogHelper = dialogHelper;
        messenger.RegisterAll(this);
        messenger.Send(new RequirePasswordReadyMessage());
    }

    public void Receive(RequirePasswordDialogOpenedMessage message)
    {
        _requestMessage = message.RequestMessage;
    }

    [RelayCommand]
    private void Confirm()
    {
        _replied = true;
        _requestMessage?.Reply(Password);
        _dialogHelper.Close(ApplicationViews.RequirePassword);
    }

    [RelayCommand]
    private void Cancel()
    {
        _replied = true;
        _requestMessage?.Reply(null);
        _dialogHelper.Close(ApplicationViews.RequirePassword);
    }

    [RelayCommand]
    private void OnWindowClosed()
    {
        if (!_replied)
            _requestMessage?.Reply(null);
    }
}

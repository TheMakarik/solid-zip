namespace SolidZip.ViewModels;

public sealed partial class ProgressViewModel : ViewModelBase, 
    IRecipient<UpdateProgressMessage>,
    IRecipient<GetCancellationTokenMessage>,
    IRecipient<SetProgressMessage>
{
    [ObservableProperty]
    [ValueRange(0, 100)]
    private double _progress = 0;

    [ObservableProperty] private string _progressMessage;
    
    private readonly CancellationTokenSource _cancellationToken;
    private readonly IDialogHelper _dialogHelper;

    public ProgressViewModel(StrongTypedLocalizationManager localization, IMessenger messenger, IDialogHelper dialogHelper) : base(localization, messenger)
    {
        _dialogHelper = dialogHelper;
        messenger.RegisterAll(this);
        _cancellationToken = new CancellationTokenSource();
    }

    public void Receive(UpdateProgressMessage message)
    {
        Debug.Assert(message.Value is > 0 and < 100, "Progress must be between 0 and 100");
        Progress = message.Value;
    }

    public void Receive(GetCancellationTokenMessage message)
    {
        message.Reply(_cancellationToken);
    }

    public void Receive(SetProgressMessage message)
    {
        ProgressMessage = message.Value;
    }

    [RelayCommand]
    private void Stop()
    {
        _cancellationToken.Cancel();
        _dialogHelper.Close(ApplicationViews.Progress);
    }
}
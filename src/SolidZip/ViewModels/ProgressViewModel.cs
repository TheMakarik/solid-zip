namespace SolidZip.ViewModels;

public sealed partial class ProgressViewModel : ViewModelBase, IRecipient<UpdateProgressMessage>, IRecipient<GetCancellationTokenMessage>
{
    [ObservableProperty]
    [ValueRange(0, 100)]
    private double _progress = 0;
    
    private readonly CancellationTokenSource _cancellationToken;

    public ProgressViewModel(StrongTypedLocalizationManager localization, IMessenger messenger) : base(localization, messenger)
    {
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
}
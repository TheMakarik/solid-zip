namespace SolidZip.ViewModels;

public sealed partial class StartupViewModel : ViewModelBase, IRecipient<UpdateProgressMessage>
{
    [ObservableProperty] private double _progress;
    
    public StartupViewModel(StrongTypedLocalizationManager localization, IMessenger messenger) : base(localization, messenger)
    {
        messenger.RegisterAll(this);
    }

    public void Receive(UpdateProgressMessage message)
    {
        Debug.Assert(message.Value is > 0 and < 100, "Progress must be between 0 and 100");
        Progress += message.Value;
    }
}
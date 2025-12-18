using SolidZip.ViewModels.Messages;

namespace SolidZip.ViewModels;

public partial class ViewModelBase : ObservableObject, IRecipient<ChangeLanguageMessage>
{
    [ObservableProperty] private StrongTypedLocalizationManager _Localization;
    private readonly IMessenger _messenger;
    
    public ViewModelBase(StrongTypedLocalizationManager localization, IMessenger messenger)
    {
        _messenger = messenger;
        _Localization = localization;
        _messenger.RegisterAll(this);
    }

    public void ChangeLanguage(CultureInfo culture)
    {
        Localization.ChangeLanguage(culture);
        _messenger.Send(new ChangeLanguageMessage(culture));
    }
    
    public void Receive(ChangeLanguageMessage message)
    {
        OnPropertyChanged(nameof(Localization));
    }
}
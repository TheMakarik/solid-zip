using SolidZip.ViewModels.Messages;

namespace SolidZip.ViewModels;

public abstract partial class ViewModelBase : ObservableObject, IRecipient<ChangeLanguageMessage>
{
    [ObservableProperty] private StrongTypedLocalizationManager _Localization;
    [ObservableProperty] private bool _localizationWasChanged = false;
    
    private readonly IMessenger _messenger;

    protected ViewModelBase(StrongTypedLocalizationManager localization, IMessenger messenger)
    {
        _messenger = messenger;
        _Localization = localization;
        _messenger.RegisterAll(this);
    }

    protected void ChangeLanguage(CultureInfo culture)
    {
        Localization.ChangeLanguage(culture);
        _messenger.Send(new ChangeLanguageMessage(culture));
    }
    
    public void Receive(ChangeLanguageMessage message)
    {
        LocalizationWasChanged = true;
        OnPropertyChanged(nameof(Localization));
    }
}
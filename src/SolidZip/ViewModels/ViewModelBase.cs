
namespace SolidZip.ViewModels;

public abstract partial class ViewModelBase : ObservableValidator, IRecipient<ChangeLanguageMessage>
{
    [ObservableProperty] private StrongTypedLocalizationManager _Localization;
    [ObservableProperty] private bool _localizationWasChanged = false;
    
    private readonly IMessenger _messenger;

    protected ViewModelBase(StrongTypedLocalizationManager localization, IMessenger messenger)
    {
        _messenger = messenger;
        _Localization = localization;
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
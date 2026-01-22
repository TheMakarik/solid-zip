namespace SolidZip.ViewModels;

public abstract partial class ViewModelBase(StrongTypedLocalizationManager localization, IMessenger messenger)
    : ObservableValidator, IRecipient<ChangeLanguageMessage>
{
    [ObservableProperty] private StrongTypedLocalizationManager _Localization = localization;
    [ObservableProperty] private bool _localizationWasChanged;

    public void Receive(ChangeLanguageMessage message)
    {
        LocalizationWasChanged = true;
        OnPropertyChanged(nameof(Localization));
    }

    protected void ChangeLanguage(CultureInfo culture)
    {
        Localization.ChangeLanguage(culture);
        messenger.Send(new ChangeLanguageMessage(culture));
    }
}
namespace SolidZip.ViewModels;

public partial class ErrorViewModel(StrongTypedLocalizationManager localization, IMessenger messenger)
    : ViewModelBase(localization, messenger)
{
    [ObservableProperty] private string _exception;
}
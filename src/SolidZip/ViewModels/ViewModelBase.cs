namespace SolidZip.ViewModels;

public partial class ViewModelBase(StrongTypedLocalizationManager localization) : ObservableObject
{
    [ObservableProperty] private StrongTypedLocalizationManager _Localization = localization;
}
using SolidZip.Localization;

namespace SolidZip.ViewModels;

public abstract partial class ViewModelBase(StrongTypedLocalizationManager localizationManager) : ObservableObject
{
    [ObservableProperty] private StrongTypedLocalizationManager _localizationManager = localizationManager;
}
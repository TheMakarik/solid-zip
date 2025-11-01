using SolidZip.Deprecated.Localization;

namespace SolidZip.Deprecated.ViewModels;

public abstract partial class ViewModelBase(StrongTypedLocalizationManager localizationManager) : ObservableObject
{
    [ObservableProperty] private StrongTypedLocalizationManager _localizationManager = localizationManager;
}
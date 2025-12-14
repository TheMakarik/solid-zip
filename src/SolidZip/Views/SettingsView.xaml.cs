namespace SolidZip.Views;

public partial class SettingsView
{
    private readonly StrongTypedLocalizationManager _localization;

    public SettingsView(StrongTypedLocalizationManager localization)
    {
        InitializeComponent();
        _localization = localization;
    }

    private void CloseSettings(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
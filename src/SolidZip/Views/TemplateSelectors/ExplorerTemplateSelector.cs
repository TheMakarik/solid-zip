namespace SolidZip.Views.TemplateSelectors;

public class ExplorerTemplateSelector : DataTemplateSelector
{
    public DataTemplate ListTemplate { get; set; }
    public DataTemplate DataGridTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        var userJsonManager = Ioc.Default.GetRequiredService<IUserJsonManager>();
        var viewTask = userJsonManager.GetExplorerElementsViewAsync();

        var view = viewTask.Result;

        return view switch
        {
            nameof(ExplorerElementsView.List) => ListTemplate,
            nameof(ExplorerElementsView.Grid) => DataGridTemplate,
            _ => null
        };
    }
}
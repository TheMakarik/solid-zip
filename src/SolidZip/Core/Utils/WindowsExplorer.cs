namespace SolidZip.Core.Utils;

public sealed class WindowsExplorer(StrongTypedLocalizationManager localizationManager)
{
    public Result<WindowsExplorerDialogResult, string> SelectFolder()
    {
        using var dialog = new CommonOpenFileDialog();

        dialog.IsFolderPicker = true;
        dialog.Title = localizationManager.SelectDirectory;

        var result = dialog.ShowDialog();
        return result switch
        {
            CommonFileDialogResult.Ok => new Result<WindowsExplorerDialogResult, string>(WindowsExplorerDialogResult.Ok,
                dialog.FileName),
            CommonFileDialogResult.Cancel => new Result<WindowsExplorerDialogResult, string>(WindowsExplorerDialogResult
                .Cancel),
            _ => new Result<WindowsExplorerDialogResult, string>(WindowsExplorerDialogResult.None)
        };
    }

    public void OpenFolder(string path)
    {
        if (Directory.Exists(path))
            Process.Start("cmd.exe", $"/C START {path}");
    }
}
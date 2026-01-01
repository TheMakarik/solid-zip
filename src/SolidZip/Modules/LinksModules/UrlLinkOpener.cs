
namespace SolidZip.Modules.LinksModules;

public sealed class UrlLinkOpener(ILogger<UrlLinkOpener> logger) : IUrlInBrowserOpener
{
    public void OpenUrl(string link)
    {
        if (!Uri.TryCreate(link, UriKind.Absolute, out Uri uri))
        {
            logger.LogError($"Invalid URL: {link}");
            return;
        }
        
        var processInfo = new ProcessStartInfo
        {
            FileName = link,
            UseShellExecute = true
        };
        
        Process.Start(processInfo);
    }
}
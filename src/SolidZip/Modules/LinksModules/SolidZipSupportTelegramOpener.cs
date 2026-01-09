namespace SolidZip.Modules.LinksModules;

public class SolidZipSupportTelegramOpener(IUrlInBrowserOpener urlInBrowserOpener, ILogger<SolidZipSupportTelegramOpener> logger, IOptions<TheMakariksOptions> options) : ISolidZipSupportInTelegramOpener
{
    public void Open()
    {
        logger.LogInformation("Opening Telegram support for this application");
        urlInBrowserOpener.OpenUrl(options.Value.TelegramLink);
    }
}
namespace SolidZip.Modules.HostedServices;

public class LogCompressor(
    PathsCollection paths,
    ILogger<LogCompressor> logger) : BackgroundService
{
    private static readonly TimeSpan OneWeek = TimeSpan.FromDays(7);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        { 
            await CompressOldLogsAsync(stoppingToken);
        }
        catch (Exception ex)
        { 
            logger.LogError(ex, "Error occurred while compressing logs");
        }
    }

    private async Task CompressOldLogsAsync(CancellationToken cancellationToken)
    {
        var logsDirectory = paths.Logging;
        var archivePath = paths.LogsArchive;

        if (!Directory.Exists(logsDirectory))
        {
            logger.LogInformation("Logs directory does not exist: {logsDirectory}", logsDirectory);
            return;
        }
        
        var archiveDirectory = Path.GetDirectoryName(archivePath);
        if (!string.IsNullOrEmpty(archiveDirectory) && !Directory.Exists(archiveDirectory))
        {
            Directory.CreateDirectory(archiveDirectory);
            logger.LogInformation("Created archive directory: {archiveDirectory}", archiveDirectory);
        }
        
        if (!File.Exists(archivePath))
            await CreateArchiveAsync(archivePath, cancellationToken);
        
        
        var cutoffDate = DateTime.Now - OneWeek;
        var logFilesEnumerable = Directory.GetFiles(logsDirectory, "log_*.txt")
            .Where(file => new FileInfo(file).LastWriteTime < cutoffDate);

        var filesEnumerable = logFilesEnumerable as string[] ?? logFilesEnumerable.ToArray();
        if (filesEnumerable.Any())
        {
            logger.LogInformation("No log files older than one week found");
            return;
        }
        
        var logFiles = filesEnumerable.ToArray();

        logger.LogInformation("Found {count} log files to compress", logFiles.Length);
        await AddFilesToArchiveAsync(archivePath, logFiles, cancellationToken);
        
        foreach (var logFile in logFiles)
        {
            try
            {
                File.Delete(logFile);
                logger.LogDebug("Deleted compressed log file: {logFile}", logFile);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to delete log file: {logFile}", logFile);
            }
        }

        logger.LogInformation("Successfully compressed {count} log files", logFiles.Length);
    }

    private async Task CreateArchiveAsync(string archivePath, CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            using var zip = new ZipFile();
            zip.Save(archivePath);
        }, cancellationToken);
        
        logger.LogInformation("Created new log archive: {archivePath}", archivePath);
    }

    private async Task AddFilesToArchiveAsync(string archivePath, IEnumerable<string> logFiles, CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            using var zip = new ZipFile(archivePath);
            
            foreach (var logFile in logFiles)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var fileName = Path.GetFileName(logFile);
                if (zip[fileName] is not null)
                {
                    logger.LogDebug("File already exists in archive, skipping: {fileName}", fileName);
                    continue;
                }

                zip.AddFile(logFile, string.Empty);
                logger.LogDebug("Added file to archive: {logFile}", logFile);
            }

            zip.Save();
        }, cancellationToken);
    }
}
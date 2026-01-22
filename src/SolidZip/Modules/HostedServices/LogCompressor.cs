namespace SolidZip.Modules.HostedServices;

public class LogCompressor(PathsCollection paths) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
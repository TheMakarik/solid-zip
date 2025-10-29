using SolidZip.Services.FactoriesServices.Abstractions;

namespace SolidZip.Services.JsonSerializationServices;

internal sealed class JsonSerializer(
    ILogger<JsonSerializer> logger,
    IFileStreamFactory fileStreamFactory
    ) : IJsonSerializer
{
    private const string SerializingFromPathLogMessage = "Serializing JSON from {path} for {time} milliseconds";
    private const string DeserializingFromPathLogMessage = "Deserializing from JSON at {path} for {time} milliseconds";
    
    
    //In some cases I need to control FileStream by myself, but for logging I'm still need path
    public async Task SerializeAsync<T>(T entity, FileStream stream, string pathForLogging, JsonSerializerOptions? options = null)
    {
        var stopwatch = Stopwatch.StartNew();
        await System.Text.Json.JsonSerializer.SerializeAsync(stream, entity, options);
        stopwatch.Stop();
        logger.LogInformation(SerializingFromPathLogMessage, pathForLogging, stopwatch.ElapsedMilliseconds);
    }

    public async Task<T?> DeserializeAsync<T>(string pathForLogging, FileStream stream, JsonSerializerOptions? options = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream,  options);
        stopwatch.Stop();
        
        logger.LogDebug(DeserializingFromPathLogMessage, pathForLogging, stopwatch.ElapsedMilliseconds);
        return result;
    }

    public async Task SerializeAsync<T>(T entity, string path, JsonSerializerOptions? options = null)
    {
        await using var stream = fileStreamFactory.GetFactory(path, FileMode.Truncate);
        await SerializeAsync(entity, stream, path, options);
    }

    public async Task<T?> DeserializeAsync<T>(string path, JsonSerializerOptions? options = null)
    {
        await using var stream = fileStreamFactory.GetFactory(path, FileMode.Open);
        return await DeserializeAsync<T>(path, stream, options);
    }
    
}
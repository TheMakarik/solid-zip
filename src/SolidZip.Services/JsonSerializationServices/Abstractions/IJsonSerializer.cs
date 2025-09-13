namespace SolidZip.Services.JsonSerializationServices.Abstractions;

public interface IJsonSerializer
{
    public Task SerializeAsync<T>(T entity, FileStream stream, string pathForLogging, JsonSerializerOptions? options = null);
    public Task<T?> DeserializeAsync<T>(string pathForLogging, FileStream stream, JsonSerializerOptions? options = null);
    public Task SerializeAsync<T>(T entity, string path, JsonSerializerOptions? options = null);
    public Task<T?> DeserializeAsync<T>(string path, JsonSerializerOptions? options = null);
}
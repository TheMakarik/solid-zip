namespace SolidZip.Services.JsonSerializationServices.Contexts;

[JsonSerializable(typeof(AppDataContent))]
[JsonSourceGenerationOptions(
    Converters = [typeof(Converters.StringToCultureInfoConverter)], 
    PropertyNamingPolicy = JsonKnownNamingPolicy.KebabCaseLower)]
internal partial class AppDataContentSerializerContext : JsonSerializerContext;
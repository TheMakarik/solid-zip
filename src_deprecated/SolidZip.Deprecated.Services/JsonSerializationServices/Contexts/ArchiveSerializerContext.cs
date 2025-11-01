namespace SolidZip.Services.JsonSerializationServices.Contexts;

[JsonSerializable(typeof(ArchiveConfiguration))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.KebabCaseLower)]
internal partial class ArchiveSerializerContext : JsonSerializerContext;
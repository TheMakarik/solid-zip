namespace SolidZip.Core.JsonContext;

[JsonSerializable(typeof(UserData))]
[JsonSourceGenerationOptions(
    Converters = [typeof(Converters.StringToCultureInfoConverter)], 
    PropertyNamingPolicy = JsonKnownNamingPolicy.KebabCaseLower)]
internal partial class UserDataSerializerContext : JsonSerializerContext;
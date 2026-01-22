using SolidZip.Core.JsonContext.Converters;

namespace SolidZip.Core.JsonContext;

[JsonSerializable(typeof(UserData))]
[JsonSourceGenerationOptions(
    Converters = [typeof(StringToCultureInfoConverter)],
    PropertyNamingPolicy = JsonKnownNamingPolicy.KebabCaseLower)]
internal partial class UserDataSerializerContext : JsonSerializerContext;
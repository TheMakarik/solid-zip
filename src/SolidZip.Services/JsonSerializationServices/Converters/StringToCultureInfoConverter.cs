namespace SolidZip.Services.JsonSerializationServices.Converters;

public class StringToCultureInfoConverter : JsonConverter<CultureInfo>
{
    public override CultureInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var cultureName = reader!.GetString();
        return new CultureInfo(cultureName!);
    }

    public override void Write(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}
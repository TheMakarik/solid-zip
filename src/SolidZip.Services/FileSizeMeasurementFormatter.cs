namespace SolidZip.Services;

public sealed class FileSizeMeasurementFormatter
{
    public decimal ToKiloBytes(ulong bytes)
    {
        return Convert.ToDecimal(bytes) / 1024;
    }

    public decimal ToMegaBytes(ulong bytes)
    {
        return ToKiloBytes(bytes) / 1024;
    }
}
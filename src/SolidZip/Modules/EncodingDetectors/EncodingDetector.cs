namespace SolidZip.Modules.EncodingDetectors;

public class EncodingDetector(ILogger<EncodingDetector> logger) : IEncodingDetector
{
    public Encoding DetectEncoding(string path)
    {
        logger.LogInformation("Start detecting encoding for {path}", path);
        
        
        using var file = File.OpenRead(path);
        var ude = new Ude.CharsetDetector();
        ude.Feed(file);
        ude.DataEnd();
        if (ude.Charset is not null)
        {
            logger.LogInformation("Encoding {encoding} detected for {path}", ude.Charset, path);
            return Encoding.GetEncoding(ude.Charset);
        }
        logger.LogError("Cannot detect encoding for {path}, Default encoding will be returned", path);
        return Encoding.Default;
    }
}
namespace SolidZip.Core.Contracts.EncodingDetectors;

public interface IEncodingDetector
{
    public Encoding DetectEncoding(string path);
}
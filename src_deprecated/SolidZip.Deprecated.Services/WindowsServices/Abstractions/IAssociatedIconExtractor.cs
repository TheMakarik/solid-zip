
namespace SolidZip.Services.WindowsServices.Abstractions;

public interface IAssociatedIconExtractor
{
   public IconInfo Extract(string path);
}
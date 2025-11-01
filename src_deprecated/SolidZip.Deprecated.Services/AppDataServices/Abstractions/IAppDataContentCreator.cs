namespace SolidZip.Services.AppDataServices.Abstractions;

public interface IAppDataContentCreator
{
    public ValueTask CreateAsync();
}
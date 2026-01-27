namespace SolidZip.Core.Contracts.StateMachines;

public interface IIconExtractorStateMachine
{
    public IconInfo ExtractIcon(FileEntity fileEntity);
}
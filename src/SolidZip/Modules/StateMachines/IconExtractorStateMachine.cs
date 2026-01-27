namespace SolidZip.Modules.StateMachines;

public sealed class IconExtractorStateMachine(
    IFileSystemStateMachine stateMachine,
    AssociatedIconExtractor associatedIconExtractor,
    ExtensionIconExtractor extensionIconExtractor) : IIconExtractorStateMachine
{
    public IconInfo ExtractIcon(FileEntity fileEntity)
    {
        return stateMachine.GetState() == FileSystemState.Directory
            ? associatedIconExtractor.Extract(fileEntity)
            : extensionIconExtractor.Extract(fileEntity);
    }
}

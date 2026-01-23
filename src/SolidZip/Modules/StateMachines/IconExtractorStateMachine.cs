namespace SolidZip.Modules.StateMachines;

public sealed class IconExtractorStateMachine(
    IFileSystemStateMachine stateMachine,
    AssociatedIconExtractor associatedIconExtractor,
    ExtensionIconExtractor extensionIconExtractor) : IIconExtractorStateMachine
{
    public IconInfo ExtractIcon(string path)
    {
        return stateMachine.GetState() == FileSystemState.Directory
            ? associatedIconExtractor.Extract(path)
            : extensionIconExtractor.Extract(path);
    }
}
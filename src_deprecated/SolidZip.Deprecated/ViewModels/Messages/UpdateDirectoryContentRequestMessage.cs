namespace SolidZip.Deprecated.ViewModels.Messages;

public sealed class UpdateDirectoryContentRequestMessage
    : RequestMessage<(IEnumerable<FileEntity> Entites, ExplorerResult Result)>
{
    public required FileEntity Directory { get; set; }
}
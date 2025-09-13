namespace SolidZip.ViewModels.Messages;

public sealed class FileEntityForGettingContentMessage(FileEntity directory) : ValueChangedMessage<FileEntity>(directory);
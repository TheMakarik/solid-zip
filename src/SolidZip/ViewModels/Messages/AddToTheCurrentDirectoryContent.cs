namespace SolidZip.ViewModels.Messages;

public class AddToTheCurrentDirectoryContent(FileEntity value) : ValueChangedMessage<FileEntity>(value);
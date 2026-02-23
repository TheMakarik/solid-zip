namespace SolidZip.ViewModels.Messages;

public class AddToTheCurrentDirectoryContentMessage(FileEntity value) : ValueChangedMessage<FileEntity>(value);
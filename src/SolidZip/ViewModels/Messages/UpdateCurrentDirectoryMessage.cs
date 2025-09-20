namespace SolidZip.ViewModels.Messages;

public sealed class UpdateCurrentDirectoryMessage(string newCurrentDirectory) : ValueChangedMessage<string>(newCurrentDirectory);
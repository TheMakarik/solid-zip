namespace SolidZip.ViewModels.Messages;

public class MoveExplorerCurrentDirectoryToMessage(string value) : ValueChangedMessage<string>(value);
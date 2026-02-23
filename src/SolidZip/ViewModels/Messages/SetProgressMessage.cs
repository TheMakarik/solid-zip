namespace SolidZip.ViewModels.Messages;

public sealed class SetProgressMessage(string value) : ValueChangedMessage<string>(value);